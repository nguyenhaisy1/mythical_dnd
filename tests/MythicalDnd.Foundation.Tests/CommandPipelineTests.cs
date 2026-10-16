using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using Game.Foundation.Random;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class CommandPipelineTests
    {
        private static readonly EntityId<Actor> ActorId = new EntityId<Actor>(new EntityId(1, "actor", 1));
        private static readonly EntityId<Lot> Source = new EntityId<Lot>(new EntityId(1, "lot", 1));
        private static readonly EntityId<Lot> Destination = new EntityId<Lot>(new EntityId(1, "lot", 2));
        private static readonly EntityId<Settlement> Location = new EntityId<Settlement>(new EntityId(1, "settlement", 1));
        private static readonly ExecutionPrincipal Principal = new ExecutionPrincipal(ActorId, CommandSource.Player);
        private static WorldSnapshot World(long quantity = 10, long reserved = 2, long target = 3, ulong revision = 1)
        {
            var stage = IdentitySnapshot.Empty(1).BeginStage(1);
            stage.Allocate<Actor>(); stage.Allocate<Lot>(); stage.Allocate<Lot>(); stage.Allocate<Settlement>();
            var ids = stage.PrepareSnapshot();
            if (revision != 1) ids = IdentitySnapshot.Restore(1, revision, ids.Cursors, ids.Entries);
            return new WorldSnapshot(ids, new WorldTick(50), new[] { new ActorState(ActorId, true, Location) }, new[] {
                new ResourceLot(Source, DefinitionId.Parse("commodity.timber@1"), ActorId.Value, Location, quantity, reserved),
                new ResourceLot(Destination, DefinitionId.Parse("commodity.timber@1"), ActorId.Value, Location, target, 0) }, new[] { new RngStreamState("transfer-fixture", new RngState(new RngContext(new string('0',64), "assets", "fixture", "first", ActorId.Value, "main"), 7)) });
        }
        private static Dictionary<string, object?> Payload(long quantity = 4, bool same = false) => new Dictionary<string, object?> {
            ["source_lot_id"] = Source.ToString(), ["destination_lot_id"] = (same ? Source : Destination).ToString(), ["quantity"] = quantity };
        private static CommandEnvelope Command(string id = "command.1", ulong revision = 1, long quantity = 4, long tick = 50,
            string source = "Player", string correlation = "correlation.1", int version = 1, string type = "TransferResource",
            Dictionary<string, object?>? payload = null) => new CommandEnvelope(id, type, ActorId.ToString(), source, tick, revision,
                version, payload ?? Payload(quantity), correlation);
        private static CommandProcessor Processor(WorldSnapshot? world = null, Action? before = null, Action? after = null,
            IRuleHandler? handler = null) => new CommandProcessor(world ?? World(), new RuleRegistry(new[] { handler ?? new TransferResourceRule() }), before, after);
        private static void Unchanged(CommittedWorld before, CommittedWorld after)
        {
            Assert.That(after.World, Is.SameAs(before.World));
            Assert.That(after.AcceptedReceipts, Is.EqualTo(before.AcceptedReceipts));
            Assert.That(after.Events, Is.EqualTo(before.Events));
        }

        [Test]
        public void TEST_024_TransferCommitsWorldReceiptEventIdentityTogetherAndKeepsSnapshotImmutable()
        {
            var processor = Processor(); var before = processor.InspectCommitted();
            var receipt = processor.Execute(Command(), Principal); var after = processor.InspectCommitted();
            Assert.That(receipt.Outcome, Is.EqualTo(CommandOutcome.Accepted));
            Assert.That(after.World.Revision.Value, Is.EqualTo(2UL));
            Assert.That(after.World.GetLot(Source).OnHand, Is.EqualTo(6));
            Assert.That(after.World.GetLot(Destination).OnHand, Is.EqualTo(7));
            Assert.That(after.World.GetLot(Source).Reserved, Is.EqualTo(2));
            Assert.That(before.World.GetLot(Source).OnHand, Is.EqualTo(10));
            Assert.That(after.AcceptedReceipts.Single(), Is.SameAs(receipt));
            Assert.That(after.Events.Single().Id, Is.EqualTo(receipt.OrderedEventIds.Single()));
            Assert.That(after.World.Identity.Resolve<Event>(receipt.OrderedEventIds.Single().Value).CreationRevision, Is.EqualTo(2UL));
            Assert.That(before.World.Identity.Cursor("event").NextCounter, Is.EqualTo(1UL));
            Assert.That(after.World.Identity.Cursor("event").NextCounter, Is.EqualTo(2UL));
            Assert.That(after.World.Tick, Is.EqualTo(before.World.Tick));
            Assert.That(after.World.RngStates, Is.EqualTo(before.World.RngStates));
            Assert.That(after.MetadataReceipts, Is.Empty);
        }

        [Test]
        public void TEST_024_PrecommitFailureRollsBackAllAndRetryAllocatesSameEvent()
        {
            var fail = true;
            var processor = Processor(before: () => { if (fail) throw new InvalidOperationException("injected precommit"); });
            var before = processor.InspectCommitted();
            Assert.Throws<InvalidOperationException>(() => processor.Execute(Command(), Principal));
            Assert.That(processor.InspectCommitted(), Is.SameAs(before));
            fail = false;
            var result = processor.Execute(Command(), Principal);
            Assert.That(result.OrderedEventIds.Single().Value.Counter, Is.EqualTo(1UL));
            Assert.That(processor.InspectCommitted().World.GetLot(Source).OnHand, Is.EqualTo(6));
        }

        [Test]
        public void TEST_024_AfterCommitResponseLossRetriesReceiptWithoutTransferringAgain()
        {
            var probes = 0;
            var processor = Processor(after: () => { probes++; throw new InvalidOperationException("response lost"); });
            Assert.Throws<InvalidOperationException>(() => processor.Execute(Command(), Principal));
            var committed = processor.InspectCommitted();
            var retry = processor.Execute(Command(), Principal);
            Assert.That(retry, Is.SameAs(committed.AcceptedReceipts.Single()));
            Assert.That(processor.InspectCommitted(), Is.SameAs(committed));
            Assert.That(probes, Is.EqualTo(1));
        }

        [Test]
        public void TEST_026_DuplicatePrecedesStaleAndMismatchNeverReplacesReceipt()
        {
            var processor = Processor(); var original = processor.Execute(Command(), Principal);
            Assert.That(processor.Execute(Command(), Principal), Is.SameAs(original));
            Assert.That(processor.Execute(Command(quantity: 3), Principal).ErrorCode, Is.EqualTo("CMD_PAYLOAD_MISMATCH"));
            Assert.That(processor.Execute(Command(), Principal), Is.SameAs(original));
            Assert.That(processor.InspectCommitted().AcceptedReceipts.Count, Is.EqualTo(1));
            Assert.That(processor.InspectCommitted().MetadataReceipts, Is.Empty);
        }

        [TestCase("STALE_REVISION", 0UL, 50L, 4L)]
        [TestCase("TIME_MISMATCH", 1UL, 51L, 4L)]
        [TestCase("INSUFFICIENT_RESOURCE", 1UL, 50L, 9L)]
        [TestCase("INVALID_PAYLOAD", 1UL, 50L, 0L)]
        public void TEST_025_RejectionIsMetadataOnlyAndRetryStable(string error, ulong revision, long tick, long quantity)
        {
            var processor = Processor(); var before = processor.InspectCommitted();
            var command = Command(revision: revision, tick: tick, quantity: quantity);
            var rejected = processor.Execute(command, Principal);
            Assert.That(rejected.ErrorCode, Is.EqualTo(error));
            Unchanged(before, processor.InspectCommitted());
            Assert.That(processor.InspectCommitted().MetadataReceipts.Single(), Is.SameAs(rejected));
            Assert.That(processor.Execute(command, Principal), Is.SameAs(rejected));
        }

        [Test]
        public void TEST_025_AcceptedNoopDoesNotConsumeIdsRngRevisionOrEvents()
        {
            var processor = Processor(); var before = processor.InspectCommitted();
            var command = Command(payload: Payload(same: true));
            var receipt = processor.Execute(command, Principal);
            Assert.That(receipt.Outcome, Is.EqualTo(CommandOutcome.NoOp));
            Unchanged(before, processor.InspectCommitted());
            Assert.That(processor.InspectCommitted().MetadataReceipts.Single(), Is.SameAs(receipt));
            Assert.That(processor.Execute(command, Principal), Is.SameAs(receipt));
        }

        [TestCase("")]
        [TestCase("bad id")]
        [TestCase("command\n")]
        public void TEST_025_MalformedIdNeverPersistsReceipt(string id)
        {
            var processor = Processor(); var before = processor.InspectCommitted();
            Assert.That(processor.Execute(Command(id: id), Principal).ErrorCode, Is.EqualTo("INVALID_ENVELOPE"));
            Assert.That(processor.InspectCommitted(), Is.SameAs(before));
        }

        [Test]
        public void TEST_025_PayloadCannotSpoofTrustedSchedulerAndPoisonReceipt()
        {
            var processor = Processor(); var before = processor.InspectCommitted();
            Assert.That(processor.Execute(Command(source: "Scheduler"), Principal).ErrorCode, Is.EqualTo("NOT_AUTHORIZED"));
            Assert.That(processor.InspectCommitted(), Is.SameAs(before));
            Assert.That(processor.Execute(Command(), Principal).Outcome, Is.EqualTo(CommandOutcome.Accepted));
            var other = new ExecutionPrincipal(new EntityId<Actor>(new EntityId(1, "actor", 2)), CommandSource.Player);
            Assert.That(processor.Execute(Command(), other).ErrorCode, Is.EqualTo("NOT_AUTHORIZED"));
        }

        [Test]
        public void TEST_026_EveryEnvelopeFieldParticipatesInFingerprintAndPayloadIsCopied()
        {
            var payload = Payload(); var original = Command(payload: payload); var hash = original.Fingerprint();
            payload["quantity"] = 999L;
            Assert.That(original.Fingerprint(), Is.EqualTo(hash));
            var variants = new[] { Command(id: "other"), Command(revision: 2), Command(tick: 51), Command(quantity: 5),
                Command(source: "NPC"), Command(correlation: "other"), Command(version: 2), Command(type: "other"),
                new CommandEnvelope("command.1", "TransferResource", new EntityId(1,"actor",2).ToString(), "Player",50,1,1,Payload(),"correlation.1") };
            Assert.That(variants.All(c => c.Fingerprint() != hash), Is.True);
            Assert.That(Command(payload: Payload().Reverse().ToDictionary(p => p.Key,p => p.Value)).Fingerprint(), Is.EqualTo(hash));
            Assert.Throws<NotSupportedException>(() => ((IDictionary<string, object?>)original.Payload).Add("extra", 1));
        }

        [TestCase("COMMAND_UNKNOWN", "Other", 1)]
        [TestCase("PAYLOAD_VERSION_UNSUPPORTED", "TransferResource", 2)]
        public void TEST_025_UnknownCommandOrVersionRejectsWithoutMutation(string error, string type, int version)
        {
            var processor = Processor(); var before = processor.InspectCommitted();
            Assert.That(processor.Execute(Command(type: type, version: version), Principal).ErrorCode, Is.EqualTo(error));
            Unchanged(before, processor.InspectCommitted());
        }

        [Test]
        public void TEST_025_UnknownPayloadFieldAndWrongReferenceTypeReject()
        {
            var processor = Processor(); var extra = Payload(); extra.Add("owner", "cheat");
            Assert.That(processor.Execute(Command(payload: extra), Principal).ErrorCode, Is.EqualTo("INVALID_PAYLOAD"));
            var wrong = Payload(); wrong["source_lot_id"] = ActorId.ToString();
            Assert.That(processor.Execute(Command(id: "command.2", payload: wrong), Principal).ErrorCode, Is.EqualTo("REF_TYPE_MISMATCH"));
            Assert.That(processor.InspectCommitted().World.Revision.Value, Is.EqualTo(1UL));
        }

        [Test]
        public void TEST_026_ConcurrentSameEnvelopeCommitsExactlyOnce()
        {
            var processor = Processor(); var results = new CommandReceipt[32];
            Parallel.For(0, results.Length, i => results[i] = processor.Execute(Command(), Principal));
            Assert.That(results.All(r => ReferenceEquals(r, results[0])), Is.True);
            Assert.That(processor.InspectCommitted().World.Revision.Value, Is.EqualTo(2UL));
            Assert.That(processor.InspectCommitted().Events.Count, Is.EqualTo(1));
        }

        [Test]
        public void TEST_026_CompetingRevisionHasOneWinnerAndOneStaleReceipt()
        {
            var processor = Processor(); var results = new CommandReceipt[2];
            Parallel.For(0, 2, i => results[i] = processor.Execute(Command(id: "command." + i), Principal));
            Assert.That(results.Count(r => r.Outcome == CommandOutcome.Accepted), Is.EqualTo(1));
            Assert.That(results.Count(r => r.ErrorCode == "STALE_REVISION"), Is.EqualTo(1));
            Assert.That(processor.InspectCommitted().World.GetLot(Source).OnHand, Is.EqualTo(6));
        }

        [Test]
        public void TEST_025_MetadataPrecommitFailureLeavesNoTerminalReceipt()
        {
            var fail = true;
            var processor = Processor(before: () => { if (fail) throw new InvalidOperationException("metadata failure"); });
            var before = processor.InspectCommitted(); var command = Command(quantity: 99);
            Assert.Throws<InvalidOperationException>(() => processor.Execute(command, Principal));
            Assert.That(processor.InspectCommitted(), Is.SameAs(before));
            fail = false;
            Assert.That(processor.Execute(command, Principal).ErrorCode, Is.EqualTo("INSUFFICIENT_RESOURCE"));
            Assert.That(processor.InspectCommitted().MetadataReceipts.Count, Is.EqualTo(1));
        }

        [Test]
        public void TEST_026_AllTerminalReceiptsSurviveLaterRevisionAndRngRemainsUntouched()
        {
            var processor = Processor();
            var rejectedCommand = Command(id: "reject", quantity: 99);
            var noopCommand = Command(id: "noop", payload: Payload(same: true));
            var acceptedCommand = Command();
            var rejected = processor.Execute(rejectedCommand, Principal);
            var noop = processor.Execute(noopCommand, Principal);
            var accepted = processor.Execute(acceptedCommand, Principal);
            processor.Execute(Command(id: "later", revision: 2, quantity: 1), Principal);
            var after = processor.InspectCommitted();
            Assert.That(after.World.Revision.Value, Is.EqualTo(3UL));
            Assert.That(processor.Execute(rejectedCommand, Principal), Is.SameAs(rejected));
            Assert.That(processor.Execute(noopCommand, Principal), Is.SameAs(noop));
            Assert.That(processor.Execute(acceptedCommand, Principal), Is.SameAs(accepted));
            Assert.That(processor.InspectCommitted(), Is.SameAs(after));
            Assert.That(after.World.RngStates.Single().State.DrawIndex, Is.EqualTo(7UL));
            Assert.That(after.Events.Select(e => e.Id.Value.Counter), Is.EqualTo(new ulong[] { 1, 2 }));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TEST_025_HouseholdOwnershipRequiresExplicitControl(bool controls)
        {
            var initial = World(); var stage = initial.Identity.BeginStage(2); var household = stage.Allocate<Household>();
            var world = new WorldSnapshot(stage.PrepareSnapshot(), initial.Tick,
                new[] { new ActorState(ActorId,true,Location, controls ? new[] { household.Value } : Array.Empty<EntityId>()) },
                initial.Lots.Select(l => new ResourceLot(l.Id,l.Commodity,household.Value,l.Location,l.OnHand,l.Reserved)), initial.RngStates);
            var processor = Processor(world); var result = processor.Execute(Command(revision: 2),Principal);
            Assert.That(result.Outcome, Is.EqualTo(controls ? CommandOutcome.Accepted : CommandOutcome.Rejected));
            Assert.That(result.ErrorCode, Is.EqualTo(controls ? null : "NOT_AUTHORIZED"));
            Assert.That(processor.InspectCommitted().World.Revision.Value, Is.EqualTo(controls ? 3UL : 2UL));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void FND_08_MaliciousHandlerCannotBypassDeadActorOrUncontrolledOwner(bool dead)
        {
            var initial = World(); var stage = initial.Identity.BeginStage(2); var household = stage.Allocate<Household>();
            var world = new WorldSnapshot(stage.PrepareSnapshot(), initial.Tick, new[] { new ActorState(ActorId,!dead,Location) },
                initial.Lots.Select(l => new ResourceLot(l.Id,l.Commodity,dead ? ActorId.Value : household.Value,l.Location,l.OnHand,l.Reserved)), initial.RngStates);
            var handler = new ProposalRule(_ => new ChangeSet(new[] { new LotChange(Source,10,6),new LotChange(Destination,3,7) }));
            var processor = Processor(world,handler: handler); var before = processor.InspectCommitted();
            Assert.That(Assert.Throws<PipelineFailureException>(() => processor.Execute(Command(revision: 2),Principal))!.Code,
                Is.EqualTo("PROPOSAL_PRECONDITION"));
            Assert.That(processor.InspectCommitted(),Is.SameAs(before));
        }

        [Test]
        public void TEST_026_AnotherAuthenticatedActorCannotRetrieveReceiptByCollidingId()
        {
            var processor = Processor(); var committed = processor.Execute(Command(),Principal);
            var other = new EntityId<Actor>(new EntityId(1,"actor",2));
            var command = new CommandEnvelope("command.1","TransferResource",other.ToString(),"Player",50,1,1,Payload(),"correlation.1");
            var result = processor.Execute(command,new ExecutionPrincipal(other,CommandSource.Player));
            Assert.That(result.ErrorCode, Is.EqualTo("CMD_PAYLOAD_MISMATCH"));
            Assert.That(result.OrderedEventIds, Is.Empty);
            Assert.That(processor.InspectCommitted().AcceptedReceipts.Single(), Is.SameAs(committed));
        }

        [Test]
        public void TEST_025_MalformedCanonicalPayloadKeyDoesNotPoisonReceipt()
        {
            var payload = Payload(); payload.Add("Invalid-Key",1);
            var processor = Processor(); var before = processor.InspectCommitted();
            Assert.That(processor.Execute(Command(payload: payload),Principal).ErrorCode, Is.EqualTo("INVALID_ENVELOPE"));
            Assert.That(processor.InspectCommitted(), Is.SameAs(before));
            Assert.That(processor.Execute(Command(),Principal).Outcome,Is.EqualTo(CommandOutcome.Accepted));
        }

        private sealed class ProposalRule : IRuleHandler
        {
            private readonly Func<WorldSnapshot, ChangeSet> build;
            public string CommandType => "TransferResource";
            public int PayloadVersion => 1;
            public Type PayloadType => typeof(TransferResource);
            public ProposalRule(Func<WorldSnapshot, ChangeSet> build) { this.build = build; }
            public RuleResult Evaluate(WorldSnapshot world, EntityId<Actor> issuer, IKernelCommand command) => RuleResult.Propose(build(world));
        }
        [TestCase("owner")]
        [TestCase("field")]
        [TestCase("expected")]
        [TestCase("negative")]
        [TestCase("reserved")]
        [TestCase("conservation")]
        [TestCase("duplicate")]
        [TestCase("missing")]
        [TestCase("reference")]
        [TestCase("empty")]
        public void FND_08_IndependentValidationRejectsMaliciousProposalWithoutTerminalReceipt(string flaw)
        {
            var handler = new ProposalRule(_ => {
                if (flaw == "empty") return new ChangeSet(Array.Empty<LotChange>());
                var source = new LotChange(Source, flaw == "expected" ? 9 : 10,
                    flaw == "negative" ? -1 : flaw == "reserved" ? 1 : 6,
                    flaw == "owner" ? "Economy" : "Assets", flaw == "field" ? "lots.reservations" : "lots.on_hand");
                if (flaw == "missing") return new ChangeSet(new[] { source });
                var target = new LotChange(flaw == "duplicate" ? Source : flaw == "reference" ? new EntityId<Lot>(new EntityId(1,"lot",999)) : Destination,
                    3, flaw == "conservation" ? 8 : 7);
                return new ChangeSet(new[] { source, target });
            });
            var processor = Processor(handler: handler); var before = processor.InspectCommitted();
            Assert.Throws<PipelineFailureException>(() => processor.Execute(Command(), Principal));
            Assert.That(processor.InspectCommitted(), Is.SameAs(before));
            Assert.That(before.World.Identity.Cursor("event").NextCounter, Is.EqualTo(1UL));
        }

        [Test]
        public void FND_08_NumericOverflowIsGameplayRejectionButBadHandlerOverflowIsSystemFailure()
        {
            var processor = Processor(World(target: long.MaxValue));
            Assert.That(processor.Execute(Command(), Principal).ErrorCode, Is.EqualTo("NUMERIC_OVERFLOW"));
            Assert.That(processor.InspectCommitted().World.GetLot(Destination).OnHand, Is.EqualTo(long.MaxValue));
            var evil = Processor(World(target: long.MaxValue), handler: new ProposalRule(_ => new ChangeSet(new[] {
                new LotChange(Source,10,6), new LotChange(Destination,long.MaxValue,long.MaxValue) })));
            Assert.That(Assert.Throws<PipelineFailureException>(() => evil.Execute(Command(), Principal))!.Code, Is.EqualTo("PROPOSAL_OVERFLOW"));
            Assert.That(evil.InspectCommitted().MetadataReceipts, Is.Empty);
        }

        [Test]
        public void FND_08_RevisionExhaustionDoesNotWrapOrPublish()
        {
            var processor = Processor(World(revision: ulong.MaxValue)); var before = processor.InspectCommitted();
            Assert.That(Assert.Throws<PipelineFailureException>(() => processor.Execute(Command(revision: ulong.MaxValue), Principal))!.Code,
                Is.EqualTo("REVISION_EXHAUSTED"));
            Assert.That(processor.InspectCommitted(), Is.SameAs(before));
        }

        [Test]
        public void FND_08_ReentrantExecutionCannotCommitNestedCommand()
        {
            CommandProcessor? processor = null;
            processor = Processor(before: () => processor!.Execute(Command(id: "nested"), Principal));
            var before = processor.InspectCommitted();
            Assert.That(Assert.Throws<PipelineFailureException>(() => processor.Execute(Command(), Principal))!.Code, Is.EqualTo("REENTRANT_COMMAND"));
            Assert.That(processor.InspectCommitted(), Is.SameAs(before));
        }

        [Test]
        public void TEST_025_MetadataResponseLossReturnsStoredRejectionOnRetry()
        {
            var processor = Processor(after: () => throw new InvalidOperationException("response lost"));
            var command = Command(quantity: 99);
            Assert.Throws<InvalidOperationException>(() => processor.Execute(command, Principal));
            var committed = processor.InspectCommitted();
            Assert.That(processor.Execute(command, Principal), Is.SameAs(committed.MetadataReceipts.Single()));
            Assert.That(committed.World.Revision.Value, Is.EqualTo(1UL));
        }
    }
}
