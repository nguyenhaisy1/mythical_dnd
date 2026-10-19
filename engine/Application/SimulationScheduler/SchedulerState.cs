using System;
using System.Collections.Generic;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Time;

namespace Game.Application.SimulationScheduler
{
    public enum SchedulerPhase { Boundary, DueObligation, Environment, ProductionHealth, ActorDecisions, InformationNarrative, ValidationProjection }
    public enum ScheduledJobStatus { Pending, Completed, Cancelled }
    public enum AdvanceStatus { Pending, PausedBudget, PausedDecision, Completed }

    /// <summary>Approved compiled continuation can only propose transfers, follow-ups and a decision stop.</summary>
    public sealed class JobProposal
    {
        public WorldTick DueTick { get; }
        public SchedulerPhase Phase { get; }
        public int Priority { get; }
        public TransferResource Transfer { get; }
        public string HandlerKey { get; }
        public JobProposal(WorldTick dueTick, SchedulerPhase phase, int priority, TransferResource transfer, string handlerKey = "transfer-v1")
        {
            if (!Enum.IsDefined(typeof(SchedulerPhase), phase)) throw new ArgumentOutOfRangeException(nameof(phase));
            DueTick = dueTick; Phase = phase; Priority = priority;
            Transfer = transfer ?? throw new ArgumentNullException(nameof(transfer));
            if (string.IsNullOrWhiteSpace(handlerKey)) throw new ArgumentException("Missing handler key");
            HandlerKey = handlerKey;
        }
    }
    public sealed class JobContinuation
    {
        public IReadOnlyList<JobProposal> FollowUps { get; }
        public bool RequiresDecision { get; }
        public JobContinuation(IEnumerable<JobProposal>? followUps = null, bool requiresDecision = false)
        {
            var values = (followUps ?? Array.Empty<JobProposal>()).ToArray();
            if (values.Any(x => x == null)) throw new ArgumentException("Null follow-up");
            FollowUps = Array.AsReadOnly(values); RequiresDecision = requiresDecision;
        }
    }
    public interface IJobContinuationHandler
    {
        string Key { get; }
        JobContinuation Evaluate(WorldSnapshot world, ScheduledJob job);
    }
    public sealed class JobHandlerRegistry
    {
        private readonly Dictionary<string, IJobContinuationHandler> handlers;
        public JobHandlerRegistry(IEnumerable<IJobContinuationHandler>? approvedHandlers = null)
        {
            handlers = new Dictionary<string, IJobContinuationHandler>(StringComparer.Ordinal);
            foreach (var handler in approvedHandlers ?? Array.Empty<IJobContinuationHandler>())
            {
                if (handler == null || string.IsNullOrWhiteSpace(handler.Key) || handler.Key == "transfer-v1") throw new ArgumentException("Invalid handler registration");
                handlers.Add(handler.Key, handler);
            }
        }
        public bool Contains(string key) => key == "transfer-v1" || handlers.ContainsKey(key);
        internal JobContinuation Evaluate(WorldSnapshot world, ScheduledJob job) => job.HandlerKey == "transfer-v1"
            ? new JobContinuation() : handlers[job.HandlerKey].Evaluate(world, job) ?? throw new PipelineFailureException("MISSING_JOB_PROPOSAL");
    }
    public sealed class ScheduledJob
    {
        public EntityId<Job> Id { get; }
        public EntityId<Actor> ActorId { get; }
        public WorldTick DueTick { get; }
        public SchedulerPhase Phase { get; }
        public int Priority { get; }
        public ulong CreationSequence { get; }
        public TransferResource Transfer { get; }
        public string HandlerKey { get; }
        public string CausationId { get; }
        public string CorrelationId { get; }
        public WorldTick? DeferredFromTick { get; }
        public ScheduledJobStatus Status { get; }
        public IReadOnlyList<string> AncestorKeys { get; }
        public ScheduledJob(EntityId<Job> id, EntityId<Actor> actorId, WorldTick dueTick, SchedulerPhase phase,
            int priority, ulong creationSequence, TransferResource transfer, string causationId, string correlationId,
            string handlerKey = "transfer-v1", WorldTick? deferredFromTick = null,
            ScheduledJobStatus status = ScheduledJobStatus.Pending, IEnumerable<string>? ancestorKeys = null)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id)); ActorId = actorId ?? throw new ArgumentNullException(nameof(actorId));
            if (!Enum.IsDefined(typeof(SchedulerPhase), phase) || !Enum.IsDefined(typeof(ScheduledJobStatus), status) || creationSequence == 0) throw new ArgumentException("Invalid job metadata");
            if (string.IsNullOrWhiteSpace(causationId) || string.IsNullOrWhiteSpace(correlationId) || string.IsNullOrWhiteSpace(handlerKey)) throw new ArgumentException("Missing job metadata");
            DueTick = dueTick; Phase = phase; Priority = priority; CreationSequence = creationSequence;
            Transfer = transfer ?? throw new ArgumentNullException(nameof(transfer)); CausationId = causationId;
            CorrelationId = correlationId; HandlerKey = handlerKey; DeferredFromTick = deferredFromTick; Status = status;
            AncestorKeys = Array.AsReadOnly((ancestorKeys ?? Array.Empty<string>()).ToArray());
        }
        internal ScheduledJob WithStatus(ScheduledJobStatus status) => new ScheduledJob(Id, ActorId, DueTick, Phase, Priority,
            CreationSequence, Transfer, CausationId, CorrelationId, HandlerKey, DeferredFromTick, status, AncestorKeys);
    }
    public sealed class AdvanceWorkflow
    {
        public string Id { get; }
        public EntityId<Actor> OwnerId { get; }
        public CommandSource OwnerSource { get; }
        public WorldTick TargetTick { get; }
        public AdvanceStatus Status { get; }
        public EntityId<Job>? Cursor { get; }
        public ulong CommittedJobs { get; }
        public string CorrelationId { get; }
        internal AdvanceWorkflow(string id, EntityId<Actor> owner, CommandSource source, WorldTick target,
            AdvanceStatus status, EntityId<Job>? cursor, ulong committedJobs, string correlationId)
        { Id = id; OwnerId = owner; OwnerSource = source; TargetTick = target; Status = status; Cursor = cursor; CommittedJobs = committedJobs; CorrelationId = correlationId; }
        internal AdvanceWorkflow With(AdvanceStatus status, EntityId<Job>? cursor = null, bool jobCommitted = false)
            => new AdvanceWorkflow(Id, OwnerId, OwnerSource, TargetTick, status, status == AdvanceStatus.Completed ? null : cursor ?? Cursor,
                jobCommitted ? checked(CommittedJobs + 1) : CommittedJobs, CorrelationId);
    }
    public sealed class JobExecution
    {
        public EntityId<Job> JobId { get; }
        public WorldTick Tick { get; }
        public string CommandId { get; }
        public string? ErrorCode { get; }
        internal JobExecution(ScheduledJob job, string commandId, string? error) { JobId = job.Id; Tick = job.DueTick; CommandId = commandId; ErrorCode = error; }
    }
    /// <summary>Immutable aggregate member; Restore is trusted composition only, not a persistence implementation.</summary>
    public sealed class SchedulerState
    {
        public IReadOnlyList<ScheduledJob> Jobs { get; }
        public ulong NextCreationSequence { get; }
        public bool SequenceExhausted { get; }
        public IReadOnlyList<AdvanceWorkflow> Workflows { get; }
        public AdvanceWorkflow? Workflow => Workflows.LastOrDefault();
        public IReadOnlyList<JobExecution> Executions { get; }
        public static SchedulerState Empty { get; } = new SchedulerState(Array.Empty<ScheduledJob>(), 1, false, Array.Empty<AdvanceWorkflow>(), Array.Empty<JobExecution>());
        public static SchedulerState Restore(IEnumerable<ScheduledJob> jobs, ulong nextCreationSequence, bool sequenceExhausted = false)
            => new SchedulerState(jobs, nextCreationSequence, sequenceExhausted, Array.Empty<AdvanceWorkflow>(), Array.Empty<JobExecution>());
        internal SchedulerState(IEnumerable<ScheduledJob> jobs, ulong next, bool exhausted, IEnumerable<AdvanceWorkflow> workflows, IEnumerable<JobExecution> executions)
        {
            var values = jobs.ToArray();
            if (next == 0 || (exhausted && next != ulong.MaxValue) || values.Any(x => x == null || (!exhausted && x.CreationSequence >= next)) ||
                values.Select(x => x.Id).Distinct().Count() != values.Length || values.Select(x => x.CreationSequence).Distinct().Count() != values.Length)
                throw new ArgumentException("Invalid scheduler identity/sequence");
            Jobs = Array.AsReadOnly(values.OrderBy(j => j.DueTick.Value).ThenBy(j => j.Phase).ThenBy(j => j.Priority).ThenBy(j => j.CreationSequence).ThenBy(j => j.Id.Value).ToArray());
            NextCreationSequence = next; SequenceExhausted = exhausted;
            Workflows = Array.AsReadOnly(workflows.ToArray()); Executions = Array.AsReadOnly(executions.ToArray());
        }
        internal SchedulerState With(IEnumerable<ScheduledJob>? jobs = null, ulong? next = null, bool? exhausted = null,
            AdvanceWorkflow? workflow = null, JobExecution? execution = null)
        {
            var workflows = workflow == null ? Workflows : Workflows.Where(w => w.Id != workflow.Id).Concat(new[] { workflow });
            return new SchedulerState(jobs ?? Jobs, next ?? NextCreationSequence, exhausted ?? SequenceExhausted, workflows,
                execution == null ? Executions : Executions.Concat(new[] { execution }));
        }
    }
}
