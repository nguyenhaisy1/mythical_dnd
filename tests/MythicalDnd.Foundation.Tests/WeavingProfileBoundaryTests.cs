using System;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Infrastructure.Diagnostics;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Replay;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    public sealed class WeavingProfileBoundaryTests
    {
        [Test]
        public void S17_04_ReplayArtifactV1RefusesWeavingImage4()
        {
            var state = WeavingTestFixture.Processor().InspectCommitted();
            Assert.Throws<ReplayCompatibilityException>(() => ReplayHarness.Record(state, Array.Empty<ReplayAction>(),
                new ReplayManifest(new string('d',64)), WeavingTestFixture.Rules()));
        }
        [Test]
        public void S17_04_ReplayArtifactV1RefusesWeavingRuleOnLegacyImage()
        {
            var work = WeavingTestFixture.World();
            var world = new WorldSnapshot(work.Identity, work.Tick, work.Actors, work.Lots, work.RngStates);
            var state = new CommandProcessor(world, WeavingTestFixture.Rules()).InspectCommitted();
            Assert.Throws<ReplayCompatibilityException>(() => ReplayHarness.Record(state, Array.Empty<ReplayAction>(),
                new ReplayManifest(new string('d',64), SqliteWorldStore.WeavingRuleVersion), WeavingTestFixture.Rules()));
        }
        [Test]
        public void S17_04_FullDeveloperStateDiffContainsWeavingFields()
        {
            var left = WeavingTestFixture.Processor(WeavingTestFixture.World(43)).InspectCommitted();
            var right = WeavingTestFixture.Processor(WeavingTestFixture.World(44)).InspectCommitted();
            Assert.That(StateDiffer.Compare(left,right).Entries, Has.Some.Matches<DiffEntry>(entry => entry.Path == "/weaving/skill"));
        }
    }
}
