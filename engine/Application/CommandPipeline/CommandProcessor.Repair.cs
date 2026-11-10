using System;
using System.Collections.Generic;
using System.Linq;
using Game.Application.Events;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Domain.Repair;
using Game.Foundation.Identity;
using Game.Foundation.Time;

namespace Game.Application.CommandPipeline
{
    public sealed partial class CommandProcessor
    {
        private readonly RepairRecipe? repairRecipe;
        private void ValidateRepairProfile(WorldSnapshot world)
        {
            if (world.Repair != null && (repairRecipe == null || world.Repair.Recipe.Fingerprint != repairRecipe.Fingerprint))
                throw new ArgumentException("REPAIR_PROFILE_MISMATCH");
        }
        private static void ValidateRepairBindings(WorldSnapshot world, SchedulerState scheduler)
        {
            var repairJobs = scheduler.Jobs.Where(j => j.IsRepair).ToArray();
            if (world.Repair == null)
            {
                if (repairJobs.Length != 0) throw new ArgumentException("REPAIR_PROFILE_MISSING");
                return;
            }
            var repair = world.Repair;
            if (repair.Activities.Count != repairJobs.Length) throw new ArgumentException("REPAIR_JOB_PAIRING");
            foreach (var activity in repair.Activities)
            {
                var job = repairJobs.SingleOrDefault(j => j.Id.Equals(activity.JobId));
                var status = activity.Status == RepairActivityStatus.Running ? ScheduledJobStatus.Pending
                    : activity.Status == RepairActivityStatus.Completed ? ScheduledJobStatus.Completed : ScheduledJobStatus.Cancelled;
                if (job == null || !job.RepairActivityId!.Equals(activity.Id) || !job.ActorId.Equals(activity.ActorId) ||
                    job.DueTick != activity.DueTick || job.Phase != SchedulerPhase.ProductionHealth || job.Priority != 0 ||
                    job.HandlerKey != "boat-repair-v1" || job.Status != status || job.CausationId != activity.StartCommandId ||
                    job.CorrelationId != activity.CorrelationId || job.DeferredFromTick.HasValue || job.AncestorKeys.Count != 0 ||
                    (status == ScheduledJobStatus.Pending && job.DueTick < world.Tick)) throw new ArgumentException("REPAIR_JOB_PAIRING");
                if (world.Identity.Resolve<Job>(job.Id.Value).Lifecycle != (status == ScheduledJobStatus.Pending ? EntityLifecycle.Active : EntityLifecycle.Retired))
                    throw new ArgumentException("REPAIR_JOB_LIFECYCLE");
            }
        }
        private CommandReceipt ExecuteRepair(CommandEnvelope envelope, ExecutionPrincipal principal, string fingerprint)
        {
            if (current.World.Repair == null || repairRecipe == null)
                return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "COMMAND_UNKNOWN");
            if (envelope.PayloadVersion != 1) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "PAYLOAD_VERSION_UNSUPPORTED");
            try
            {
                return envelope.CommandType == "StartRepair" ? StartRepair(envelope, principal, fingerprint) : CancelRepair(envelope, principal, fingerprint);
            }
            catch (SchedulerInputException error) { return Metadata(envelope, fingerprint, CommandOutcome.Rejected, error.Code); }
        }
        private CommandReceipt StartRepair(CommandEnvelope envelope, ExecutionPrincipal principal, string fingerprint)
        {
            if (envelope.Payload.Count != 4) throw new SchedulerInputException("INVALID_PAYLOAD");
            var actorId = Input(() => EntityId<Actor>.Parse(Text(envelope.Payload, "actor_id")));
            var itemId = Input(() => EntityId<Item>.Parse(Text(envelope.Payload, "item_id")));
            var lotId = Input(() => EntityId<Lot>.Parse(Text(envelope.Payload, "material_lot_id")));
            var recipeId = Input(() => DefinitionId.Parse(Text(envelope.Payload, "recipe_id")));
            var before = current; var world = before.World;
            if (before.Scheduler.SequenceExhausted) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "SEQUENCE_EXHAUSTED");
            var revision = NextRevision(world); var stage = world.Identity.BeginStage(revision);
            var activityId = stage.Allocate<Activity>(); var jobId = stage.Allocate<Job>();
            var result = BoatRepairRules.EvaluateStart(world, principal.ActorId, actorId, itemId, lotId, recipeId, activityId, jobId,
                envelope.CommandId, envelope.CorrelationId);
            if (result.ErrorCode != null) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, result.ErrorCode);
            var plan = result.Plan ?? throw new PipelineFailureException("MISSING_REPAIR_PROPOSAL");
            ValidateStartRepairTarget(world, plan.Activity, actorId, itemId, lotId, jobId, envelope);
            ValidateRepairPlan(world, plan, activityId, RepairActivityStatus.Running);
            var sequence = before.Scheduler.NextCreationSequence;
            var job = ScheduledJob.ForRepair(jobId, actorId, plan.Activity.DueTick, sequence, activityId, envelope.CommandId, envelope.CorrelationId);
            var scheduler = before.Scheduler.With(jobs: before.Scheduler.Jobs.Concat(new[] { job }),
                next: sequence == ulong.MaxValue ? sequence : sequence + 1, exhausted: sequence == ulong.MaxValue);
            return CommitRepair(before, envelope, fingerprint, plan, stage, scheduler, "RepairStarted", false, null);
        }
        private CommandReceipt CancelRepair(CommandEnvelope envelope, ExecutionPrincipal principal, string fingerprint)
        {
            if (envelope.Payload.Count != 2) throw new SchedulerInputException("INVALID_PAYLOAD");
            var activityId = Input(() => EntityId<Activity>.Parse(Text(envelope.Payload, "activity_id")));
            var reason = Text(envelope.Payload, "reason"); var before = current; var world = before.World;
            var result = BoatRepairRules.EvaluateCancel(world, principal.ActorId, activityId, reason);
            if (result.ErrorCode != null) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, result.ErrorCode);
            var plan = result.Plan ?? throw new PipelineFailureException("MISSING_REPAIR_PROPOSAL");
            ValidateRepairPlan(world, plan, activityId, RepairActivityStatus.Cancelled);
            var stage = world.Identity.BeginStage(NextRevision(world)); stage.Retire(activityId); stage.Retire(plan.Activity.JobId);
            var scheduler = before.Scheduler.With(jobs: before.Scheduler.Jobs.Select(j => j.Id.Equals(plan.Activity.JobId) ? j.WithStatus(ScheduledJobStatus.Cancelled) : j));
            return CommitRepair(before, envelope, fingerprint, plan, stage, scheduler, "ActivityCancelled", true, null);
        }
        private void ExecuteRepairJob(ScheduledJob job, bool lastBudgetSlot)
        {
            var before = current; var world = before.World; var workflow = before.Scheduler.Workflow!;
            ValidateRepairBindings(world, before.Scheduler);
            if (job.DueTick < world.Tick) throw new PipelineFailureException("JOB_IN_PAST");
            var command = new CommandEnvelope("scheduler/job/" + job.Id, "ExecuteScheduledRepair", job.ActorId.ToString(), "Scheduler",
                world.Tick.Value, world.Revision.Value, 1, new Dictionary<string, object?> { ["job_id"] = job.Id.ToString(), ["workflow_id"] = workflow.Id }, job.CorrelationId);
            var input = WithIdentityAndTick(world, world.Identity, job.DueTick);
            var result = BoatRepairRules.EvaluateComplete(input, job.RepairActivityId!);
            if (result.ErrorCode != null) throw new PipelineFailureException("REPAIR_COMPLETION_STATE");
            var plan = result.Plan ?? throw new PipelineFailureException("MISSING_REPAIR_PROPOSAL");
            ValidateRepairPlan(input, plan, job.RepairActivityId!, plan.Activity.Status);
            if (plan.Activity.Status != RepairActivityStatus.Completed && plan.Activity.Status != RepairActivityStatus.Cancelled)
                throw new PipelineFailureException("REPAIR_COMPLETION_STATE");
            var cancelled = plan.Activity.Status == RepairActivityStatus.Cancelled;
            var jobs = before.Scheduler.Jobs.Select(j => j.Id.Equals(job.Id) ? j.WithStatus(cancelled ? ScheduledJobStatus.Cancelled : ScheduledJobStatus.Completed) : j).ToArray();
            var status = lastBudgetSlot && jobs.Any(j => j.Status == ScheduledJobStatus.Pending && j.DueTick <= workflow.TargetTick)
                ? AdvanceStatus.PausedBudget : AdvanceStatus.Pending;
            var scheduler = before.Scheduler.With(jobs: jobs, workflow: workflow.With(status, job.Id, true),
                execution: new JobExecution(job, command.CommandId, plan.Activity.TerminalReason));
            var stage = world.Identity.BeginStage(NextRevision(world)); stage.Retire(job.RepairActivityId!); stage.Retire(job.Id);
            CommitRepair(before, command, command.Fingerprint(), plan, stage, scheduler,
                cancelled ? "ActivityCancelled" : "RepairCompleted", true, workflow.Id, job.DueTick);
        }
        private CommandReceipt CommitRepair(CommittedWorld before, CommandEnvelope command, string fingerprint, RepairPlan plan,
            IdentityStage stage, SchedulerState scheduler, string eventType, bool terminal, string? workflowId, WorldTick? dueTick = null)
        {
            var world = before.World; var tick = dueTick ?? world.Tick; var revision = NextRevision(world); var activity = plan.Activity;
            var facts = new List<CommittedEvent>();
            facts.Add(new CommittedEvent(stage.Allocate<Event>(), eventType, activity.Id.Value, tick, revision, 0,
                command.CommandId, command.CorrelationId, "boat-repair-v1", 1, new Dictionary<string, object?> {
                    ["activity_id"] = activity.Id.ToString(), ["actor_id"] = activity.ActorId.ToString(), ["item_id"] = activity.ItemId.ToString(),
                    ["material_lot_id"] = activity.MaterialLotId.ToString(), ["tool_item_id"] = activity.ToolItemId.ToString(), ["job_id"] = activity.JobId.ToString(),
                    ["recipe_id"] = activity.Recipe.Id.ToString(), ["recipe_hash"] = activity.Recipe.Fingerprint, ["reserved_quantity"] = activity.ReservedQuantity,
                    ["material_quantity"] = activity.Recipe.MaterialQuantity, ["due_tick"] = activity.DueTick.Value, ["reason"] = activity.TerminalReason }));
            if (terminal)
                facts.Add(new CommittedEvent(stage.Allocate<Event>(), activity.Status == RepairActivityStatus.Completed ? "JobCompleted" : "JobCancelled",
                    activity.JobId.Value, tick, revision, 1, command.CommandId, command.CorrelationId, "scheduler-v1", 1,
                    new Dictionary<string, object?> { ["error_code"] = activity.TerminalReason, ["scheduled_by"] = activity.StartCommandId,
                        ["workflow_id"] = workflowId, ["status"] = scheduler.Workflow?.Status.ToString() }));
            var candidate = new WorldSnapshot(stage.PrepareSnapshot(), tick, world.Actors, plan.Lots, world.RngStates, plan.Repair, world.Origin, world.Weaving);
            var receipt = new CommandReceipt(command, fingerprint, CommandOutcome.Accepted, null, world.Revision.Value, revision, tick, facts.Select(f => f.Id));
            Publish(before, new CommittedWorld(candidate, before.AcceptedReceipts.Concat(new[] { receipt }), before.MetadataReceipts,
                before.Events, scheduler, before.Journal.Events.Concat(facts)));
            return receipt;
        }
        private static void ValidateStartRepairTarget(WorldSnapshot world, RepairActivity activity, EntityId<Actor> actorId,
            EntityId<Item> itemId, EntityId<Lot> materialLotId, EntityId<Job> jobId, CommandEnvelope command)
        {
            var repair = world.Repair ?? throw new PipelineFailureException("REPAIR_PROFILE_MISSING");
            var actor = world.GetActor(actorId); var capability = repair.GetCapability(actorId);
            if (!activity.ActorId.Equals(actorId) || !activity.ItemId.Equals(itemId) || !activity.MaterialLotId.Equals(materialLotId) ||
                !activity.JobId.Equals(jobId) || !activity.ToolItemId.Equals(capability.ToolItemId) || !activity.Worksite.Equals(actor.Location) ||
                activity.StartedTick != world.Tick || activity.DueTick != world.Tick.Add(repair.Recipe.DurationTicks) ||
                activity.Recipe.Fingerprint != repair.Recipe.Fingerprint || activity.StartCommandId != command.CommandId ||
                activity.CorrelationId != command.CorrelationId || activity.TerminalReason != null)
                throw new PipelineFailureException("PROPOSAL_REPAIR_START_TARGET");
            ValidateRepairEligibility(world, activity);
            if (world.GetLot(materialLotId).Available < activity.Recipe.MaterialQuantity ||
                repair.Activities.Any(a => a.Status == RepairActivityStatus.Running &&
                    (a.ActorId.Equals(actorId) || a.ItemId.Equals(itemId) || a.ToolItemId.Equals(activity.ToolItemId))))
                throw new PipelineFailureException("PROPOSAL_REPAIR_PRECONDITION");
        }
        private static void ValidateRepairEligibility(WorldSnapshot world, RepairActivity activity)
        {
            var repair = world.Repair ?? throw new PipelineFailureException("REPAIR_PROFILE_MISSING");
            var actor = world.GetActor(activity.ActorId); var capability = repair.GetCapability(activity.ActorId);
            var item = repair.GetItem(activity.ItemId); var tool = repair.GetItem(activity.ToolItemId);
            var material = world.GetLot(activity.MaterialLotId);
            if (!actor.Alive || !capability.Qualified || !capability.ToolItemId.Equals(activity.ToolItemId) ||
                item.Kind != RepairItemKind.Boat || item.Destroyed || item.Condition >= activity.Recipe.ConditionMax ||
                tool.Kind != RepairItemKind.RepairTool || tool.Destroyed || tool.Condition <= 0 ||
                !actor.Controls(item.Owner) || !actor.Controls(tool.Owner) || !actor.Controls(material.Owner) ||
                !actor.Location.Equals(activity.Worksite) || !item.Location.Equals(activity.Worksite) ||
                !tool.Location.Equals(activity.Worksite) || !material.Location.Equals(activity.Worksite) ||
                !material.Commodity.Equals(activity.Recipe.MaterialCommodity))
                throw new PipelineFailureException("PROPOSAL_REPAIR_PRECONDITION");
        }
        private static bool SameItemMetadata(RepairItemState left, RepairItemState right)
            => left.Id.Equals(right.Id) && left.Kind == right.Kind && left.Owner.Equals(right.Owner) &&
                left.Location.Equals(right.Location) && left.Destroyed == right.Destroyed;
        private static bool SameActivityPins(RepairActivity left, RepairActivity right)
            => left.Id.Equals(right.Id) && left.ActorId.Equals(right.ActorId) && left.ItemId.Equals(right.ItemId) &&
                left.MaterialLotId.Equals(right.MaterialLotId) && left.ToolItemId.Equals(right.ToolItemId) &&
                left.Worksite.Equals(right.Worksite) && left.JobId.Equals(right.JobId) && left.StartedTick == right.StartedTick &&
                left.DueTick == right.DueTick && left.Recipe.Fingerprint == right.Recipe.Fingerprint &&
                left.StartCommandId == right.StartCommandId && left.CorrelationId == right.CorrelationId;
        private static bool SameActivity(RepairActivity left, RepairActivity right)
            => SameActivityPins(left, right) && left.Status == right.Status && left.ReservedQuantity == right.ReservedQuantity &&
                left.TerminalReason == right.TerminalReason;
        private static void ValidateRepairPlan(WorldSnapshot world, RepairPlan plan, EntityId<Activity> activityId, RepairActivityStatus status)
        {
            var before = world.Repair ?? throw new PipelineFailureException("REPAIR_PROFILE_MISSING");
            var activity = plan.Activity;
            if (!Enum.IsDefined(typeof(RepairActivityStatus), status) || !activity.Id.Equals(activityId) || activity.Status != status ||
                plan.Repair.Recipe.Fingerprint != before.Recipe.Fingerprint || activity.Recipe.Fingerprint != before.Recipe.Fingerprint ||
                plan.Lots.Count != world.Lots.Count || plan.Lots.Select(l => l.Id).Distinct().Count() != plan.Lots.Count ||
                plan.Repair.Items.Count != before.Items.Count || plan.Repair.Capabilities.Count != before.Capabilities.Count)
                throw new PipelineFailureException("PROPOSAL_REPAIR_EFFECT");
            foreach (var capability in before.Capabilities)
            {
                var proposed = plan.Repair.Capabilities.SingleOrDefault(c => c.ActorId.Equals(capability.ActorId));
                if (proposed == null || proposed.Qualified != capability.Qualified || !proposed.ToolItemId.Equals(capability.ToolItemId))
                    throw new PipelineFailureException("PROPOSAL_REPAIR_CAPABILITY");
            }
            var proposedActivity = plan.Repair.Activities.SingleOrDefault(a => a.Id.Equals(activityId));
            if (proposedActivity == null || !SameActivity(proposedActivity, activity))
                throw new PipelineFailureException("PROPOSAL_REPAIR_ACTIVITY");
            var prior = before.Activities.SingleOrDefault(a => a.Id.Equals(activityId));
            if ((status == RepairActivityStatus.Running && (prior != null || plan.Repair.Activities.Count != before.Activities.Count + 1)) ||
                (status != RepairActivityStatus.Running && (prior == null || prior.Status != RepairActivityStatus.Running ||
                    plan.Repair.Activities.Count != before.Activities.Count || !SameActivityPins(prior, activity))))
                throw new PipelineFailureException("PROPOSAL_REPAIR_ACTIVITY");
            foreach (var old in before.Activities.Where(a => !a.Id.Equals(activityId)))
            {
                var proposed = plan.Repair.Activities.SingleOrDefault(a => a.Id.Equals(old.Id));
                if (proposed == null || !SameActivity(old, proposed)) throw new PipelineFailureException("PROPOSAL_REPAIR_ACTIVITY");
            }
            if (status == RepairActivityStatus.Completed)
            {
                if (world.Tick < prior!.DueTick) throw new PipelineFailureException("PROPOSAL_REPAIR_PRECONDITION");
                ValidateRepairEligibility(world, prior);
            }
            foreach (var old in before.Items)
            {
                var proposed = plan.Repair.Items.SingleOrDefault(i => i.Id.Equals(old.Id));
                var expectedCondition = status == RepairActivityStatus.Completed && old.Id.Equals(prior!.ItemId)
                    ? Math.Min(prior.Recipe.ConditionMax, checked(old.Condition + prior.Recipe.ConditionGain)) : old.Condition;
                if (proposed == null || !SameItemMetadata(old, proposed) || proposed.Condition != expectedCondition)
                    throw new PipelineFailureException("PROPOSAL_REPAIR_ITEM");
            }
            var quantity = status == RepairActivityStatus.Running ? activity.Recipe.MaterialQuantity : prior!.ReservedQuantity;
            foreach (var old in world.Lots)
            {
                var after = plan.Lots.SingleOrDefault(l => l.Id.Equals(old.Id));
                var selected = old.Id.Equals(activity.MaterialLotId);
                var expectedHand = selected && status == RepairActivityStatus.Completed ? checked(old.OnHand - quantity) : old.OnHand;
                var expectedReserved = selected ? checked(old.Reserved + (status == RepairActivityStatus.Running ? quantity : -quantity)) : old.Reserved;
                if (after == null || !after.Commodity.Equals(old.Commodity) || !after.Owner.Equals(old.Owner) || !after.Location.Equals(old.Location) ||
                    after.OnHand != expectedHand || after.Reserved != expectedReserved) throw new PipelineFailureException("PROPOSAL_REPAIR_MATERIAL");
            }
            if (activity.ReservedQuantity != (status == RepairActivityStatus.Running ? activity.Recipe.MaterialQuantity : 0))
                throw new PipelineFailureException("PROPOSAL_REPAIR_HOLD");
        }
    }
}
