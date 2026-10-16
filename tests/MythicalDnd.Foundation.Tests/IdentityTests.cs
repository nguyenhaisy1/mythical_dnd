using System;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text.Json;
using Game.Foundation.Identity;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class IdentityTests
    {
        private const string ActorOne = "w0000000000000001/actor/0000000000000001";
        private static IdentitySnapshot At(ulong next) => IdentitySnapshot.Restore(1, 0,
            EntityKinds.All.Select(k => new AllocationCursor(k, k == "actor" ? next : 1)), Array.Empty<RegistryEntry>());
        private static void Error(string code, TestDelegate action) => Assert.That(Assert.Throws<IdentityException>(action)!.Code, Is.EqualTo(code));

        [TestCase(ActorOne)]
        [TestCase("wffffffffffffffff/thread/ffffffffffffffff")]
        [TestCase("w000000000000000a/lot/00000000000000ff")]
        public void TEST_027_CanonicalWireRoundTrip(string wire) => Assert.That(EntityId.Parse(wire).ToString(), Is.EqualTo(wire));

        [TestCase("w0000000000000000/actor/0000000000000001")]
        [TestCase("w0000000000000001/actor/0000000000000000")]
        [TestCase("w0000000000000001/Actor/0000000000000001")]
        [TestCase("w0000000000000001/actor/1")]
        [TestCase("w000000000000000A/actor/0000000000000001")]
        [TestCase("w0000000000000001/unknown/0000000000000001")]
        [TestCase(ActorOne + "\n")]
        [TestCase(" " + ActorOne)]
        [TestCase("w0000000000000001%2factor/0000000000000001")]
        [TestCase("")]
        public void TEST_027_RejectNoncanonicalWire(string wire) => Assert.Throws<FormatException>(() => EntityId.Parse(wire));

        [Test]
        public void TEST_027_TypedEqualityAndReferenceErrors()
        {
            var id = EntityId<Actor>.Parse(ActorOne);
            Assert.That(id, Is.EqualTo(EntityId<Actor>.Parse(ActorOne)));
            Assert.That(id.GetHashCode(), Is.EqualTo(EntityId<Actor>.Parse(ActorOne).GetHashCode()));
            Error("REF_TYPE_MISMATCH", () => EntityId<Lot>.Parse(ActorOne));
            var snapshot = IdentitySnapshot.Empty(1);
            Error("REF_NOT_FOUND", () => snapshot.Resolve<Actor>(id.Value));
            Error("REF_TYPE_MISMATCH", () => snapshot.Resolve<Lot>(id.Value));
            Error("REF_WORLD_MISMATCH", () => snapshot.Resolve<Actor>(new EntityId(2, "actor", 1)));
            Error("REF_TYPE_MISMATCH", () => EntityKinds.For<string>());
        }

        [TestCase("commodity.timber@1")]
        [TestCase("kind.a-b2@999999999999999999999999999")]
        public void TEST_027_DefinitionRoundTrip(string wire)
        {
            var value = DefinitionId.Parse(wire);
            Assert.That(value.ToString(), Is.EqualTo(wire));
            Assert.That(value, Is.EqualTo(DefinitionId.Parse(wire)));
            Assert.That(value.Equals(EntityId.Parse(ActorOne)), Is.False);
        }
        [TestCase("commodity.timber@01")]
        [TestCase("commodity.timber@0")]
        [TestCase("Commodity.timber@1")]
        [TestCase("commodity.timber@1\n")]
        [TestCase(ActorOne)]
        [TestCase("")]
        public void TEST_027_InvalidDefinition(string wire) => Assert.Throws<FormatException>(() => DefinitionId.Parse(wire));

        [Test]
        public void TEST_027_OrderingIsWorldThenAsciiKindThenCounter()
        {
            var original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
                var ids = new[] { new EntityId(2, "actor", 1), new EntityId(1, "lot", 1), new EntityId(1, "actor", 10), new EntityId(1, "actor", 2) };
                Assert.That(ids.OrderBy(x => x).Select(x => x.ToString()), Is.EqualTo(new[] {
                    "w0000000000000001/actor/0000000000000002", "w0000000000000001/actor/000000000000000a",
                    "w0000000000000001/lot/0000000000000001", "w0000000000000002/actor/0000000000000001" }));
            }
            finally { CultureInfo.CurrentCulture = original; }
        }

        [Test]
        public void TEST_027_AllocationAbortRetryAndRestore()
        {
            var initial = At(7);
            var aborted = initial.BeginStage(1);
            var provisional = aborted.Allocate<Actor>();
            Assert.That(provisional.Value.Counter, Is.EqualTo(7UL));
            Error("REF_NOT_FOUND", () => initial.Resolve<Actor>(provisional.Value));
            aborted.Abort();
            Assert.Throws<InvalidOperationException>(() => aborted.PrepareSnapshot());
            Assert.That(initial.Cursor("actor").NextCounter, Is.EqualTo(7UL));
            var retry = initial.BeginStage(1);
            Assert.That(retry.Allocate<Actor>(), Is.EqualTo(provisional));
            var committed = retry.PrepareSnapshot();
            Assert.That(committed.Resolve<Actor>(provisional.Value).CreationRevision, Is.EqualTo(1UL));
            Assert.That(committed.Cursor("actor").NextCounter, Is.EqualTo(8UL));
            Assert.That(committed.Cursor("item").NextCounter, Is.EqualTo(1UL));
            Assert.Throws<InvalidOperationException>(() => retry.Allocate<Actor>());
            var restored = IdentitySnapshot.Restore(committed.World, committed.Revision, committed.Cursors, committed.Entries);
            Assert.That(restored.BeginStage(2).Allocate<Actor>().Value.Counter, Is.EqualTo(8UL));
            Assert.That(initial.Entries, Is.Empty);
        }

        [Test]
        public void TEST_027_ExhaustionDoesNotWrapAndRestores()
        {
            var source = At(ulong.MaxValue);
            var stage = source.BeginStage(1);
            var last = stage.Allocate<Actor>();
            Assert.That(last.Value.Counter, Is.EqualTo(ulong.MaxValue));
            Error("ID_EXHAUSTED", () => stage.Allocate<Actor>());
            Assert.That(stage.Allocate<Item>().Value.Counter, Is.EqualTo(1UL));
            var committed = stage.PrepareSnapshot();
            var restored = IdentitySnapshot.Restore(1, 1, committed.Cursors, committed.Entries);
            Error("ID_EXHAUSTED", () => restored.BeginStage(2).Allocate<Actor>());
            Assert.That(source.BeginStage(1).Allocate<Actor>(), Is.EqualTo(last));
        }

        [Test]
        public void TEST_027_RetirementTombstoneNeverReuseOrDeleteHistory()
        {
            var creation = At(7).BeginStage(1);
            var actor = creation.Allocate<Actor>();
            var active = creation.PrepareSnapshot();
            var retirement = active.BeginStage(2);
            Error("INVALID_LIFECYCLE_TRANSITION", () => retirement.Tombstone(actor, true));
            retirement.Retire(actor);
            var retired = retirement.PrepareSnapshot();
            var removal = retired.BeginStage(3);
            Error("INVALID_LIFECYCLE_TRANSITION", () => removal.Tombstone(actor, false));
            removal.Tombstone(actor, true);
            var tombstone = removal.PrepareSnapshot();
            var record = tombstone.Resolve<Actor>(actor.Value);
            Assert.That(record.Lifecycle, Is.EqualTo(EntityLifecycle.Tombstone));
            Assert.That(record.CreationRevision, Is.EqualTo(1UL));
            Assert.That(record.RetirementRevision, Is.EqualTo(2UL));
            var next = tombstone.BeginStage(4);
            Error("INVALID_LIFECYCLE_TRANSITION", () => next.Retire(actor));
            Assert.That(next.Allocate<Actor>().Value.Counter, Is.EqualTo(8UL));
            Assert.That(active.Resolve<Actor>(actor.Value).Lifecycle, Is.EqualTo(EntityLifecycle.Active));
        }

        [Test]
        public void TEST_027_RestoreRejectsInconsistentState()
        {
            var empty = IdentitySnapshot.Empty(1);
            Assert.Throws<ArgumentException>(() => IdentitySnapshot.Restore(1, 0, empty.Cursors.Skip(1), empty.Entries));
            Assert.Throws<ArgumentException>(() => IdentitySnapshot.Restore(1, 0, empty.Cursors.Concat(empty.Cursors.Take(1)), empty.Entries));
            var entry = new RegistryEntry(EntityId.Parse(ActorOne), EntityLifecycle.Active, 1);
            Assert.Throws<ArgumentException>(() => IdentitySnapshot.Restore(1, 1, empty.Cursors, new[] { entry }));
            Assert.Throws<ArgumentException>(() => IdentitySnapshot.Restore(1, 0, At(2).Cursors, new[] { entry }));
            Assert.Throws<ArgumentException>(() => IdentitySnapshot.Restore(2, 1, At(2).Cursors, new[] { entry }));
            Assert.Throws<ArgumentException>(() => IdentitySnapshot.Restore(1, 1, At(2).Cursors, new[] { entry, entry }));
            Assert.Throws<ArgumentException>(() => new AllocationCursor("actor", 3, true));
            Assert.Throws<ArgumentException>(() => new RegistryEntry(entry.Id, EntityLifecycle.Tombstone, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => empty.BeginStage(0));
        }

        [Test]
        public void TEST_027_AbortedStagesAcrossSequenceMatchUninterruptedRun()
        {
            var current = IdentitySnapshot.Empty(1);
            for (ulong i = 1; i <= 100; i++)
            {
                var discarded = current.BeginStage(i);
                discarded.Allocate<Actor>(); discarded.Allocate<Actor>(); discarded.Abort();
                var actual = current.BeginStage(i);
                Assert.That(actual.Allocate<Actor>().Value.Counter, Is.EqualTo(i));
                current = actual.PrepareSnapshot();
                current = IdentitySnapshot.Restore(1, i, current.Cursors.Reverse(), current.Entries.Reverse());
            }
            Assert.That(current.Entries.Count, Is.EqualTo(100));
            Assert.That(current.Cursor("actor").NextCounter, Is.EqualTo(101UL));
        }

        [Test]
        public void TEST_027_KernelSmokeEntityIdsRoundTrip()
        {
            using var json = JsonDocument.Parse(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "scenario.json")));
            var ids = json.RootElement.GetProperty("entities").EnumerateObject().ToArray();
            Assert.That(ids.Length, Is.EqualTo(7));
            foreach (var property in ids)
            {
                var wire = property.Value.GetString()!;
                Assert.That(EntityId.Parse(wire).ToString(), Is.EqualTo(wire));
            }
        }

        [Test]
        public void TEST_027_AllKindsHaveIndependentCountersAndWorlds()
        {
            var first = IdentitySnapshot.Empty(1).BeginStage(1);
            var ids = new[] { first.Allocate<Actor>().Value, first.Allocate<Household>().Value,
                first.Allocate<Item>().Value, first.Allocate<Lot>().Value, first.Allocate<Activity>().Value,
                first.Allocate<Job>().Value, first.Allocate<Event>().Value, first.Allocate<Route>().Value,
                first.Allocate<Settlement>().Value, first.Allocate<StoryThread>().Value };
            Assert.That(ids.Select(x => x.Kind).OrderBy(x => x, StringComparer.Ordinal), Is.EqualTo(EntityKinds.All));
            Assert.That(ids.All(x => x.Counter == 1), Is.True);
            Assert.That(first.PrepareSnapshot().Entries.Count, Is.EqualTo(10));
            var other = IdentitySnapshot.Empty(2).BeginStage(1).Allocate<Actor>();
            Assert.That(other.Value.Counter, Is.EqualTo(1UL));
            Assert.That(other.Value, Is.Not.EqualTo(ids[0]));
        }

        [Test]
        public void TEST_027_LifecycleAbortAndRevisionGuards()
        {
            var stage = IdentitySnapshot.Empty(1).BeginStage(1);
            var actor = stage.Allocate<Actor>();
            Error("REF_NOT_FOUND", () => stage.Retire(actor));
            var active = stage.PrepareSnapshot();
            var aborted = active.BeginStage(2);
            aborted.Retire(actor); aborted.Abort();
            Assert.That(active.Resolve<Actor>(actor.Value).Lifecycle, Is.EqualTo(EntityLifecycle.Active));
            Assert.Throws<ArgumentOutOfRangeException>(() => active.BeginStage(1));
            Assert.Throws<ArgumentOutOfRangeException>(() => IdentitySnapshot.Restore(1, ulong.MaxValue, active.Cursors, active.Entries).BeginStage(0));
            Assert.Throws<ArgumentException>(() => new RegistryEntry(actor.Value, EntityLifecycle.Active, 1, 2));
            Assert.Throws<ArgumentException>(() => new RegistryEntry(actor.Value, EntityLifecycle.Retired, 2, 1));
            Assert.Throws<ArgumentException>(() => new AllocationCursor("actor", 0));
        }

        [Test, Category("NegativeControl")]
        public void TEST_027_RunnerNegativeControl()
        {
            // Normal run asserts the contract; dedicated runner mode intentionally supplies a false expectation.
            var expected = Environment.GetEnvironmentVariable("MYTHICAL_NEGATIVE_CONTROL") == "1" ? 2UL : 1UL;
            Assert.That(IdentitySnapshot.Empty(1).BeginStage(1).Allocate<Actor>().Value.Counter, Is.EqualTo(expected));
        }
    }
}
