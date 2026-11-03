using System.Linq;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Domain.Repair;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using NUnit.Framework;
using F = MythicalDnd.Foundation.Tests.BoatRepairTestFixture;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class BoatRepairIndependentTests
    {
        private static WorldSnapshot Running()
        {
            var registry = F.Registry();
            var processor = new CommandProcessor(F.Initial(registry), F.Rules(), repairRecipe: registry.Recipe);
            Assert.That(processor.Execute(F.Start(), F.Principal).Outcome, Is.EqualTo(CommandOutcome.Accepted));
            return processor.InspectCommitted().World;
        }

        // Trusted fixture changes only: this is revalidation evidence, not an implemented movement command.
        [Test]
        public void VS_03A_IndependentMovingAllInputsTogetherCannotMovePinnedWorksite()
        {
            var running = Running();
            var stage = running.Identity.BeginStage(running.Revision.Value + 1);
            var remote = stage.Allocate<Settlement>();
            var repair = running.Repair!;
            var changed = new WorldSnapshot(stage.PrepareSnapshot(), new WorldTick(60),
                running.Actors.Select(a => new ActorState(a.Id, a.Alive, remote, a.ControlledOwnerIds)),
                running.Lots.Select(l => new ResourceLot(l.Id, l.Commodity, l.Owner, remote, l.OnHand, l.Reserved)),
                running.RngStates,
                new RepairState(repair.Recipe,
                    repair.Items.Select(i => new RepairItemState(i.Id, i.Kind, i.Owner, remote, i.Condition, i.Destroyed)),
                    repair.Capabilities, repair.Activities));

            var evaluation = BoatRepairRules.EvaluateComplete(changed, F.Activity);

            Assert.That(evaluation.ErrorCode, Is.Null);
            var plan = evaluation.Plan!;
            Assert.That(plan.Activity.Status, Is.EqualTo(RepairActivityStatus.Cancelled));
            Assert.That(plan.Activity.TerminalReason, Is.EqualTo("LOCATION_MISMATCH"));
            Assert.That(plan.Activity.Worksite, Is.EqualTo(repair.GetActivity(F.Activity).Worksite));
            Assert.That(plan.Lots.Single(l => l.Id.Equals(F.Timber)).Reserved, Is.Zero);
            Assert.That(plan.Lots.Single(l => l.Id.Equals(F.Timber)).OnHand, Is.EqualTo(10));
            Assert.That(plan.Repair.GetItem(F.Boat).Condition, Is.EqualTo(500));
            Assert.That(running.GetLot(F.Timber).Reserved, Is.EqualTo(2));
        }

        // A valid replacement tool must not silently replace the tool reserved by this activity.
        [Test]
        public void VS_03A_IndependentUsableReplacementToolCannotCompleteOldActivity()
        {
            var running = Running();
            var stage = running.Identity.BeginStage(running.Revision.Value + 1);
            var replacement = stage.Allocate<Item>();
            var repair = running.Repair!;
            var originalTool = repair.GetItem(F.Tool);
            var changed = new WorldSnapshot(stage.PrepareSnapshot(), new WorldTick(60),
                running.Actors, running.Lots, running.RngStates,
                new RepairState(repair.Recipe,
                    repair.Items.Concat(new[] { new RepairItemState(replacement, RepairItemKind.RepairTool,
                        originalTool.Owner, originalTool.Location, 1000) }),
                    new[] { new RepairCapability(F.Actor, true, replacement) }, repair.Activities));

            var evaluation = BoatRepairRules.EvaluateComplete(changed, F.Activity);

            Assert.That(evaluation.ErrorCode, Is.Null);
            var plan = evaluation.Plan!;
            Assert.That(plan.Activity.Status, Is.EqualTo(RepairActivityStatus.Cancelled));
            Assert.That(plan.Activity.TerminalReason, Is.EqualTo("ITEM_UNAVAILABLE"));
            Assert.That(plan.Activity.ToolItemId, Is.EqualTo(F.Tool));
            Assert.That(plan.Lots.Single(l => l.Id.Equals(F.Timber)).Reserved, Is.Zero);
            Assert.That(plan.Lots.Single(l => l.Id.Equals(F.Timber)).OnHand, Is.EqualTo(10));
            Assert.That(plan.Repair.GetItem(F.Boat).Condition, Is.EqualTo(500));
            Assert.That(plan.Repair.GetItem(replacement).Condition, Is.EqualTo(1000));
            Assert.That(running.GetLot(F.Timber).Reserved, Is.EqualTo(2));
        }
    }
}
