using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Application.CommandPipeline;
using Game.Application.Events;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class EventTests
    {
        private static readonly EntityId<Actor> ActorId = new EntityId<Actor>(new EntityId(1, "actor", 1));
        private static readonly EntityId<Lot> Source = new EntityId<Lot>(new EntityId(1, "lot", 1));
        private static readonly EntityId<Lot> Destination = new EntityId<Lot>(new EntityId(1, "lot", 2));
        private static readonly ExecutionPrincipal Principal = new ExecutionPrincipal(ActorId, CommandSource.Player);
        private static CommandProcessor Processor(Action? before = null, Action? after = null)
        {
            var stage = IdentitySnapshot.Empty(1).BeginStage(1);
            stage.Allocate<Actor>(); stage.Allocate<Lot>(); stage.Allocate<Lot>(); var location = stage.Allocate<Settlement>();
            var world = new WorldSnapshot(stage.PrepareSnapshot(), new WorldTick(50), new[] { new ActorState(ActorId, true, location) }, new[] {
                new ResourceLot(Source, DefinitionId.Parse("commodity.timber@1"), ActorId.Value, location, 10, 0),
                new ResourceLot(Destination, DefinitionId.Parse("commodity.timber@1"), ActorId.Value, location, 0, 0) });
            return new CommandProcessor(world, new RuleRegistry(new[] { new TransferResourceRule() }), before, after);
        }
        private static CommandEnvelope Command(string id = "c1", ulong revision = 1, long quantity = 2, bool same = false, string correlation = "flow1")
            => new CommandEnvelope(id, "TransferResource", ActorId.ToString(), "Player", 50, revision, 1,
                new Dictionary<string, object?> { ["source_lot_id"] = Source.ToString(), ["destination_lot_id"] = (same ? Source : Destination).ToString(), ["quantity"] = quantity }, correlation);
        private static CommittedEvent Fact(ulong id = 1, ulong sequence = 2, uint index = 0, int version = 1, long quantity = 2, ulong world = 1,
            string rule = "transfer-resource-v1", string correlation = "flow1", long tick = 50)
            => new CommittedEvent(new EntityId<Event>(new EntityId(world, "event", id)), "ResourceTransferred", new EntityId(world, "lot", 1),
                new WorldTick(tick), sequence, index, "c1", correlation, rule, version, new Dictionary<string, object?> { ["quantity"] = quantity });
        private static ProjectionState Project(ProjectionState state, CommittedEvent fact) => new ProjectionState(new Dictionary<string, long> {
            ["total"] = checked((state.Values.TryGetValue("total", out var total) ? total : 0) + (long)fact.Payload["quantity"]!) });

        [Test]
        public void TEST_037_CommittedEnvelopeContainsStableVersionedCausalFields()
        {
            var processor = Processor(); var receipt = processor.Execute(Command(), Principal); var snapshot = processor.InspectCommitted();
            var fact = snapshot.Journal.Events.Single();
            Assert.That(fact, Is.SameAs(snapshot.Events.Single().Envelope));
            Assert.That(fact.Id, Is.EqualTo(receipt.OrderedEventIds.Single()));
            Assert.That(fact.AggregateId, Is.EqualTo(Source.Value));
            Assert.That(fact.EventType, Is.EqualTo("ResourceTransferred"));
            Assert.That(fact.Tick.Value, Is.EqualTo(50)); Assert.That(fact.CommitSequence, Is.EqualTo(2));
            Assert.That(fact.EventIndex, Is.Zero); Assert.That(fact.CausationId, Is.EqualTo("c1"));
            Assert.That(fact.CorrelationId, Is.EqualTo("flow1")); Assert.That(fact.RuleVersion, Is.EqualTo("transfer-resource-v1"));
            Assert.That(fact.EnvelopeVersion, Is.EqualTo(1)); Assert.That(fact.PayloadVersion, Is.EqualTo(1));
            Assert.That(fact.Payload["quantity"], Is.EqualTo(2L));
            Assert.That(fact.Payload["source_lot_id"], Is.EqualTo(Source.ToString()));
            Assert.That(fact.Payload["destination_lot_id"], Is.EqualTo(Destination.ToString()));
        }
        [Test]
        public void TEST_037_JournalQueriesPreserveCommitOrderAndSnapshotIsolation()
        {
            var processor = Processor(); processor.Execute(Command(), Principal); var first = processor.InspectCommitted();
            processor.Execute(Command("c2", 2), Principal); processor.Execute(Command("c3", 3, correlation: "flow2"), Principal);
            var journal = processor.InspectCommitted().Journal;
            Assert.That(journal.Events.Select(e => e.CommitSequence), Is.EqualTo(new ulong[] { 2, 3, 4 }));
            Assert.That(journal.QueryByCorrelation("flow1").Select(e => e.CausationId), Is.EqualTo(new[] { "c1", "c2" }));
            Assert.That(journal.QueryByCausation("c2").Single().CommitSequence, Is.EqualTo(3));
            Assert.That(journal.QueryByAggregate(Source.Value).Count, Is.EqualTo(3));
            Assert.That(journal.QueryByAggregate(Destination.Value), Is.Empty);
            Assert.That(journal.QueryByCausation("missing"), Is.Empty); Assert.That(first.Journal.Events.Count, Is.EqualTo(1));
            Assert.Throws<NotSupportedException>(() => ((IList<CommittedEvent>)journal.Events).Clear());
        }
        [Test]
        public void TEST_037_RollbackNoopRejectionRetryAndLostResponseNeverEmitExtraFacts()
        {
            var fail = true; var processor = Processor(before: () => { if (fail) throw new InvalidOperationException(); });
            Assert.Throws<InvalidOperationException>(() => processor.Execute(Command(), Principal));
            Assert.That(processor.InspectCommitted().Journal.Events, Is.Empty); fail = false;
            processor.Execute(Command("noop", same: true), Principal); processor.Execute(Command("reject", quantity: 99), Principal);
            Assert.That(processor.InspectCommitted().Journal.Events, Is.Empty);
            processor.Execute(Command(), Principal); processor.Execute(Command(), Principal);
            Assert.That(processor.InspectCommitted().Journal.Events.Count, Is.EqualTo(1));
            Assert.That(processor.InspectCommitted().Journal.Events.Single().Id.Value.Counter, Is.EqualTo(1));
            var lost = Processor(after: () => throw new InvalidOperationException());
            Assert.Throws<InvalidOperationException>(() => lost.Execute(Command(), Principal)); lost.Execute(Command(), Principal);
            Assert.That(lost.InspectCommitted().Journal.Events.Count, Is.EqualTo(1));
        }
        [Test]
        public void TEST_037_EnvelopeCopiesPayloadAndRejectsMutableOrCrossWorldInput()
        {
            var payload = new Dictionary<string, object?> { ["quantity"] = 2L };
            var fact = new CommittedEvent(Fact().Id, "x", Source.Value, new WorldTick(50), 2, 0, "c", "f", "v1", 1, payload);
            var hash = fact.Fingerprint(); payload["quantity"] = 9L;
            Assert.That(fact.Fingerprint(), Is.EqualTo(hash));
            Assert.Throws<NotSupportedException>(() => ((IDictionary<string, object?>)fact.Payload)["quantity"] = 4L);
            Assert.Throws<ArgumentOutOfRangeException>(() => new CommittedEvent(Fact().Id, "x", Source.Value, new WorldTick(50), 2, 0, "c", "f", "v1", 1, payload, envelopeVersion: 2));
            payload["bad"] = new List<int>();
            Assert.Throws<ArgumentException>(() => new CommittedEvent(Fact().Id, "x", Source.Value, new WorldTick(50), 2, 0, "c", "f", "v1", 1, payload));
            Assert.Throws<ArgumentException>(() => new CommittedEvent(Fact().Id, "x", new EntityId(2,"lot",1), new WorldTick(50), 2, 0, "c", "f", "v1", 1, new Dictionary<string, object?>()));
        }
        [Test]
        public void TEST_037_ConsumerDuplicateAndConcurrentDeliveryAppliesOnce()
        {
            var consumer = new InMemoryEventConsumer(); var fact = Fact(); var applied = new bool[20];
            Parallel.For(0, applied.Length, i => applied[i] = consumer.Consume(fact, Project));
            Assert.That(applied.Count(x => x), Is.EqualTo(1)); var publication = consumer.InspectCommitted();
            Assert.That(publication.Projection.Values["total"], Is.EqualTo(2));
            Assert.That(publication.AppliedFingerprints.Count, Is.EqualTo(1)); Assert.That(publication.Checkpoint, Is.SameAs(fact));
            Assert.That(consumer.Consume(fact, (_, __) => throw new Exception("must not execute")), Is.False);
            Assert.That(consumer.InspectCommitted(), Is.SameAs(publication));
        }
        [TestCase("payload")]
        [TestCase("version")]
        [TestCase("rule")]
        [TestCase("correlation")]
        public void TEST_037_ConsumerIdentityCollisionRejectsChangedContentOrVersion(string field)
        {
            var consumer = new InMemoryEventConsumer(); consumer.Consume(Fact(), Project); var before = consumer.InspectCommitted();
            var changed = Fact(quantity: field == "payload" ? 9 : 2, version: field == "version" ? 2 : 1,
                rule: field == "rule" ? "v2" : "transfer-resource-v1", correlation: field == "correlation" ? "other" : "flow1");
            Assert.That(Assert.Throws<InvalidOperationException>(() => consumer.Consume(changed, Project))!.Message, Is.EqualTo("EVENT_IDENTITY_COLLISION"));
            Assert.That(consumer.InspectCommitted(), Is.SameAs(before));
        }
        [TestCase("propose")]
        [TestCase("before")]
        [TestCase("after")]
        public void TEST_037_ConsumerFailureRetryPublishesProjectionAndCheckpointTogether(string stage)
        {
            var fail = true; var consumer = new InMemoryEventConsumer(
                beforeCommit: () => { if (fail && stage == "before") throw new InvalidOperationException(); },
                afterCommit: () => { if (fail && stage == "after") throw new InvalidOperationException(); });
            var initial = consumer.InspectCommitted();
            Assert.Throws<InvalidOperationException>(() => consumer.Consume(Fact(), (state, fact) => {
                if (fail && stage == "propose") throw new InvalidOperationException(); return Project(state, fact); }));
            if (stage != "after") Assert.That(consumer.InspectCommitted(), Is.SameAs(initial));
            else Assert.That(consumer.InspectCommitted().Projection.Values["total"], Is.EqualTo(2));
            fail = false; Assert.That(consumer.Consume(Fact(), Project), Is.EqualTo(stage != "after"));
            Assert.That(consumer.InspectCommitted().Projection.Values["total"], Is.EqualTo(2));
            Assert.That(consumer.InspectCommitted().AppliedFingerprints.Count, Is.EqualTo(1));
        }
        [Test]
        public void TEST_037_ConsumerRejectsUnseenOlderEqualPositionForeignWorldAndRegressedTick()
        {
            var consumer = new InMemoryEventConsumer(); consumer.Consume(Fact(sequence: 3), Project); var before = consumer.InspectCommitted();
            foreach (var other in new[] { Fact(id: 2, sequence: 2), Fact(id: 2, sequence: 3), Fact(id: 2, sequence: 4, world: 2), Fact(id: 2, sequence: 4, tick: 49) })
                Assert.Throws<InvalidOperationException>(() => consumer.Consume(other, Project));
            Assert.That(consumer.InspectCommitted(), Is.SameAs(before));
            consumer.Consume(Fact(id: 2, sequence: 3, index: 1), Project);
            consumer.Consume(Fact(id: 3, sequence: 4), Project);
            Assert.That(consumer.InspectCommitted().Projection.Values["total"], Is.EqualTo(6));
            var latest = consumer.InspectCommitted();
            Assert.That(consumer.Consume(Fact(sequence: 3), (_, __) => throw new Exception("old duplicate must skip")), Is.False);
            Assert.That(consumer.InspectCommitted(), Is.SameAs(latest));
        }
        [Test]
        public void TEST_037_ProjectionAndCheckpointAreImmutableAndReentryCannotPublish()
        {
            var values = new Dictionary<string, long> { ["total"] = 5 }; var projection = new ProjectionState(values); values["total"] = 99;
            Assert.That(projection.Values["total"], Is.EqualTo(5));
            Assert.Throws<NotSupportedException>(() => ((IDictionary<string,long>)projection.Values)["total"] = 8);
            var consumer = new InMemoryEventConsumer(); var before = consumer.InspectCommitted();
            Assert.Throws<InvalidOperationException>(() => consumer.Consume(Fact(), (_, __) => {
                consumer.Consume(Fact(id: 2, sequence: 3), Project); return projection; }));
            Assert.That(consumer.InspectCommitted(), Is.SameAs(before));
            consumer.Consume(Fact(), Project); var committed = consumer.InspectCommitted();
            Assert.Throws<NotSupportedException>(() => ((IDictionary<string,string>)committed.AppliedFingerprints).Clear());
            consumer.Consume(Fact(id: 2, sequence: 3), Project);
            Assert.That(committed.Projection.Values["total"], Is.EqualTo(2)); Assert.That(committed.AppliedFingerprints.Count, Is.EqualTo(1));
        }
    }
}
