using System;
using System.Collections.Generic;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Domain.Repair;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using NUnit.Framework;
using F = MythicalDnd.Foundation.Tests.BoatRepairTestFixture;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class BoatRepairCommandTests
    {
        private static CommandProcessor Processor(int condition = 500, long timber = 10)
        { var r = F.Registry(); return new CommandProcessor(F.Initial(r, condition, timber), F.Rules(), repairRecipe: r.Recipe); }
        private static void Finish(CommandProcessor p)
        {
            var w = p.InspectCommitted().World;
            Assert.That(p.Execute(F.Advance(w.Revision.Value, w.Tick.Value), F.Principal).Outcome, Is.EqualTo(CommandOutcome.AcceptedPending));
            Assert.That(p.DrainAdvance(F.Principal).Status, Is.EqualTo(AdvanceStatus.Completed));
        }
        [Test]
        public void VS_03A_StartReservesAtomicallyWithoutConsumingAndKeepsOriginalSnapshot()
        {
            var p = Processor(); var before = p.InspectCommitted(); var start = F.Start();
            var receipt = p.Execute(start, F.Principal); var after = p.InspectCommitted();
            Assert.That(receipt.Outcome, Is.EqualTo(CommandOutcome.Accepted));
            Assert.That(after.World.GetLot(F.Timber).OnHand, Is.EqualTo(10));
            Assert.That(after.World.GetLot(F.Timber).Reserved, Is.EqualTo(2));
            Assert.That(before.World.GetLot(F.Timber).Reserved, Is.Zero);
            Assert.That(before.World.Repair!.Activities, Is.Empty);
            var activity = after.World.Repair!.Activities.Single(); var job = after.Scheduler.Jobs.Single();
            Assert.That(activity.Status, Is.EqualTo(RepairActivityStatus.Running));
            Assert.That(activity.ReservedQuantity, Is.EqualTo(2));
            Assert.That(activity.DueTick.Value, Is.EqualTo(60));
            Assert.That(activity.Recipe.Fingerprint, Is.EqualTo(F.Registry().Recipe.Fingerprint));
            Assert.That(job.IsRepair, Is.True); Assert.That(job.RepairActivityId, Is.EqualTo(activity.Id));
            Assert.That(job.Phase, Is.EqualTo(SchedulerPhase.ProductionHealth));
            Assert.That(after.Journal.Events.Single().EventType, Is.EqualTo("RepairStarted"));
            Assert.That(p.Execute(start, F.Principal), Is.SameAs(receipt));
            Assert.That(p.InspectCommitted(), Is.SameAs(after));
        }
        [Test]
        public void VS_03A_BeforeDueDoesNotRepairAndDueCompletionConsumesExactlyOnce()
        {
            var p = Processor(); p.Execute(F.Start(), F.Principal);
            p.Execute(F.Advance(target: 59), F.Principal);
            var at59 = p.InspectCommitted();
            Assert.That(at59.World.Tick.Value, Is.EqualTo(59));
            Assert.That(at59.World.Repair!.GetItem(F.Boat).Condition, Is.EqualTo(500));
            Assert.That(at59.World.GetLot(F.Timber).OnHand, Is.EqualTo(10));
            var command = F.Advance(at59.World.Revision.Value, 59, 60, "due.advance");
            p.Execute(command, F.Principal); p.DrainAdvance(F.Principal);
            var done = p.InspectCommitted();
            Assert.That(done.World.Repair!.GetItem(F.Boat).Condition, Is.EqualTo(700));
            Assert.That(done.World.GetLot(F.Timber).OnHand, Is.EqualTo(8));
            Assert.That(done.World.GetLot(F.Timber).Reserved, Is.Zero);
            Assert.That(done.World.Identity.Resolve<Activity>(F.Activity.Value).Lifecycle, Is.EqualTo(EntityLifecycle.Retired));
            Assert.That(done.Journal.Events.Count(e => e.EventType == "RepairCompleted"), Is.EqualTo(1));
            p.Execute(command, F.Principal); p.DrainAdvance(F.Principal);
            Assert.That(p.InspectCommitted(), Is.SameAs(done));
        }
        [Test]
        public void VS_03A_CancelReleasesHoldWithoutCreatingWoodAndCannotResurrectTerminalActivity()
        {
            var p = Processor(); p.Execute(F.Start(), F.Principal);
            var cancel = F.Cancel(2); var receipt = p.Execute(cancel, F.Principal); var state = p.InspectCommitted();
            Assert.That(receipt.Outcome, Is.EqualTo(CommandOutcome.Accepted));
            Assert.That(state.World.GetLot(F.Timber).OnHand, Is.EqualTo(10));
            Assert.That(state.World.GetLot(F.Timber).Reserved, Is.Zero);
            Assert.That(state.World.Repair!.GetItem(F.Boat).Condition, Is.EqualTo(500));
            Assert.That(state.World.Repair.GetActivity(F.Activity).Status, Is.EqualTo(RepairActivityStatus.Cancelled));
            Assert.That(p.Execute(cancel, F.Principal), Is.SameAs(receipt));
            Assert.That(p.Execute(F.Cancel(3, id: "cancel.again"), F.Principal).ErrorCode, Is.EqualTo("ACTIVITY_STATE"));
            var w = p.InspectCommitted().World;
            p.Execute(F.Advance(w.Revision.Value, w.Tick.Value, id: "advance.cancelled"), F.Principal);
            Assert.That(p.InspectCommitted().World.Repair!.GetItem(F.Boat).Condition, Is.EqualTo(500));
            Assert.That(p.InspectCommitted().Journal.Events.Any(e => e.EventType == "RepairCompleted"), Is.False);
        }
        [Test]
        public void VS_03A_CancelJobCannotLeakHoldButCancelActivityCanRunDuringAdvance()
        {
            var p = Processor(); p.Execute(F.Start(), F.Principal); p.Execute(F.Advance(), F.Principal);
            var w = p.InspectCommitted().World; var job = p.InspectCommitted().Scheduler.Jobs.Single();
            var direct = F.Command("bad.cancel-job", "CancelJob", w.Revision.Value, w.Tick.Value,
                new Dictionary<string, object?> { ["job_id"] = job.Id.ToString() });
            Assert.That(p.Execute(direct, F.Principal).ErrorCode, Is.EqualTo("ACTIVITY_STATE"));
            Assert.That(p.InspectCommitted().World.GetLot(F.Timber).Reserved, Is.EqualTo(2));
            Assert.That(p.Execute(F.Cancel(w.Revision.Value), F.Principal).Outcome, Is.EqualTo(CommandOutcome.Accepted));
            Assert.That(p.DrainAdvance(F.Principal).Status, Is.EqualTo(AdvanceStatus.Completed));
            Assert.That(p.InspectCommitted().World.GetLot(F.Timber).OnHand, Is.EqualTo(10));
            Assert.That(p.InspectCommitted().World.GetLot(F.Timber).Reserved, Is.Zero);
            Assert.That(p.InspectCommitted().Scheduler.Executions, Is.Empty);
        }
        [Test]
        public void VS_03A_TransferRespectsRepairHoldAndPreservesRepairExtension()
        {
            var p = Processor(); p.Execute(F.Start(), F.Principal);
            var spare = EntityId<Lot>.Parse("w000000000000000e/lot/0000000000000002");
            CommandEnvelope Transfer(string id, long q) => F.Command(id, "TransferResource", p.InspectCommitted().World.Revision.Value, 0,
                new Dictionary<string, object?> { ["source_lot_id"] = F.Timber.ToString(), ["destination_lot_id"] = spare.ToString(), ["quantity"] = q });
            Assert.That(p.Execute(Transfer("over.available", 9), F.Principal).ErrorCode, Is.EqualTo("INSUFFICIENT_RESOURCE"));
            var repair = p.InspectCommitted().World.Repair;
            Assert.That(p.Execute(Transfer("allowed.transfer", 3), F.Principal).Outcome, Is.EqualTo(CommandOutcome.Accepted));
            Assert.That(p.InspectCommitted().World.Repair, Is.SameAs(repair));
            Finish(p); var after = p.InspectCommitted();
            Assert.That(after.World.GetLot(F.Timber).OnHand, Is.EqualTo(5));
            Assert.That(after.World.GetLot(spare).OnHand, Is.EqualTo(3));
            Assert.That(after.World.GetLot(F.Timber).Reserved, Is.Zero);
            Assert.That(after.World.Repair!.GetItem(F.Boat).Condition, Is.EqualTo(700));
        }
        [TestCase(950, 1000)] [TestCase(500, 700)]
        public void VS_03A_ConditionGainIsCapped(int initial, int expected)
        { var p = Processor(initial); p.Execute(F.Start(), F.Principal); Finish(p); Assert.That(p.InspectCommitted().World.Repair!.GetItem(F.Boat).Condition, Is.EqualTo(expected)); }
        [TestCase(1000, 10, "ITEM_UNAVAILABLE")] [TestCase(500, 1, "INSUFFICIENT_RESOURCE")]
        public void VS_03A_RejectedStartDoesNotAllocateActivityOrReserve(int condition, long wood, string error)
        {
            var p = Processor(condition, wood); var before = p.InspectCommitted().World;
            Assert.That(p.Execute(F.Start(), F.Principal).ErrorCode, Is.EqualTo(error));
            Assert.That(p.InspectCommitted().World, Is.SameAs(before));
            Assert.That(p.InspectCommitted().World.Repair!.Activities, Is.Empty);
        }
        [Test]
        public void VS_03A_ExternalCompletionAndPayloadOverridesAreRejected()
        {
            var p = Processor(); p.Execute(F.Start(), F.Principal);
            var forged = F.Command("forged.complete", "CompleteRepair", 2, 0,
                new Dictionary<string, object?> { ["activity_id"] = F.Activity.ToString() });
            Assert.That(p.Execute(forged, F.Principal).ErrorCode, Is.EqualTo("NOT_AUTHORIZED"));
            var overridePayload = F.Start().Payload.ToDictionary(x => x.Key, x => x.Value); overridePayload["quantity"] = 0L;
            Assert.That(p.Execute(F.Command("override.quantity", "StartRepair", 2, 0, overridePayload), F.Principal).ErrorCode, Is.EqualTo("INVALID_PAYLOAD"));
            Assert.That(p.Execute(F.Start(id: "stale.start"), F.Principal).ErrorCode, Is.EqualTo("STALE_REVISION"));
            var mismatch = F.Start().Payload.ToDictionary(x => x.Key, x => x.Value); mismatch["recipe_id"] = "recipe.other@1";
            Assert.That(p.Execute(F.Command("repair.start", "StartRepair", 1, 0, mismatch), F.Principal).ErrorCode, Is.EqualTo("CMD_PAYLOAD_MISMATCH"));
            Assert.That(p.InspectCommitted().World.GetLot(F.Timber).Reserved, Is.EqualTo(2));
        }
        [Test]
        public void VS_03A_PreCommitFailureKeepsAllStateAndRetryUsesFirstActivityId()
        {
            var r = F.Registry(); var fail = true;
            var p = new CommandProcessor(F.Initial(r), F.Rules(), beforeCommit: () => { if (fail) throw new InvalidOperationException("injected"); }, repairRecipe: r.Recipe);
            var before = p.InspectCommitted(); Assert.Throws<InvalidOperationException>(() => p.Execute(F.Start(), F.Principal));
            Assert.That(p.InspectCommitted(), Is.SameAs(before)); fail = false;
            p.Execute(F.Start(), F.Principal);
            Assert.That(p.InspectCommitted().World.Repair!.Activities.Single().Id, Is.EqualTo(F.Activity));
        }
        [Test]
        public void VS_03A_ResponseLossRetriesOriginalReceiptWithoutSecondReservation()
        {
            var r = F.Registry(); var p = new CommandProcessor(F.Initial(r), F.Rules(), afterCommit: () => throw new InvalidOperationException("response lost"), repairRecipe: r.Recipe);
            Assert.Throws<InvalidOperationException>(() => p.Execute(F.Start(), F.Principal));
            var state = p.InspectCommitted(); var receipt = p.Execute(F.Start(), F.Principal);
            Assert.That(receipt, Is.SameAs(state.AcceptedReceipts.Single()));
            Assert.That(p.InspectCommitted(), Is.SameAs(state)); Assert.That(state.World.GetLot(F.Timber).Reserved, Is.EqualTo(2));
        }
        [Test]
        public void VS_03A_DueTickOverflowDoesNotPublishOrSpendMaterial()
        {
            var r = F.Registry(); var start = F.Initial(r).World;
            var world = new WorldSnapshot(start.Identity, new WorldTick(long.MaxValue - 30), start.Actors, start.Lots, start.RngStates, start.Repair);
            var p = new CommandProcessor(world, F.Rules(), repairRecipe: r.Recipe);
            var command = F.Command("overflow.start", "StartRepair", 1, world.Tick.Value, F.Start().Payload.ToDictionary(x => x.Key, x => x.Value));
            Assert.That(p.Execute(command, F.Principal).ErrorCode, Is.EqualTo("TIME_RANGE"));
            Assert.That(p.InspectCommitted().World, Is.SameAs(world));
        }
    }
}
