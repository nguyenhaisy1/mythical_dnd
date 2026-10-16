using System;
using System.Collections.Generic;
using System.Linq;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Foundation.Time;
using Game.Foundation.World;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class WorldRuleTests
    {
        private sealed class Fixture
        {
            public readonly EntityId<Actor> Actor;
            public readonly EntityId<Household> Household;
            public readonly EntityId<Settlement> Location, OtherLocation;
            public readonly EntityId<Lot> Source, Destination;
            public readonly IdentitySnapshot Identity;
            public Fixture()
            {
                var stage = IdentitySnapshot.Empty(1).BeginStage(1);
                Actor = stage.Allocate<Actor>(); Household = stage.Allocate<Household>();
                Location = stage.Allocate<Settlement>(); OtherLocation = stage.Allocate<Settlement>();
                Source = stage.Allocate<Lot>(); Destination = stage.Allocate<Lot>(); Identity = stage.PrepareSnapshot();
            }
            public WorldSnapshot World(long source = 20, long destination = 3, long reserved = 5, bool alive = true,
                bool control = true, bool otherCommodity = false, bool otherLocation = false)
            {
                var actor = new ActorState(Actor, alive, Location, control ? new[] { Household.Value } : Array.Empty<EntityId>());
                return new WorldSnapshot(Identity, new WorldTick(4), new[] { actor }, new[] {
                    new ResourceLot(Source, DefinitionId.Parse("commodity.timber@1"), Household.Value, Location, source, reserved),
                    new ResourceLot(Destination, DefinitionId.Parse(otherCommodity ? "commodity.rice@1" : "commodity.timber@1"), Actor.Value,
                        otherLocation ? OtherLocation : Location, destination, 0) });
            }
        }
        private static RuleResult Transfer(Fixture f, WorldSnapshot world, long quantity = 4) =>
            new RuleRegistry(new[] { new TransferResourceRule() }).Evaluate(world, f.Actor, new TransferResource(f.Source, f.Destination, quantity));

        [Test]
        public void FND_05_RevisionMonotonicAndExhaustion()
        {
            Assert.That(new WorldRevision(0).Next().Value, Is.EqualTo(1UL));
            Assert.That(new WorldRevision(8), Is.EqualTo(new WorldRevision(8)));
            Assert.That(new WorldRevision(8).CompareTo(new WorldRevision(9)), Is.LessThan(0));
            Assert.Throws<OverflowException>(() => new WorldRevision(ulong.MaxValue).Next());
        }
        [Test]
        public void FND_05_SnapshotCopiesCollectionsAndSortsWithoutWritableLeaks()
        {
            var f = new Fixture(); var initial = f.World();
            var actors = initial.Actors.ToList(); var lots = initial.Lots.Reverse().ToList();
            var world = new WorldSnapshot(f.Identity, initial.Tick, actors, lots);
            actors.Clear(); lots.Clear();
            Assert.That(world.Actors.Count, Is.EqualTo(1)); Assert.That(world.Lots.Count, Is.EqualTo(2));
            Assert.That(world.Lots.Select(x => x.Id), Is.EqualTo(new[] { f.Source, f.Destination }));
            Assert.Throws<NotSupportedException>(() => ((IList<ResourceLot>)world.Lots).Clear());
            Assert.Throws<NotSupportedException>(() => ((IList<EntityId>)world.Actors[0].ControlledOwnerIds).Clear());
            Assert.That(world.Revision.Value, Is.EqualTo(f.Identity.Revision));
        }
        [Test]
        public void FND_05_SnapshotRejectsMissingCrossWorldAndRetiredReferences()
        {
            var f = new Fixture(); var world = f.World();
            Assert.Throws<IdentityException>(() => world.GetActor(new EntityId<Actor>(new EntityId(2, "actor", 1))));
            Assert.Throws<IdentityException>(() => world.GetLot(new EntityId<Lot>(new EntityId(1, "lot", 9))));
            var stage = f.Identity.BeginStage(2); stage.Retire(f.Source);
            Assert.Throws<IdentityException>(() => new WorldSnapshot(stage.PrepareSnapshot(), world.Tick, world.Actors, world.Lots));
            Assert.Throws<ArgumentException>(() => new WorldSnapshot(f.Identity, world.Tick, world.Actors.Concat(world.Actors), world.Lots));
            Assert.Throws<IdentityException>(() => new WorldSnapshot(f.Identity, world.Tick, Array.Empty<ActorState>(), world.Lots));
        }
        [TestCase(-1, 0)]
        [TestCase(10, -1)]
        [TestCase(10, 11)]
        public void FND_05_InvalidQuantitiesCannotEnterSnapshot(long onHand, long reserved)
        {
            var f = new Fixture();
            Assert.Throws<ArgumentOutOfRangeException>(() => new ResourceLot(f.Source, DefinitionId.Parse("commodity.timber@1"), f.Actor.Value, f.Location, onHand, reserved));
        }
        [Test]
        public void FND_05_RngCursorAndIdentitySurviveSnapshotCopy()
        {
            var f = new Fixture(); var world = f.World();
            var state = new RngState(new RngContext(new string('0', 64), "assets", "transfer", "test", f.Actor.Value, "main"), 9);
            var streams = new List<RngStreamState> { new RngStreamState("b", state), new RngStreamState("a", state) };
            var snapshot = new WorldSnapshot(f.Identity, world.Tick, world.Actors, world.Lots, streams);
            streams.Clear();
            Assert.That(snapshot.RngStates.Select(x => x.ContextKey), Is.EqualTo(new[] { "a", "b" }));
            Assert.That(snapshot.RngStates[0].State.DrawIndex, Is.EqualTo(9UL));
            Assert.That(snapshot.Identity.Cursors, Is.EqualTo(f.Identity.Cursors));
            Assert.Throws<NotSupportedException>(() => ((IList<RngStreamState>)snapshot.RngStates).Clear());
        }
        [Test]
        public void FND_06_TransferReturnsOwnedProposalsWithoutMutatingWorld()
        {
            var f = new Fixture(); var world = f.World(); var result = Transfer(f, world);
            Assert.That(result.ErrorCode, Is.Null); Assert.That(result.IsNoOp, Is.False);
            Assert.That(result.Changes!.LotChanges.Select(x => x.NewOnHand), Is.EqualTo(new long[] { 16, 7 }));
            Assert.That(result.Changes.LotChanges.All(x => x.Owner == "Assets" && x.FieldPath == "lots.on_hand"), Is.True);
            Assert.That(world.GetLot(f.Source).OnHand, Is.EqualTo(20));
            Assert.That(world.GetLot(f.Destination).OnHand, Is.EqualTo(3));
            Assert.That(world.Revision.Value, Is.EqualTo(1UL));
            Assert.Throws<NotSupportedException>(() => ((IList<LotChange>)result.Changes.LotChanges).Clear());
        }
        [TestCase(0)]
        [TestCase(-1)]
        public void FND_06_InvalidQuantityRejected(long quantity)
        {
            var f = new Fixture(); Assert.That(Transfer(f, f.World(), quantity).ErrorCode, Is.EqualTo("INVALID_PAYLOAD"));
        }
        [Test]
        public void FND_06_HardConstraintsRejectBeforeProposal()
        {
            var f = new Fixture();
            Assert.That(Transfer(f, f.World(alive: false)).ErrorCode, Is.EqualTo("ACTOR_DEAD"));
            Assert.That(Transfer(f, f.World(control: false)).ErrorCode, Is.EqualTo("NOT_AUTHORIZED"));
            Assert.That(Transfer(f, f.World(otherCommodity: true)).ErrorCode, Is.EqualTo("COMMODITY_MISMATCH"));
            Assert.That(Transfer(f, f.World(otherLocation: true)).ErrorCode, Is.EqualTo("LOCATION_MISMATCH"));
            Assert.That(Transfer(f, f.World(), 16).ErrorCode, Is.EqualTo("INSUFFICIENT_RESOURCE"));
            Assert.That(Transfer(f, f.World(destination: long.MaxValue)).ErrorCode, Is.EqualTo("NUMERIC_OVERFLOW"));
        }
        [Test]
        public void FND_06_SameLotNoOpStillValidatesAvailableAndAuthority()
        {
            var f = new Fixture(); var registry = new RuleRegistry(new[] { new TransferResourceRule() });
            var cmd = new TransferResource(f.Source, f.Source, 1);
            Assert.That(registry.Evaluate(f.World(), f.Actor, cmd).IsNoOp, Is.True);
            Assert.That(registry.Evaluate(f.World(control: false), f.Actor, cmd).ErrorCode, Is.EqualTo("NOT_AUTHORIZED"));
            Assert.That(registry.Evaluate(f.World(source: 5), f.Actor, cmd).ErrorCode, Is.EqualTo("INSUFFICIENT_RESOURCE"));
        }
        private sealed class UnknownCommand : IKernelCommand
        {
            public string CommandType { get; }
            public int PayloadVersion { get; }
            public UnknownCommand(string type, int version) { CommandType = type; PayloadVersion = version; }
        }
        [Test]
        public void FND_06_RegistryRejectsUnknownVersionTypeSpoofAndDuplicates()
        {
            var f = new Fixture(); var world = f.World(); var handler = new TransferResourceRule();
            var registry = new RuleRegistry(new[] { handler });
            Assert.Throws<ArgumentException>(() => new RuleRegistry(new[] { handler, handler }));
            Assert.That(registry.Evaluate(world, f.Actor, new UnknownCommand("arbitrary.script", 1)).ErrorCode, Is.EqualTo("COMMAND_UNKNOWN"));
            Assert.That(registry.Evaluate(world, f.Actor, new UnknownCommand("TransferResource", 2)).ErrorCode, Is.EqualTo("PAYLOAD_VERSION_UNSUPPORTED"));
            Assert.That(registry.Evaluate(world, f.Actor, new UnknownCommand("TransferResource", 1)).ErrorCode, Is.EqualTo("INVALID_PAYLOAD"));
        }
    }
}
