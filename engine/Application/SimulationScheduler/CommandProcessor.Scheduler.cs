using System;
using System.Collections.Generic;
using System.Linq;
using Game.Application.Events;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Foundation.Time;

namespace Game.Application.CommandPipeline
{
    public sealed partial class CommandProcessor
    {
        public const int MaximumJobsPerInvocation = 1000;
        public const int MaximumFollowUps = 32;
        public const int MaximumCausalDepth = 64;
        private readonly JobHandlerRegistry jobHandlers;
        private sealed class SchedulerInputException : Exception
        {
            public string Code { get; }
            public SchedulerInputException(string code) { Code = code; }
        }
        // Convert only explicit schema/admission failures, never rule or publication callback failures.
        private static T Input<T>(Func<T> parse)
        {
            try { return parse(); }
            catch (IdentityException error) { throw new SchedulerInputException(error.Code); }
            catch (TimeRangeException) { throw new SchedulerInputException("TIME_RANGE"); }
            catch (Exception error) when (error is ArgumentException || error is OverflowException || error is FormatException)
            { throw new SchedulerInputException("INVALID_PAYLOAD"); }
        }
        private static bool IsSchedulerCommand(string type) => type == "ScheduleTransfer" || type == "CancelJob" || type == "AdvanceTime" || type == "ResolveAdvanceDecision";
        private void ValidateInitialScheduler(WorldSnapshot world, SchedulerState scheduler, bool restoreHistory = false)
        {
            foreach (var job in scheduler.Jobs)
            {
                if ((!restoreHistory && job.Status != ScheduledJobStatus.Pending) || world.Identity.Resolve<Job>(job.Id.Value).Lifecycle != (job.Status == ScheduledJobStatus.Pending ? EntityLifecycle.Active : EntityLifecycle.Retired) || (job.Status == ScheduledJobStatus.Pending && job.DueTick < world.Tick))
                    throw new ArgumentException("Invalid restored job lifecycle/time");
                ValidateJobReferences(world, job.ActorId, job.Transfer, job.HandlerKey);
                if (job.AncestorKeys.Count > MaximumCausalDepth || job.AncestorKeys.Any(string.IsNullOrWhiteSpace) ||
                    job.AncestorKeys.Distinct(StringComparer.Ordinal).Count() != job.AncestorKeys.Count ||
                    job.AncestorKeys.Contains(ReactionKey(job.ActorId, job.Transfer, job.HandlerKey))) throw new ArgumentException("Invalid restored causality");
                if (job.DeferredFromTick.HasValue && (job.DeferredFromTick.Value.Value == long.MaxValue || job.DueTick.Value != job.DeferredFromTick.Value.Value + 1))
                    throw new ArgumentException("Invalid deferral trace");
            }
        }
        private void ValidateJobReferences(WorldSnapshot world, EntityId<Actor> actor, TransferResource transfer, string handlerKey)
        {
            world.GetActor(actor); world.GetLot(transfer.SourceLotId); world.GetLot(transfer.DestinationLotId);
            if (transfer.Quantity <= 0) throw new ArgumentException("Invalid transfer quantity");
            if (!jobHandlers.Contains(handlerKey)) throw new ArgumentException("Unapproved job handler");
        }
        private static string ReactionKey(EntityId<Actor> actor, TransferResource transfer, string handlerKey)
            => CanonicalCodec.Hash(new Dictionary<string, object?> { ["handler"] = handlerKey, ["actor"] = actor.ToString(),
                ["source"] = transfer.SourceLotId.ToString(), ["destination"] = transfer.DestinationLotId.ToString(), ["quantity"] = transfer.Quantity });
        private static long Number(IReadOnlyDictionary<string, object?> payload, string key)
        {
            if (!payload.TryGetValue(key, out var value) || !(value is long || value is int)) throw new SchedulerInputException("INVALID_PAYLOAD");
            return Convert.ToInt64(value);
        }
        private static string Text(IReadOnlyDictionary<string, object?> payload, string key)
        {
            if (!payload.TryGetValue(key, out var value) || !(value is string text) || string.IsNullOrWhiteSpace(text)) throw new SchedulerInputException("INVALID_PAYLOAD");
            return text;
        }
        private static ulong NextRevision(WorldSnapshot world)
        {
            if (world.Revision.Value == ulong.MaxValue) throw new PipelineFailureException("REVISION_EXHAUSTED");
            return world.Revision.Value + 1;
        }
        private static WorldSnapshot WithIdentityAndTick(WorldSnapshot world, IdentitySnapshot identity, WorldTick tick, IEnumerable<ResourceLot>? lots = null)
            => new WorldSnapshot(identity, tick, world.Actors, lots ?? world.Lots, world.RngStates);
        private void Publish(CommittedWorld before, CommittedWorld publication)
        {
            beforeCommit?.Invoke();
            if (!ReferenceEquals(current, before)) throw new PipelineFailureException("COMMIT_CONFLICT");
            if (store != null)
            {
                try { store.Commit(before, publication); }
                catch { NeedsRecovery = true; throw; }
            }
            current = publication;
            afterCommit?.Invoke();
        }
        private CommandReceipt CommitScheduler(CommandEnvelope command, string fingerprint, SchedulerState scheduler, IdentityStage stage,
            WorldTick tick, CommandOutcome outcome, string eventType, EntityId aggregate, string? detail = null)
        {
            var before = current; var revision = NextRevision(before.World); var eventId = stage.Allocate<Event>();
            var fact = new CommittedEvent(eventId, eventType, aggregate, tick, revision, 0, command.CommandId,
                command.CorrelationId, "scheduler-v1", 1, new Dictionary<string, object?> { ["detail"] = detail });
            var world = WithIdentityAndTick(before.World, stage.PrepareSnapshot(), tick);
            var receipt = new CommandReceipt(command, fingerprint, outcome, null, before.World.Revision.Value, revision, tick, new[] { eventId });
            Publish(before, new CommittedWorld(world, before.AcceptedReceipts.Concat(new[] { receipt }), before.MetadataReceipts,
                before.Events, scheduler, before.Journal.Events.Concat(new[] { fact })));
            return receipt;
        }
        private CommandReceipt ExecuteScheduler(CommandEnvelope envelope, ExecutionPrincipal principal, string fingerprint)
        {
            if (envelope.PayloadVersion != 1) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "PAYLOAD_VERSION_UNSUPPORTED");
            try
            {
                switch (envelope.CommandType)
                {
                    case "ScheduleTransfer": return Schedule(envelope, principal, fingerprint);
                    case "CancelJob": return Cancel(envelope, principal, fingerprint);
                    case "AdvanceTime": return BeginAdvance(envelope, principal, fingerprint);
                    default: return ResolveDecision(envelope, principal, fingerprint);
                }
            }
            catch (SchedulerInputException error) { return Metadata(envelope, fingerprint, CommandOutcome.Rejected, error.Code); }
        }
        private CommandReceipt Schedule(CommandEnvelope envelope, ExecutionPrincipal principal, string fingerprint)
        {
            var before = current; var world = before.World; var payload = envelope.Payload;
            var handler = payload.ContainsKey("handler_key") ? Text(payload, "handler_key") : "transfer-v1";
            if (payload.Count != (payload.ContainsKey("handler_key") ? 7 : 6)) throw new SchedulerInputException("INVALID_PAYLOAD");
            var due = Input(() => new WorldTick(Number(payload, "due_tick")));
            if (due < world.Tick) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "JOB_IN_PAST");
            var phase = Input(() => (SchedulerPhase)checked((int)Number(payload, "phase")));
            if (!Enum.IsDefined(typeof(SchedulerPhase), phase)) throw new SchedulerInputException("INVALID_PAYLOAD");
            var priority = Input(() => checked((int)Number(payload, "priority")));
            var transfer = Input(() => new TransferResource(EntityId<Lot>.Parse(Text(payload, "source_lot_id")), EntityId<Lot>.Parse(Text(payload, "destination_lot_id")), Number(payload, "quantity")));
            Input(() => { ValidateJobReferences(world, principal.ActorId, transfer, handler); return true; });
            var evaluation = rules.Evaluate(world, principal.ActorId, transfer);
            if (evaluation.ErrorCode != null) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, evaluation.ErrorCode);
            if (evaluation.Changes == null) throw new PipelineFailureException("MISSING_PROPOSAL");
            ValidateTransfer(world, principal.ActorId, transfer, evaluation.Changes);
            if (before.Scheduler.SequenceExhausted) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "SEQUENCE_EXHAUSTED");
            var sequence = before.Scheduler.NextCreationSequence;
            var stage = world.Identity.BeginStage(NextRevision(world)); var id = stage.Allocate<Job>();
            var job = new ScheduledJob(id, principal.ActorId, due, phase, priority, sequence, transfer, envelope.CommandId, envelope.CorrelationId, handler);
            var scheduler = before.Scheduler.With(jobs: before.Scheduler.Jobs.Concat(new[] { job }),
                next: sequence == ulong.MaxValue ? sequence : sequence + 1, exhausted: sequence == ulong.MaxValue);
            return CommitScheduler(envelope, fingerprint, scheduler, stage, world.Tick, CommandOutcome.Accepted, "JobScheduled", id.Value);
        }
        private CommandReceipt Cancel(CommandEnvelope envelope, ExecutionPrincipal principal, string fingerprint)
        {
            if (envelope.Payload.Count != 1) throw new SchedulerInputException("INVALID_PAYLOAD");
            var id = Input(() => EntityId<Job>.Parse(Text(envelope.Payload, "job_id")));
            Input(() => current.World.Identity.Resolve<Job>(id.Value));
            var job = current.Scheduler.Jobs.FirstOrDefault(j => j.Id.Equals(id));
            if (job == null) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "REF_NOT_FOUND");
            if (!job.ActorId.Equals(principal.ActorId)) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "NOT_AUTHORIZED");
            if (job.Status != ScheduledJobStatus.Pending) return Metadata(envelope, fingerprint, CommandOutcome.NoOp, null);
            var stage = current.World.Identity.BeginStage(NextRevision(current.World)); stage.Retire(id);
            var scheduler = current.Scheduler.With(jobs: current.Scheduler.Jobs.Select(j => j.Id.Equals(id) ? j.WithStatus(ScheduledJobStatus.Cancelled) : j));
            return CommitScheduler(envelope, fingerprint, scheduler, stage, current.World.Tick, CommandOutcome.Accepted, "JobCancelled", id.Value);
        }
        private CommandReceipt BeginAdvance(CommandEnvelope envelope, ExecutionPrincipal principal, string fingerprint)
        {
            if (envelope.Payload.Count != 1) throw new SchedulerInputException("INVALID_PAYLOAD");
            var world = current.World; var actor = Input(() => world.GetActor(principal.ActorId));
            if (!actor.Alive) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "ACTOR_DEAD");
            var target = Input(() => new WorldTick(Number(envelope.Payload, "target_tick")));
            if (target < world.Tick) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "TIME_RANGE");
            var hasJobs = current.Scheduler.Jobs.Any(j => j.Status == ScheduledJobStatus.Pending && j.DueTick <= target);
            if (!hasJobs && target == world.Tick) return Metadata(envelope, fingerprint, CommandOutcome.NoOp, null);
            var workflow = new AdvanceWorkflow(envelope.CommandId, principal.ActorId, principal.Source, target,
                hasJobs ? AdvanceStatus.Pending : AdvanceStatus.Completed, null, 0, envelope.CorrelationId);
            return CommitScheduler(envelope, fingerprint, current.Scheduler.With(workflow: workflow), world.Identity.BeginStage(NextRevision(world)),
                hasJobs ? world.Tick : target, hasJobs ? CommandOutcome.AcceptedPending : CommandOutcome.Accepted,
                hasJobs ? "AdvanceRequested" : "AdvanceCompleted", principal.ActorId.Value);
        }
        private CommandReceipt ResolveDecision(CommandEnvelope envelope, ExecutionPrincipal principal, string fingerprint)
        {
            if (envelope.Payload.Count != 1) throw new SchedulerInputException("INVALID_PAYLOAD");
            var id = Text(envelope.Payload, "workflow_id"); var workflow = current.Scheduler.Workflow;
            if (workflow == null || workflow.Id != id || workflow.Status != AdvanceStatus.PausedDecision)
                return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "DECISION_NOT_PENDING");
            if (!Owns(workflow, principal)) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "NOT_AUTHORIZED");
            return CommitScheduler(envelope, fingerprint, current.Scheduler.With(workflow: workflow.With(AdvanceStatus.Pending)),
                current.World.Identity.BeginStage(NextRevision(current.World)), current.World.Tick, CommandOutcome.Accepted,
                "AdvanceDecisionResolved", principal.ActorId.Value);
        }
        private static bool Owns(AdvanceWorkflow workflow, ExecutionPrincipal principal) => workflow.OwnerId.Equals(principal.ActorId) && workflow.OwnerSource == principal.Source;

        /// <summary>Trusted host drain. Each job/closing step publishes once; exceptions preserve the last committed cursor.</summary>
        public AdvanceWorkflow DrainAdvance(ExecutionPrincipal principal, int budget = MaximumJobsPerInvocation)
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));
            if (budget < 1 || budget > MaximumJobsPerInvocation) throw new ArgumentOutOfRangeException(nameof(budget));
            lock (writer)
            {
                if (NeedsRecovery) throw new PipelineFailureException("PERSISTENCE_RECOVERY_REQUIRED");
                if (executing) throw new PipelineFailureException("REENTRANT_COMMAND");
                executing = true;
                try
                {
                    var workflow = current.Scheduler.Workflow ?? throw new PipelineFailureException("NO_ADVANCE_WORKFLOW");
                    if (!Owns(workflow, principal)) throw new PipelineFailureException("NOT_AUTHORIZED");
                    if (workflow.Status == AdvanceStatus.Completed || workflow.Status == AdvanceStatus.PausedDecision) return workflow;
                    for (var count = 0; count < budget; count++)
                    {
                        var job = NextJob(current.Scheduler, workflow.TargetTick);
                        if (job == null) return CloseAdvance();
                        ExecuteJob(job, count + 1 == budget);
                        workflow = current.Scheduler.Workflow!;
                        if (workflow.Status == AdvanceStatus.PausedDecision || workflow.Status == AdvanceStatus.PausedBudget) return workflow;
                    }
                    return CloseAdvance();
                }
                finally { executing = false; }
            }
        }
        private static ScheduledJob? NextJob(SchedulerState scheduler, WorldTick target)
            => scheduler.Jobs.FirstOrDefault(j => j.Status == ScheduledJobStatus.Pending && j.DueTick <= target);
        private CommandEnvelope InternalCommand(string id, string type, AdvanceWorkflow workflow)
            => new CommandEnvelope(id, type, workflow.OwnerId.ToString(), "Scheduler", current.World.Tick.Value,
                current.World.Revision.Value, 1, Array.Empty<KeyValuePair<string, object?>>(), workflow.CorrelationId);
        private AdvanceWorkflow CloseAdvance()
        {
            var workflow = current.Scheduler.Workflow!; var command = InternalCommand("scheduler/close/" + CanonicalCodec.Hash(workflow.Id), "CloseAdvance", workflow);
            var completed = workflow.With(AdvanceStatus.Completed);
            CommitScheduler(command, command.Fingerprint(), current.Scheduler.With(workflow: completed),
                current.World.Identity.BeginStage(NextRevision(current.World)), workflow.TargetTick, CommandOutcome.Accepted,
                "AdvanceCompleted", workflow.OwnerId.Value);
            return completed;
        }
        private void ExecuteJob(ScheduledJob job, bool lastBudgetSlot)
        {
            var before = current; var world = before.World; var workflow = before.Scheduler.Workflow!;
            if (job.DueTick < world.Tick) throw new PipelineFailureException("JOB_IN_PAST");
            var command = new CommandEnvelope("scheduler/job/" + job.Id, "ExecuteScheduledTransfer", job.ActorId.ToString(), "Scheduler",
                world.Tick.Value, world.Revision.Value, 1, new Dictionary<string, object?> { ["job_id"] = job.Id.ToString(), ["workflow_id"] = workflow.Id }, job.CorrelationId);
            var revision = NextRevision(world); var stage = world.Identity.BeginStage(revision);
            // Due-tick rule input is immutable and never published separately from the resulting effects.
            var input = WithIdentityAndTick(world, world.Identity, job.DueTick);
            var result = rules.Evaluate(input, job.ActorId, job.Transfer);
            JobContinuation continuation = new JobContinuation();
            var lots = world.Lots.AsEnumerable();
            if (result.ErrorCode == null)
            {
                if (result.Changes == null) throw new PipelineFailureException("MISSING_PROPOSAL");
                ValidateTransfer(input, job.ActorId, job.Transfer, result.Changes);
                var updates = result.Changes.LotChanges.ToDictionary(c => c.LotId, c => c.NewOnHand);
                lots = world.Lots.Select(l => updates.TryGetValue(l.Id, out var quantity) ? new ResourceLot(l.Id, l.Commodity, l.Owner, l.Location, quantity, l.Reserved) : l).ToArray();
                var proposedWorld = WithIdentityAndTick(world, world.Identity, job.DueTick, lots);
                continuation = jobHandlers.Evaluate(proposedWorld, job);
            }
            if (continuation.FollowUps.Count > MaximumFollowUps) throw new PipelineFailureException("JOB_FANOUT_LIMIT");
            var jobs = before.Scheduler.Jobs.Select(j => j.Id.Equals(job.Id)
                ? j.WithStatus(result.ErrorCode == null ? ScheduledJobStatus.Completed : ScheduledJobStatus.Cancelled) : j).ToList();
            var next = before.Scheduler.NextCreationSequence; var exhausted = before.Scheduler.SequenceExhausted;
            var ancestors = job.AncestorKeys.Concat(new[] { ReactionKey(job.ActorId, job.Transfer, job.HandlerKey) }).ToArray();
            foreach (var proposal in continuation.FollowUps)
            {
                if (ancestors.Length > MaximumCausalDepth) throw new PipelineFailureException("CAUSAL_DEPTH_LIMIT");
                ValidateJobReferences(input, job.ActorId, proposal.Transfer, proposal.HandlerKey);
                if (!world.GetActor(job.ActorId).Controls(world.GetLot(proposal.Transfer.SourceLotId).Owner)) throw new PipelineFailureException("PROPOSAL_AUTHORITY");
                if (ancestors.Contains(ReactionKey(job.ActorId, proposal.Transfer, proposal.HandlerKey), StringComparer.Ordinal)) throw new PipelineFailureException("CAUSAL_CYCLE");
                if (proposal.DueTick < job.DueTick) throw new PipelineFailureException("JOB_IN_PAST");
                var due = proposal.DueTick; WorldTick? deferred = null;
                if (due == job.DueTick && proposal.Phase <= job.Phase)
                {
                    if (due.Value == long.MaxValue) throw new PipelineFailureException("TIME_RANGE");
                    deferred = due; due = due.Add(1);
                }
                if (exhausted) throw new PipelineFailureException("SEQUENCE_EXHAUSTED");
                var id = stage.Allocate<Job>();
                jobs.Add(new ScheduledJob(id, job.ActorId, due, proposal.Phase, proposal.Priority, next, proposal.Transfer,
                    command.CommandId, job.CorrelationId, proposal.HandlerKey, deferred, ancestorKeys: ancestors));
                if (next == ulong.MaxValue) exhausted = true; else next++;
            }
            stage.Retire(job.Id);
            var status = continuation.RequiresDecision ? AdvanceStatus.PausedDecision
                : lastBudgetSlot && jobs.Any(j => j.Status == ScheduledJobStatus.Pending && j.DueTick <= workflow.TargetTick)
                    ? AdvanceStatus.PausedBudget : AdvanceStatus.Pending;
            var scheduler = before.Scheduler.With(jobs, next, exhausted, workflow.With(status, job.Id, true), new JobExecution(job, command.CommandId, result.ErrorCode));
            var newFacts = new List<CommittedEvent>(); var transfers = before.Events.AsEnumerable();
            if (result.ErrorCode == null && !result.IsNoOp)
            {
                var transfer = new CommittedTransfer(stage.Allocate<Event>(), revision, job.DueTick, command, job.Transfer);
                newFacts.Add(transfer.Envelope); transfers = transfers.Concat(new[] { transfer });
            }
            var terminalId = stage.Allocate<Event>();
            newFacts.Add(new CommittedEvent(terminalId, result.ErrorCode == null ? "JobCompleted" : "JobCancelled", job.Id.Value,
                job.DueTick, revision, (uint)newFacts.Count, command.CommandId, job.CorrelationId, "scheduler-v1", 1,
                new Dictionary<string, object?> { ["error_code"] = result.ErrorCode, ["scheduled_by"] = job.CausationId, ["workflow_id"] = workflow.Id, ["status"] = status.ToString() }));
            var receipt = new CommandReceipt(command, command.Fingerprint(), CommandOutcome.Accepted, null,
                world.Revision.Value, revision, job.DueTick, newFacts.Select(f => f.Id));
            var candidate = WithIdentityAndTick(world, stage.PrepareSnapshot(), job.DueTick, lots);
            Publish(before, new CommittedWorld(candidate, before.AcceptedReceipts.Concat(new[] { receipt }), before.MetadataReceipts,
                transfers, scheduler, before.Journal.Events.Concat(newFacts)));
        }
    }
}
