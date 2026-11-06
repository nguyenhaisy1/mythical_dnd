using System;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Application.Projection;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using NUnit.Framework;
using F = MythicalDnd.Foundation.Tests.BoatRepairTestFixture;

namespace MythicalDnd.Foundation.Tests
{
    /// <summary>Independent regression: localized labels cannot grant direct observation.</summary>
    [TestFixture]
    public sealed class RepairClientBoundaryTests
    {
        private static (RepairClientSession Session, CommandProcessor Processor) Session(bool away)
        {
            var initial = F.Initial(); var world = initial.World;
            var worksite = world.GetActor(F.Actor).Location;
            if (away)
            {
                var stage = world.Identity.BeginStage(world.Revision.Value + 1);
                var otherSite = stage.Allocate<Settlement>();
                world = new WorldSnapshot(stage.PrepareSnapshot(), world.Tick,
                    world.Actors.Select(a => new ActorState(a.Id, a.Alive, otherSite, a.ControlledOwnerIds)),
                    world.Lots, world.RngStates, world.Repair);
            }
            var processor = new CommandProcessor(world, F.Rules(), repairRecipe: world.Repair!.Recipe);
            var state = processor.InspectCommitted();
            var scope = new RepairObservationScope(F.Principal, F.Boat, F.Timber, F.Tool,
                worksite, "Nhân vật thử nghiệm", "Chưa rõ", RepairObservationFields.All);
            var session = new RepairClientSession(processor, scope,
                loaded => new CommandProcessor(loaded, F.Rules(), repairRecipe: loaded.World.Repair!.Recipe),
                () => state, _ => { });
            return (session, processor);
        }

        [Test]
        public void S15_01_RecipeObservationRequiresWorksiteEvenWhenUnknownLabelMatches()
        {
            var h = Session(away: true); var before = h.Processor.InspectCommitted();
            var hash = F.Hash(before); var view = h.Session.Read(); var preview = h.Session.PreviewRepair();
            Assert.That(view.LocationLabel, Is.EqualTo("Chưa rõ"));
            Assert.That(view.Observation.Source, Is.EqualTo("unknown"));
            Assert.That(preview.RequirementsKnown, Is.False); Assert.That(preview.CanConfirm, Is.False);
            Assert.That(preview.Token, Is.Null); Assert.That(preview.Code, Is.EqualTo("not_observed"));
            Assert.That(preview.CostTimber, Is.Null); Assert.That(preview.DurationTicks, Is.Null);
            Assert.That(preview.ConditionGain, Is.Null); Assert.That(preview.ConditionCap, Is.Null);
            Assert.That(h.Processor.InspectCommitted(), Is.SameAs(before));
            Assert.That(F.Hash(h.Processor.InspectCommitted()), Is.EqualTo(hash));
        }

        [Test]
        public void S15_01_LocalizedLabelDoesNotDenyARealSameWorksiteObservation()
        {
            var h = Session(away: false); var view = h.Session.Read(); var preview = h.Session.PreviewRepair();
            Assert.That(view.LocationLabel, Is.EqualTo("Chưa rõ"));
            Assert.That(view.Observation.Source, Is.EqualTo("direct-worksite-fixture"));
            Assert.That(preview.RequirementsKnown, Is.True); Assert.That(preview.CanConfirm, Is.True);
            Assert.That(preview.CostTimber, Is.EqualTo(2)); Assert.That(preview.DurationTicks, Is.EqualTo(60));
        }
    }
}
