using System;
using System.Collections.Generic;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Domain.Weaving;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using NUnit.Framework;
using F = MythicalDnd.Foundation.Tests.WeavingTestFixture;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class WeavingRulesTests
    {
        [Test]
        public void S17_01_StartReservesThenDueConsumesOnceAndOriginRngStays()
        {
            var p = F.Processor(); var original = p.InspectCommitted(); var command = F.Start(p); var principal = F.Principal(original.World);
            var receipt = p.Execute(command, principal); var running = p.InspectCommitted(); var s = running.World.Weaving!;
            Assert.That(receipt.Outcome, Is.EqualTo(CommandOutcome.Accepted));
            Assert.That(original.World.GetLot(s.InputLotId).Reserved, Is.Zero);
            Assert.That(running.World.GetLot(s.InputLotId).OnHand, Is.EqualTo(8));
            Assert.That(running.World.GetLot(s.InputLotId).Reserved, Is.EqualTo(2));
            Assert.That(running.World.GetLot(s.OutputLotId).OnHand, Is.Zero);
            Assert.That(s.Activities.Single().DueTick.Value, Is.EqualTo(30));
            var job = running.Scheduler.Jobs.Single(); Assert.That(job.IsWeaving, Is.True); Assert.That(job.IsRepair, Is.False);
            Assert.Throws<InvalidOperationException>(() => _ = job.Transfer);
            Assert.That(job.WeavingActivityId, Is.EqualTo(s.Activities.Single().Id));
            Assert.That(job.Phase, Is.EqualTo(SchedulerPhase.ProductionHealth));
            Assert.That(p.Execute(command, principal), Is.SameAs(receipt));
            F.Advance(p, 29, "before.due"); var beforeDue = p.InspectCommitted().World;
            Assert.That(beforeDue.GetLot(s.InputLotId).OnHand, Is.EqualTo(8)); Assert.That(beforeDue.GetLot(s.OutputLotId).OnHand, Is.Zero);
            F.Advance(p, 30); var done = p.InspectCommitted();
            Assert.That(done.World.GetLot(s.InputLotId).OnHand, Is.EqualTo(6)); Assert.That(done.World.GetLot(s.InputLotId).Reserved, Is.Zero);
            Assert.That(done.World.GetLot(s.OutputLotId).OnHand, Is.EqualTo(1)); Assert.That(done.World.Weaving!.Activities.Single().Status, Is.EqualTo(WeavingActivityStatus.Completed));
            Assert.That(done.World.Origin, Is.SameAs(original.World.Origin)); Assert.That(done.World.RngStates.Single().State.DrawIndex, Is.EqualTo(1));
            Assert.That(done.Journal.Events.Count(e => e.EventType == "WeavingCompleted"), Is.EqualTo(1));
            Assert.That(done.World.Identity.Resolve<Activity>(s.Activities.Single().Id.Value).Lifecycle, Is.EqualTo(EntityLifecycle.Retired));
            Assert.That(p.Execute(command, principal), Is.SameAs(receipt));
            Assert.That(p.InspectCommitted(), Is.SameAs(done));
        }
        [TestCase(43)] [TestCase(61)] [TestCase(40)]
        public void S17_01_AuthoredQualifiedSkillsCanComplete(int skill)
        {
            var p = F.Processor(F.World(skill)); p.Execute(F.Start(p), F.Principal(p.InspectCommitted().World)); F.Advance(p, 30);
            Assert.That(p.InspectCommitted().World.GetLot(p.InspectCommitted().World.Weaving!.OutputLotId).OnHand, Is.EqualTo(1));
        }
        [TestCase(0)] [TestCase(39)]
        public void S17_01_NonWeaverDeniedWithoutAllocationOrRng(int skill)
        {
            var p = F.Processor(F.World(skill)); var before = p.InspectCommitted();
            var result = p.Execute(F.Start(p), F.Principal(before.World));
            Assert.That(result.ErrorCode, Is.EqualTo("NOT_AUTHORIZED")); Assert.That(p.InspectCommitted().World, Is.SameAs(before.World));
            Assert.That(p.InspectCommitted().World.Weaving!.Activities, Is.Empty); Assert.That(p.InspectCommitted().Scheduler.Jobs, Is.Empty);
        }
        [Test]
        public void S17_01_CancelReleasesOnlyHoldAndIsIdempotent()
        {
            var p = F.Processor(); var principal = F.Principal(p.InspectCommitted().World); p.Execute(F.Start(p), principal);
            var cancel = F.Cancel(p); var receipt = p.Execute(cancel, principal); var state = p.InspectCommitted(); var w = state.World.Weaving!;
            Assert.That(receipt.Outcome, Is.EqualTo(CommandOutcome.Accepted)); Assert.That(w.Activities.Single().Status, Is.EqualTo(WeavingActivityStatus.Cancelled));
            Assert.That(state.World.GetLot(w.InputLotId).OnHand, Is.EqualTo(8)); Assert.That(state.World.GetLot(w.InputLotId).Reserved, Is.Zero); Assert.That(state.World.GetLot(w.OutputLotId).OnHand, Is.Zero);
            Assert.That(p.Execute(cancel, principal), Is.SameAs(receipt)); Assert.That(p.Execute(F.Cancel(p, "cancel.again"), principal).ErrorCode, Is.EqualTo("ACTIVITY_STATE"));
            F.Advance(p, 60); Assert.That(p.InspectCommitted().World.GetLot(w.OutputLotId).OnHand, Is.Zero);
        }
        [Test]
        public void S17_01_CancelDuringAdvanceAllowedButDirectCancelJobDenied()
        {
            var p = F.Processor(); var principal = F.Principal(p.InspectCommitted().World); p.Execute(F.Start(p), principal);
            p.Execute(F.Command(p, "begin.advance", "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = 30L }), principal);
            var job = p.InspectCommitted().Scheduler.Jobs.Single();
            var direct = F.Command(p, "bad.canceljob", "CancelJob", new Dictionary<string, object?> { ["job_id"] = job.Id.ToString() });
            Assert.That(p.Execute(direct, principal).ErrorCode, Is.EqualTo("ACTIVITY_STATE"));
            Assert.That(p.Execute(F.Start(p, "busy.advance"), principal).ErrorCode, Is.EqualTo("ADVANCE_IN_PROGRESS"));
            Assert.That(p.Execute(F.Cancel(p), principal).Outcome, Is.EqualTo(CommandOutcome.Accepted));
            Assert.That(p.DrainAdvance(principal).Status, Is.EqualTo(AdvanceStatus.Completed)); Assert.That(p.InspectCommitted().Scheduler.Executions, Is.Empty);
        }
        [TestCase("CompleteWeaving")] [TestCase("ExecuteScheduledWeaving")]
        public void S17_01_ExternalCompletionCannotForgeSchedulerAuthority(string type)
        {
            var p = F.Processor(); var principal = F.Principal(p.InspectCommitted().World);
            var command = F.Command(p, "external.complete", type, new Dictionary<string, object?>());
            Assert.That(p.Execute(command, principal).ErrorCode, Is.EqualTo("NOT_AUTHORIZED"));
            var spoof = new CommandEnvelope("scheduler/job/forged", type, principal.ActorId.ToString(), "Scheduler", 0, 1, 1, new Dictionary<string, object?>(), "test");
            Assert.That(p.Execute(spoof, new ExecutionPrincipal(principal.ActorId, CommandSource.Scheduler)).ErrorCode, Is.EqualTo("INVALID_ENVELOPE"));
        }
        [Test]
        public void S17_01_ChangedRequestStaleAndPayloadOverrideRejected()
        {
            var p = F.Processor(); var principal = F.Principal(p.InspectCommitted().World); var start = F.Start(p);
            p.Execute(start, principal); Assert.That(p.Execute(F.Start(p, "busy"), principal).ErrorCode, Is.EqualTo("ITEM_UNAVAILABLE"));
            var changed = start.Payload.ToDictionary(x => x.Key, x => x.Value); changed["output_lot_id"] = "w0000000000000001/lot/0000000000000001";
            var spoof = new CommandEnvelope(start.CommandId, start.CommandType, start.IssuerId, start.SourceType, 0, 1, 1, changed, start.CorrelationId);
            Assert.That(p.Execute(spoof, principal).ErrorCode, Is.EqualTo("CMD_PAYLOAD_MISMATCH"));
            changed = start.Payload.ToDictionary(x => x.Key, x => x.Value); changed["quantity"] = 0L;
            Assert.That(p.Execute(F.Command(p, "override", "StartWeaving", changed), principal).ErrorCode, Is.EqualTo("INVALID_PAYLOAD"));
            var stale = new CommandEnvelope("stale", start.CommandType, start.IssuerId, "Player", 0, 1, 1, start.Payload, start.CorrelationId);
            Assert.That(p.Execute(stale, principal).ErrorCode, Is.EqualTo("STALE_REVISION"));
        }
        [Test]
        public void S17_01_ExactlyFourBatchesExhaustStarterStock()
        {
            var p = F.Processor(); var principal = F.Principal(p.InspectCommitted().World);
            for (var n = 0; n < 4; n++) { p.Execute(F.Start(p, "batch." + n), principal); F.Advance(p, 30 * (n + 1), "time." + n); }
            var w = p.InspectCommitted().World; var state = w.Weaving!;
            Assert.That(w.GetLot(state.InputLotId).OnHand, Is.Zero); Assert.That(w.GetLot(state.OutputLotId).OnHand, Is.EqualTo(4));
            Assert.That(p.Execute(F.Start(p, "batch.fifth"), principal).ErrorCode, Is.EqualTo("INSUFFICIENT_RESOURCE"));
            var transfer = F.Command(p, "transfer.denied", "TransferResource", new Dictionary<string, object?>());
            Assert.That(p.Execute(transfer, principal).ErrorCode, Is.EqualTo("COMMAND_UNKNOWN"));
            Assert.That(p.Execute(F.Command(p, "schedule.denied", "ScheduleTransfer", new Dictionary<string, object?>()), principal).ErrorCode, Is.EqualTo("COMMAND_UNKNOWN"));
        }
        [Test]
        public void S17_01_PrecommitFaultDoesNotPublishOrConsumeIds()
        {
            bool fail = true; var world = F.World(); var p = new CommandProcessor(world, F.Rules(), beforeCommit: () => { if (fail) throw new InvalidOperationException("fault"); }, weavingRecipe: F.Recipe);
            var before = p.InspectCommitted(); var start = F.Start(p); Assert.Throws<InvalidOperationException>(() => p.Execute(start, F.Principal(world)));
            Assert.That(p.InspectCommitted(), Is.SameAs(before)); fail = false; p.Execute(start, F.Principal(world));
            Assert.That(p.InspectCommitted().World.Weaving!.Activities.Single().Id.Value.Counter, Is.EqualTo(1));
        }
        [Test]
        public void S17_01_ResponseLossRetriesOriginalReceiptWithoutExtraHold()
        {
            var world = F.World(); var p = new CommandProcessor(world, F.Rules(), afterCommit: () => throw new InvalidOperationException("lost"), weavingRecipe: F.Recipe);
            var start = F.Start(p); Assert.Throws<InvalidOperationException>(() => p.Execute(start, F.Principal(world)));
            var current = p.InspectCommitted(); Assert.That(p.Execute(start, F.Principal(world)), Is.SameAs(current.AcceptedReceipts.Single()));
            Assert.That(p.InspectCommitted(), Is.SameAs(current)); Assert.That(current.World.GetLot(world.Weaving!.InputLotId).Reserved, Is.EqualTo(2));
        }
        [Test]
        public void S17_01_DueTickOverflowRejectedWithoutStateChange()
        {
            var world = F.World(tick: long.MaxValue - 10); var p = F.Processor(world); var before = p.InspectCommitted();
            Assert.That(p.Execute(F.Start(p), F.Principal(world)).ErrorCode, Is.EqualTo("TIME_RANGE")); Assert.That(p.InspectCommitted().World, Is.SameAs(before.World));
        }
        [TestCase(1, 0, "INSUFFICIENT_RESOURCE")] [TestCase(8, long.MaxValue, "NUMERIC_OVERFLOW")]
        public void S17_01_PureRulesRejectInsufficientOrOverflowTrustedInput(long input, long output, string error)
        {
            var w = F.World(input: input, output: output); var s = w.Weaving!;
            var result = WeavingRules.EvaluateStart(w, s.ActorId, s.ActorId, s.InputLotId, s.OutputLotId,
                new EntityId<Activity>(new EntityId(w.WorldId, "activity", 1)), new EntityId<Job>(new EntityId(w.WorldId, "job", 1)), "start", "test");
            Assert.That(result.ErrorCode, Is.EqualTo(error));
        }
        [Test]
        public void S17_01_MissingOrDifferentRecipeRefused()
        {
            var w = F.World(); Assert.Throws<ArgumentException>(() => new CommandProcessor(w, F.Rules()));
            Assert.Throws<ArgumentException>(() => new CommandProcessor(w, F.Rules(), weavingRecipe: new WeavingRecipe(3, 30, 1, 40, F.Recipe.Fingerprint)));
        }
    }
}
