using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.Application.CommandPipeline;
using Game.Application.Events;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Domain.Repair;
using Game.Infrastructure.Persistence;
using NUnit.Framework;
using F = MythicalDnd.Foundation.Tests.BoatRepairTestFixture;

namespace MythicalDnd.Foundation.Tests
{
    /// <summary>Independent malformed-restore cases; these do not implement health/travel or alter source saves.</summary>
    [TestFixture]
    public sealed class BoatRepairCausalityTests
    {
        private static CommandProcessor Started()
        {
            var registry = F.Registry();
            var processor = new CommandProcessor(F.Initial(registry), F.Rules(), repairRecipe: registry.Recipe);
            Assert.That(processor.Execute(F.Start(), F.Principal).Outcome, Is.EqualTo(CommandOutcome.Accepted));
            return processor;
        }

        private static CommittedWorld Recompose(CommittedWorld source, WorldSnapshot? world = null,
            SchedulerState? scheduler = null, IEnumerable<CommittedEvent>? journal = null)
        {
            var constructor = typeof(CommittedWorld).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single();
            return (CommittedWorld)constructor.Invoke(new object?[] { world ?? source.World, source.AcceptedReceipts,
                source.MetadataReceipts, source.Events, scheduler ?? source.Scheduler, journal ?? source.Journal.Events });
        }

        private static void RejectHistory(CommittedWorld malformed)
        {
            var argument = Assert.Throws<ArgumentException>(() =>
                new CommandProcessor(malformed, F.Rules(), repairRecipe: malformed.World.Repair!.Recipe));
            Assert.That(argument!.Message, Does.StartWith("REPAIR_HISTORY_"));
            var image = CommittedWorldCodec.Encode(malformed);
            var format = Assert.Throws<FormatException>(() => CommittedWorldCodec.Decode(image));
            Assert.That(format!.Message, Does.StartWith("REPAIR_HISTORY"));
        }

        private static CommittedEvent EventWith(CommittedEvent source, string? type = null,
            string? rule = null, IEnumerable<KeyValuePair<string, object?>>? payload = null,
            Game.Foundation.Identity.EntityId? aggregate = null)
            => new CommittedEvent(source.Id, type ?? source.EventType, aggregate ?? source.AggregateId,
                source.Tick, source.CommitSequence, source.EventIndex, source.CausationId, source.CorrelationId,
                rule ?? source.RuleVersion, source.PayloadVersion, payload ?? source.Payload, source.EnvelopeVersion);

        [Test]
        public void VS_03A_RestoreRejectsActivityAndJobWhoseStartCauseHasNoReceipt()
        {
            var source = Started().InspectCommitted(); var repair = source.World.Repair!;
            var old = repair.Activities.Single(); var oldJob = source.Scheduler.Jobs.Single();
            var activity = new RepairActivity(old.Id, old.ActorId, old.ItemId, old.MaterialLotId,
                old.ToolItemId, old.Worksite, old.JobId, old.StartedTick, old.DueTick, old.Recipe,
                "missing.start.receipt", old.CorrelationId, old.Status, old.ReservedQuantity, old.TerminalReason);
            var job = ScheduledJob.ForRepair(oldJob.Id, oldJob.ActorId, oldJob.DueTick, oldJob.CreationSequence,
                activity.Id, activity.StartCommandId, oldJob.CorrelationId, oldJob.Status);
            var changed = new RepairState(repair.Recipe, repair.Items, repair.Capabilities, new[] { activity });
            var world = new WorldSnapshot(source.World.Identity, source.World.Tick, source.World.Actors,
                source.World.Lots, source.World.RngStates, changed);
            var scheduler = SchedulerState.Restore(new[] { job }, source.Scheduler.NextCreationSequence,
                source.Scheduler.SequenceExhausted);
            RejectHistory(Recompose(source, world, scheduler));
        }

        [TestCase("material_quantity")]
        [TestCase("recipe_hash")]
        [TestCase("due_tick")]
        [TestCase("aggregate")]
        [TestCase("rule_version")]
        public void VS_03A_RestoreRejectsStartEventThatContradictsPinnedActivity(string flaw)
        {
            var source = Started().InspectCommitted(); var start = source.Journal.Events.Single();
            var payload = start.Payload.ToDictionary(pair => pair.Key, pair => pair.Value);
            if (flaw == "material_quantity") payload[flaw] = 1L;
            if (flaw == "recipe_hash") payload[flaw] = new string('0', 64);
            if (flaw == "due_tick") payload[flaw] = 61L;
            var corrupt = EventWith(start, rule: flaw == "rule_version" ? "transfer-resource-v1" : null,
                payload: payload, aggregate: flaw == "aggregate" ? F.Boat.Value : null);
            RejectHistory(Recompose(source, journal: new[] { corrupt }));
        }

        [Test]
        public void VS_03A_RestoreRejectsCompletedActivityWithoutItsCompletionFact()
        {
            var processor = Started(); processor.Execute(F.Advance(), F.Principal); processor.DrainAdvance(F.Principal);
            var source = processor.InspectCommitted();
            Assert.That(source.World.Repair!.Activities.Single().Status, Is.EqualTo(RepairActivityStatus.Completed));
            var changed = source.Journal.Events.Select(fact => fact.EventType == "RepairCompleted"
                ? EventWith(fact, type: "UnrelatedFact") : fact).ToArray();
            RejectHistory(Recompose(source, journal: changed));
        }

        [Test]
        public void VS_03A_RestoreRejectsCancelledReasonThatDisagreesWithCancellationFact()
        {
            var processor = Started(); processor.Execute(F.Cancel(2), F.Principal);
            var source = processor.InspectCommitted(); var repair = source.World.Repair!;
            var old = repair.Activities.Single();
            var activity = new RepairActivity(old.Id, old.ActorId, old.ItemId, old.MaterialLotId,
                old.ToolItemId, old.Worksite, old.JobId, old.StartedTick, old.DueTick, old.Recipe,
                old.StartCommandId, old.CorrelationId, old.Status, old.ReservedQuantity, "different_reason");
            var changed = new RepairState(repair.Recipe, repair.Items, repair.Capabilities, new[] { activity });
            var world = new WorldSnapshot(source.World.Identity, source.World.Tick, source.World.Actors,
                source.World.Lots, source.World.RngStates, changed);
            RejectHistory(Recompose(source, world));
        }
    }
}
