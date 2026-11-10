using System;
using System.Collections.Generic;
using System.Linq;
using Game.Application.Events;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Domain.Weaving;
using Game.Foundation.Identity;
using Game.Foundation.Time;

namespace Game.Application.CommandPipeline
{
    public sealed partial class CommandProcessor
    {
        private readonly WeavingRecipe? weavingRecipe;
        private void ValidateWeavingProfile(WorldSnapshot world)
        {
            if (world.Weaving != null && (weavingRecipe == null || !SameWeavingRecipe(world.Weaving.Recipe, weavingRecipe)))
                throw new ArgumentException("WEAVING_PROFILE_MISMATCH");
        }
        private static bool SameWeavingRecipe(WeavingRecipe a, WeavingRecipe b)
            => a.Fingerprint == b.Fingerprint && a.FiberCost == b.FiberCost && a.DurationTicks == b.DurationTicks &&
                a.CordYield == b.CordYield && a.MinimumSkill == b.MinimumSkill;
        private static void ValidateWeavingBindings(WorldSnapshot world, SchedulerState scheduler)
        {
            var jobs = scheduler.Jobs.Where(j => j.IsWeaving).ToArray();
            if (world.Weaving == null)
            {
                if (jobs.Length != 0) throw new ArgumentException("WEAVING_PROFILE_MISSING");
                return;
            }
            var state = world.Weaving;
            if (scheduler.Jobs.Any(j => !j.IsWeaving) || state.Activities.Count != jobs.Length) throw new ArgumentException("WEAVING_JOB_PAIRING");
            foreach (var activity in state.Activities)
            {
                var job = jobs.SingleOrDefault(j => j.Id.Equals(activity.JobId));
                var status = activity.Status == WeavingActivityStatus.Running ? ScheduledJobStatus.Pending :
                    activity.Status == WeavingActivityStatus.Completed ? ScheduledJobStatus.Completed : ScheduledJobStatus.Cancelled;
                if (job == null || !job.WeavingActivityId!.Equals(activity.Id) || !job.ActorId.Equals(activity.ActorId) ||
                    job.IsRepair || job.DueTick != activity.DueTick || job.Phase != SchedulerPhase.ProductionHealth || job.Priority != 0 ||
                    job.HandlerKey != WeavingState.ProfileId || job.Status != status || job.CausationId != activity.StartCommandId ||
                    job.CorrelationId != activity.CorrelationId || job.DeferredFromTick.HasValue || job.AncestorKeys.Count != 0 ||
                    (status == ScheduledJobStatus.Pending && job.DueTick < world.Tick)) throw new ArgumentException("WEAVING_JOB_PAIRING");
            }
        }
        private CommandReceipt ExecuteWeaving(CommandEnvelope envelope, ExecutionPrincipal principal, string fingerprint)
        {
            if (current.World.Weaving == null || weavingRecipe == null) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "COMMAND_UNKNOWN");
            if (envelope.PayloadVersion != 1) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "PAYLOAD_VERSION_UNSUPPORTED");
            try { return envelope.CommandType == "StartWeaving" ? StartWeaving(envelope, principal, fingerprint) : CancelWeaving(envelope, principal, fingerprint); }
            catch (SchedulerInputException e) { return Metadata(envelope, fingerprint, CommandOutcome.Rejected, e.Code); }
        }
        private CommandReceipt StartWeaving(CommandEnvelope command, ExecutionPrincipal principal, string fingerprint)
        {
            if (command.Payload.Count != 3) throw new SchedulerInputException("INVALID_PAYLOAD");
            var actor = Input(() => EntityId<Actor>.Parse(Text(command.Payload, "actor_id")));
            var input = Input(() => EntityId<Lot>.Parse(Text(command.Payload, "input_lot_id")));
            var output = Input(() => EntityId<Lot>.Parse(Text(command.Payload, "output_lot_id")));
            var before = current; var world = before.World;
            if (before.Scheduler.SequenceExhausted) return Metadata(command, fingerprint, CommandOutcome.Rejected, "SEQUENCE_EXHAUSTED");
            var stage = world.Identity.BeginStage(NextRevision(world)); var activity = stage.Allocate<Activity>(); var jobId = stage.Allocate<Job>();
            var evaluation = WeavingRules.EvaluateStart(world, principal.ActorId, actor, input, output, activity, jobId, command.CommandId, command.CorrelationId);
            if (evaluation.ErrorCode != null) return Metadata(command, fingerprint, CommandOutcome.Rejected, evaluation.ErrorCode);
            var plan = evaluation.Plan ?? throw new PipelineFailureException("MISSING_WEAVING_PROPOSAL");
            if (!plan.Activity.ActorId.Equals(actor) || !input.Equals(plan.Weaving.InputLotId) || !output.Equals(plan.Weaving.OutputLotId) ||
                !plan.Activity.JobId.Equals(jobId) || plan.Activity.StartCommandId != command.CommandId || plan.Activity.CorrelationId != command.CorrelationId ||
                plan.Activity.StartedTick != world.Tick || plan.Activity.DueTick != world.Tick.Add(weavingRecipe!.DurationTicks) ||
                WeavingRules.Eligibility(world) != null || world.GetLot(input).Available < weavingRecipe.FiberCost ||
                world.Weaving!.Activities.Any(a => a.Status == WeavingActivityStatus.Running)) throw new PipelineFailureException("PROPOSAL_WEAVING_PRECONDITION");
            ValidateWeavingPlan(world, plan, activity, WeavingActivityStatus.Running);
            var sequence = before.Scheduler.NextCreationSequence;
            var job = ScheduledJob.ForWeaving(jobId, actor, plan.Activity.DueTick, sequence, activity, command.CommandId, command.CorrelationId);
            var scheduler = before.Scheduler.With(jobs: before.Scheduler.Jobs.Concat(new[] { job }),
                next: sequence == ulong.MaxValue ? sequence : sequence + 1, exhausted: sequence == ulong.MaxValue);
            return CommitWeaving(before, command, fingerprint, plan, stage, scheduler, "WeavingStarted", false, null);
        }
        private CommandReceipt CancelWeaving(CommandEnvelope command, ExecutionPrincipal principal, string fingerprint)
        {
            if (command.Payload.Count != 2) throw new SchedulerInputException("INVALID_PAYLOAD");
            var activityId = Input(() => EntityId<Activity>.Parse(Text(command.Payload, "activity_id")));
            var reason = Text(command.Payload, "reason"); var before = current;
            var evaluation = WeavingRules.EvaluateCancel(before.World, principal.ActorId, activityId, reason);
            if (evaluation.ErrorCode != null) return Metadata(command, fingerprint, CommandOutcome.Rejected, evaluation.ErrorCode);
            var plan = evaluation.Plan ?? throw new PipelineFailureException("MISSING_WEAVING_PROPOSAL");
            ValidateWeavingPlan(before.World, plan, activityId, WeavingActivityStatus.Cancelled);
            var stage = before.World.Identity.BeginStage(NextRevision(before.World)); stage.Retire(activityId); stage.Retire(plan.Activity.JobId);
            var scheduler = before.Scheduler.With(jobs: before.Scheduler.Jobs.Select(j => j.Id.Equals(plan.Activity.JobId) ? j.WithStatus(ScheduledJobStatus.Cancelled) : j));
            return CommitWeaving(before, command, fingerprint, plan, stage, scheduler, "ActivityCancelled", true, null);
        }
        private void ExecuteWeavingJob(ScheduledJob job, bool lastBudgetSlot)
        {
            var before = current; var world = before.World; var workflow = before.Scheduler.Workflow!;
            ValidateWeavingBindings(world, before.Scheduler);
            if (job.DueTick < world.Tick) throw new PipelineFailureException("JOB_IN_PAST");
            var command = new CommandEnvelope("scheduler/job/" + job.Id, "ExecuteScheduledWeaving", job.ActorId.ToString(), "Scheduler",
                world.Tick.Value, world.Revision.Value, 1, new Dictionary<string, object?> { ["job_id"] = job.Id.ToString(), ["workflow_id"] = workflow.Id }, job.CorrelationId);
            var input = WithIdentityAndTick(world, world.Identity, job.DueTick);
            var evaluation = WeavingRules.EvaluateComplete(input, job.WeavingActivityId!);
            if (evaluation.ErrorCode != null) throw new PipelineFailureException("WEAVING_COMPLETION_STATE");
            var plan = evaluation.Plan ?? throw new PipelineFailureException("MISSING_WEAVING_PROPOSAL");
            ValidateWeavingPlan(input, plan, job.WeavingActivityId!, plan.Activity.Status);
            var cancelled = plan.Activity.Status == WeavingActivityStatus.Cancelled;
            if (!cancelled && plan.Activity.Status != WeavingActivityStatus.Completed) throw new PipelineFailureException("WEAVING_COMPLETION_STATE");
            var jobs = before.Scheduler.Jobs.Select(j => j.Id.Equals(job.Id) ? j.WithStatus(cancelled ? ScheduledJobStatus.Cancelled : ScheduledJobStatus.Completed) : j).ToArray();
            var status = lastBudgetSlot && jobs.Any(j => j.Status == ScheduledJobStatus.Pending && j.DueTick <= workflow.TargetTick) ? AdvanceStatus.PausedBudget : AdvanceStatus.Pending;
            var scheduler = before.Scheduler.With(jobs: jobs, workflow: workflow.With(status, job.Id, true), execution: new JobExecution(job, command.CommandId, plan.Activity.TerminalReason));
            var stage = world.Identity.BeginStage(NextRevision(world)); stage.Retire(job.WeavingActivityId!); stage.Retire(job.Id);
            CommitWeaving(before, command, command.Fingerprint(), plan, stage, scheduler, cancelled ? "ActivityCancelled" : "WeavingCompleted", true, workflow.Id, job.DueTick);
        }
        private CommandReceipt CommitWeaving(CommittedWorld before, CommandEnvelope command, string fingerprint, WeavingPlan plan,
            IdentityStage stage, SchedulerState scheduler, string type, bool terminal, string? workflowId, WorldTick? due = null)
        {
            var world = before.World; var tick = due ?? world.Tick; var revision = NextRevision(world); var activity = plan.Activity; var state = plan.Weaving; var recipe = state.Recipe;
            var facts = new List<CommittedEvent> { new CommittedEvent(stage.Allocate<Event>(), type, activity.Id.Value, tick, revision, 0,
                command.CommandId, command.CorrelationId, WeavingState.ProfileId, 1, new Dictionary<string, object?> {
                    ["activity_id"] = activity.Id.ToString(), ["actor_id"] = activity.ActorId.ToString(), ["input_lot_id"] = state.InputLotId.ToString(),
                    ["output_lot_id"] = state.OutputLotId.ToString(), ["job_id"] = activity.JobId.ToString(), ["recipe_id"] = recipe.Id.ToString(),
                    ["recipe_hash"] = recipe.Fingerprint, ["content_hash"] = state.ContentHash, ["fiber_cost"] = (long)recipe.FiberCost,
                    ["duration_ticks"] = recipe.DurationTicks, ["cord_yield"] = (long)recipe.CordYield, ["minimum_skill"] = (long)recipe.MinimumSkill,
                    ["skill_value"] = (long)state.SkillValue, ["reserved_quantity"] = activity.ReservedQuantity, ["due_tick"] = activity.DueTick.Value, ["reason"] = activity.TerminalReason }) };
            if (terminal) facts.Add(new CommittedEvent(stage.Allocate<Event>(), activity.Status == WeavingActivityStatus.Completed ? "JobCompleted" : "JobCancelled", activity.JobId.Value,
                tick, revision, 1, command.CommandId, command.CorrelationId, "scheduler-v1", 1, new Dictionary<string, object?> {
                    ["error_code"] = activity.TerminalReason, ["scheduled_by"] = activity.StartCommandId, ["workflow_id"] = workflowId, ["status"] = scheduler.Workflow?.Status.ToString() }));
            var candidate = new WorldSnapshot(stage.PrepareSnapshot(), tick, world.Actors, plan.Lots, world.RngStates, world.Repair, world.Origin, state);
            var receipt = new CommandReceipt(command, fingerprint, CommandOutcome.Accepted, null, world.Revision.Value, revision, tick, facts.Select(f => f.Id));
            Publish(before, new CommittedWorld(candidate, before.AcceptedReceipts.Concat(new[] { receipt }), before.MetadataReceipts, before.Events, scheduler, before.Journal.Events.Concat(facts)));
            return receipt;
        }
        private static bool SameWeavingActivity(WeavingActivity a, WeavingActivity b, bool pinsOnly = false)
            => a.Id.Equals(b.Id) && a.ActorId.Equals(b.ActorId) && a.JobId.Equals(b.JobId) && a.StartedTick == b.StartedTick && a.DueTick == b.DueTick &&
                a.StartCommandId == b.StartCommandId && a.CorrelationId == b.CorrelationId && (pinsOnly ||
                    (a.Status == b.Status && a.ReservedQuantity == b.ReservedQuantity && a.TerminalReason == b.TerminalReason));
        private static void ValidateWeavingPlan(WorldSnapshot world, WeavingPlan plan, EntityId<Activity> id, WeavingActivityStatus status)
        {
            var prior = world.Weaving ?? throw new PipelineFailureException("WEAVING_PROFILE_MISSING"); var state = plan.Weaving; var activity = plan.Activity;
            if (!Enum.IsDefined(typeof(WeavingActivityStatus), status) || !activity.Id.Equals(id) || activity.Status != status ||
                !SameWeavingRecipe(state.Recipe, prior.Recipe) || state.ContentHash != prior.ContentHash || state.SkillValue != prior.SkillValue ||
                !state.ActorId.Equals(prior.ActorId) || !state.Worksite.Equals(prior.Worksite) || !state.InputLotId.Equals(prior.InputLotId) || !state.OutputLotId.Equals(prior.OutputLotId) ||
                plan.Lots.Count != world.Lots.Count || plan.Lots.Select(l => l.Id).Distinct().Count() != plan.Lots.Count)
                throw new PipelineFailureException("PROPOSAL_WEAVING_EFFECT");
            var old = prior.Activities.SingleOrDefault(a => a.Id.Equals(id)); var next = state.Activities.SingleOrDefault(a => a.Id.Equals(id));
            if (next == null || !SameWeavingActivity(next, activity) ||
                (status == WeavingActivityStatus.Running ? old != null || state.Activities.Count != prior.Activities.Count + 1 :
                    old == null || old.Status != WeavingActivityStatus.Running || state.Activities.Count != prior.Activities.Count || !SameWeavingActivity(old, activity, true)))
                throw new PipelineFailureException("PROPOSAL_WEAVING_ACTIVITY");
            foreach (var other in prior.Activities.Where(a => !a.Id.Equals(id)))
                if (!state.Activities.Any(a => SameWeavingActivity(a, other))) throw new PipelineFailureException("PROPOSAL_WEAVING_ACTIVITY");
            if (status == WeavingActivityStatus.Completed && (world.Tick < old!.DueTick || WeavingRules.Eligibility(world) != null))
                throw new PipelineFailureException("PROPOSAL_WEAVING_PRECONDITION");
            var quantity = status == WeavingActivityStatus.Running ? prior.Recipe.FiberCost : old!.ReservedQuantity;
            foreach (var lot in world.Lots)
            {
                var after = plan.Lots.SingleOrDefault(l => l.Id.Equals(lot.Id));
                var input = lot.Id.Equals(prior.InputLotId); var output = lot.Id.Equals(prior.OutputLotId);
                var hand = input && status == WeavingActivityStatus.Completed ? checked(lot.OnHand - quantity) :
                    output && status == WeavingActivityStatus.Completed ? checked(lot.OnHand + prior.Recipe.CordYield) : lot.OnHand;
                var reserved = input ? checked(lot.Reserved + (status == WeavingActivityStatus.Running ? quantity : -quantity)) : lot.Reserved;
                if (after == null || !after.Commodity.Equals(lot.Commodity) || !after.Owner.Equals(lot.Owner) || !after.Location.Equals(lot.Location) ||
                    after.OnHand != hand || after.Reserved != reserved) throw new PipelineFailureException("PROPOSAL_WEAVING_MATERIAL");
            }
        }
    }
}
