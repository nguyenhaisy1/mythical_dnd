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
    /// <summary>Independent restore checks for the causal Advance/Drain link of automatic cancellation.</summary>
    [TestFixture]
    public sealed class BoatRepairWorkflowBindingTests
    {
        private static CommittedWorld Recompose(CommittedWorld source, WorldSnapshot? world = null,
            SchedulerState? scheduler = null)
            => (CommittedWorld)typeof(CommittedWorld).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single()
                .Invoke(new object?[] { world ?? source.World, source.AcceptedReceipts, source.MetadataReceipts,
                    source.Events, scheduler ?? source.Scheduler, source.Journal.Events });

        private static CommittedWorld AutomaticallyCancelled()
        {
            var registry = F.Registry();
            var processor = new CommandProcessor(F.Initial(registry), F.Rules(), repairRecipe: registry.Recipe);
            Assert.That(processor.Execute(F.Start(), F.Principal).Outcome, Is.EqualTo(CommandOutcome.Accepted));
            Assert.That(processor.Execute(F.Advance(), F.Principal).Outcome, Is.EqualTo(CommandOutcome.AcceptedPending));
            var accepted = processor.InspectCommitted(); var world = accepted.World; var repair = world.Repair!;
            // Trusted fixture invalidation, not a claim that profession mutation is implemented.
            var changed = new RepairState(repair.Recipe, repair.Items,
                repair.Capabilities.Select(c => new RepairCapability(c.ActorId, false, c.ToolItemId)), repair.Activities);
            var invalidated = new WorldSnapshot(world.Identity, world.Tick, world.Actors, world.Lots, world.RngStates, changed);
            processor = new CommandProcessor(Recompose(accepted, invalidated), F.Rules(), repairRecipe: registry.Recipe);
            processor.DrainAdvance(F.Principal);
            var result = processor.InspectCommitted();
            Assert.That(result.World.Repair!.GetActivity(F.Activity).Status, Is.EqualTo(RepairActivityStatus.Cancelled));
            Assert.That(result.World.Repair.GetActivity(F.Activity).TerminalReason, Is.EqualTo("NOT_AUTHORIZED"));
            Assert.That(result.Scheduler.Executions.Count, Is.EqualTo(1));
            Assert.That(result.AcceptedReceipts.Count(r => r.OriginalEnvelope.CommandType == "ExecuteScheduledRepair"), Is.EqualTo(1));
            // The unmodified aggregate must really be accepted by both restore paths.
            Assert.DoesNotThrow(() => new CommandProcessor(result, F.Rules(), repairRecipe: registry.Recipe));
            Assert.DoesNotThrow(() => CommittedWorldCodec.Decode(CommittedWorldCodec.Encode(result)));
            return result;
        }

        private static SchedulerState Scheduler(CommittedWorld source, IEnumerable<AdvanceWorkflow> workflows,
            IEnumerable<JobExecution> executions)
            => (SchedulerState)typeof(SchedulerState).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single()
                .Invoke(new object?[] { source.Scheduler.Jobs, source.Scheduler.NextCreationSequence,
                    source.Scheduler.SequenceExhausted, workflows, executions });

        private static AdvanceWorkflow WithoutExecutionCount(AdvanceWorkflow source)
            => (AdvanceWorkflow)typeof(AdvanceWorkflow).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single()
                .Invoke(new object?[] { source.Id, source.OwnerId, source.OwnerSource, source.TargetTick,
                    source.Status, null, 0UL, source.CorrelationId });

        private static void RejectBoth(CommittedWorld malformed)
        {
            Assert.Multiple(() => {
                Assert.That(() => new CommandProcessor(malformed, F.Rules(), repairRecipe: malformed.World.Repair!.Recipe),
                    Throws.ArgumentException.With.Message.StartsWith("REPAIR_HISTORY_"));
                Assert.That(() => CommittedWorldCodec.Decode(CommittedWorldCodec.Encode(malformed)),
                    Throws.TypeOf<FormatException>().With.Message.StartsWith("REPAIR_HISTORY"));
            });
        }

        [Test]
        public void VS_03A_RestoreRejectsAutomaticCancellationWithoutAnyWorkflowOrExecution()
        {
            var source = AutomaticallyCancelled();
            RejectBoth(Recompose(source, scheduler: Scheduler(source, Array.Empty<AdvanceWorkflow>(), Array.Empty<JobExecution>())));
        }

        [Test]
        public void VS_03A_RestoreRejectsAutomaticCancellationWithRemovedExecutionAndReconciledWorkflowCount()
        {
            var source = AutomaticallyCancelled();
            var workflows = source.Scheduler.Workflows.Select(WithoutExecutionCount).ToArray();
            RejectBoth(Recompose(source, scheduler: Scheduler(source, workflows, Array.Empty<JobExecution>())));
        }

        [Test]
        public void VS_03A_RestoreRejectsAutomaticCancellationWithExecutionButNoOwningWorkflow()
        {
            var source = AutomaticallyCancelled();
            RejectBoth(Recompose(source, scheduler: Scheduler(source, Array.Empty<AdvanceWorkflow>(), source.Scheduler.Executions)));
        }
    }
}
