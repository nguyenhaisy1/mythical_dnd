using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Game.Application.CommandPipeline;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Time;

namespace Game.Headless
{
    internal static class SchedulerDemo
    {
        public static int Run()
        {
            var ids = IdentitySnapshot.Empty(1).BeginStage(1);
            var actor = ids.Allocate<Actor>();
            var location = ids.Allocate<Settlement>();
            var source = ids.Allocate<Lot>();
            var destination = ids.Allocate<Lot>();
            var world = new WorldSnapshot(ids.PrepareSnapshot(), new WorldTick(0),
                new[] { new ActorState(actor, true, location) }, new[] {
                    new ResourceLot(source, DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 10, 0),
                    new ResourceLot(destination, DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 0, 0) });
            var processor = new CommandProcessor(world, new RuleRegistry(new[] { new TransferResourceRule() }));
            var principal = new ExecutionPrincipal(actor, CommandSource.Player);
            CommandEnvelope Envelope(string id, string type, Dictionary<string, object?> payload)
            {
                var state = processor.InspectCommitted().World;
                return new CommandEnvelope(id, type, actor.ToString(), "Player", state.Tick.Value,
                    state.Revision.Value, 1, payload, "scheduler-demo");
            }
            CommandEnvelope Schedule(string id, long due, long quantity) => Envelope(id, "ScheduleTransfer",
                new Dictionary<string, object?> { ["due_tick"] = due, ["phase"] = 3, ["priority"] = 100,
                    ["source_lot_id"] = source.ToString(), ["destination_lot_id"] = destination.ToString(), ["quantity"] = quantity });
            var scheduled1 = processor.Execute(Schedule("schedule-first", 10, 2), principal);
            var scheduled2 = processor.Execute(Schedule("schedule-second", 20, 3), principal);
            var advance = Envelope("advance-demo", "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = 30L });
            var pending = processor.Execute(advance, principal);
            processor.DrainAdvance(principal);
            var retry = processor.Execute(advance, principal);
            var committed = processor.InspectCommitted();
            var workflow = committed.Scheduler.Workflow!;
            bool valid = scheduled1.Outcome == CommandOutcome.Accepted && scheduled2.Outcome == CommandOutcome.Accepted
                && pending.Outcome == CommandOutcome.AcceptedPending && ReferenceEquals(pending, retry)
                && workflow.Status == AdvanceStatus.Completed && committed.World.Tick.Value == 30
                && committed.World.Revision.Value == 7 && committed.World.GetLot(source).OnHand == 5
                && committed.World.GetLot(destination).OnHand == 5 && committed.Scheduler.Executions.Count == 2
                && committed.Scheduler.Executions.Select(e => e.Tick.Value).SequenceEqual(new long[] { 10, 20 })
                && committed.Scheduler.Jobs.All(j => j.Status == ScheduledJobStatus.Completed);
            Console.WriteLine(JsonSerializer.Serialize(new {
                scope = "in-memory-scheduler-demo", passed = valid, durable = false,
                currentTick = committed.World.Tick.Value, revision = committed.World.Revision.Value,
                sourceAfter = committed.World.GetLot(source).OnHand, destinationAfter = committed.World.GetLot(destination).OnHand,
                executedJobs = committed.Scheduler.Executions.Count, dueTicks = committed.Scheduler.Executions.Select(e => e.Tick.Value).ToArray(),
                workflowStatus = workflow.Status.ToString(), retryReturnsOriginalReceipt = ReferenceEquals(pending, retry)
            }, new JsonSerializerOptions { WriteIndented = true }));
            return valid ? 0 : 1;
        }
    }
}
