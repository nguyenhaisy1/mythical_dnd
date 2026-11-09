using System;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Domain.Origins;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using Game.Infrastructure.Diagnostics;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Replay;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    public sealed class OriginProfileBoundaryTests
    {
        private static CommittedWorld Initial(string source = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")
        {
            var pick = OriginSelector.Select(new string('0', 64), source, new string('b', 64),
                new[] { new OriginCandidate("actor.vac-lam@1", "household.vac-tong@1", "community.ben-vac@1", true, 22) });
            var stage = IdentitySnapshot.Empty(pick.Origin.Principal.Value.World).BeginStage(1);
            var actor = stage.Allocate<Actor>(); _ = stage.Allocate<Household>(); var home = stage.Allocate<Settlement>();
            var world = new WorldSnapshot(stage.PrepareSnapshot(), new WorldTick(0), new[] { new ActorState(actor, true, home) },
                Array.Empty<ResourceLot>(), new[] { pick.Rng }, origin: pick.Origin);
            return new CommandProcessor(world, Rules()).InspectCommitted();
        }
        private static RuleRegistry Rules() => new RuleRegistry(new[] { new TransferResourceRule() });
        [Test]
        public void S16_04_LegacyReplayRecordRefusesOriginImage()
        {
            Assert.Throws<ReplayCompatibilityException>(() => ReplayHarness.Record(Initial(), Array.Empty<ReplayAction>(),
                new ReplayManifest(new string('b', 64)), Rules()));
        }
        [Test]
        public void S16_04_OriginReplayProfileRequiresFutureExplicitImplementation()
        {
            Assert.Throws<ReplayCompatibilityException>(() => ReplayHarness.Record(Initial(), Array.Empty<ReplayAction>(),
                new ReplayManifest(new string('b', 64), SqliteWorldStore.OriginRuleVersion), Rules()));
        }
        [Test]
        public void S16_04_OriginMetadataAppearsInFullDeveloperStateDiff()
        {
            var diff = StateDiffer.Compare(Initial(new string('a', 64)), Initial(new string('c', 64)));
            Assert.That(diff.Entries, Has.Some.Matches<DiffEntry>(e => e.Path == "/origin/source_hash"));
        }
    }
}
