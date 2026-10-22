using System;
using System.Collections.Generic;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Application.Events;
using Game.Application.SimulationScheduler;

namespace Game.Infrastructure.Diagnostics
{
    public sealed class CausalEdge
    {
        public string From { get; }
        public string To { get; }
        public string Kind { get; }
        public bool FromResolved { get; }
        public bool ToResolved { get; }
        internal CausalEdge(string from, string to, string kind, bool fromResolved = true, bool toResolved = true)
        { From = from; To = to; Kind = kind; FromResolved = fromResolved; ToResolved = toResolved; }
    }
    public sealed class CausalReport
    {
        public IReadOnlyList<CausalEdge> Edges { get; }
        public int TotalEdges { get; }
        public bool SourceTruncated { get; }
        public bool Truncated => SourceTruncated || TotalEdges > Edges.Count;
        internal CausalReport(IEnumerable<CausalEdge> edges, int total, bool sourceTruncated = false) { Edges = Array.AsReadOnly(edges.ToArray()); TotalEdges = total; SourceTruncated = sourceTruncated; }
    }
    /// <summary>Only persisted causation and explicit receipt/execution relationships; correlation is never an edge.</summary>
    public static class CausalInspector
    {
        public static CausalReport Inspect(CommittedWorld state, int maxEdges = 1000)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            ValidateLimit(maxEdges);
            var receipts = state.AcceptedReceipts.Concat(state.MetadataReceipts).ToArray();
            var commands = new HashSet<string>(receipts.Select(r => r.CommandId), StringComparer.Ordinal);
            var events = new HashSet<string>(state.Journal.Events.Select(e => e.Id.ToString()), StringComparer.Ordinal);
            var jobs = new HashSet<string>(state.Scheduler.Jobs.Select(j => j.Id.ToString()), StringComparer.Ordinal);
            var edges = new List<CausalEdge>();
            CausalEdge Cause(string cause, string target, string kind)
            {
                var candidates = new List<string>();
                if (commands.Contains(cause)) candidates.Add("command:" + cause);
                if (events.Contains(cause)) candidates.Add("event:" + cause);
                if (jobs.Contains(cause)) candidates.Add("job:" + cause);
                return new CausalEdge(candidates.Count == 1 ? candidates[0] : "external:" + cause, target,
                    candidates.Count > 1 ? kind + "-ambiguous" : kind, candidates.Count == 1);
            }
            foreach (var receipt in receipts)
                foreach (var id in receipt.OrderedEventIds)
                    edges.Add(new CausalEdge("command:" + receipt.CommandId, "event:" + id, "receipt-event", true, events.Contains(id.ToString())));
            foreach (var fact in state.Journal.Events) edges.Add(Cause(fact.CausationId, "event:" + fact.Id, "event-cause"));
            foreach (var job in state.Scheduler.Jobs) edges.Add(Cause(job.CausationId, "job:" + job.Id, "job-cause"));
            foreach (var execution in state.Scheduler.Executions)
                edges.Add(new CausalEdge("job:" + execution.JobId, "command:" + execution.CommandId, "job-execution", jobs.Contains(execution.JobId.ToString()), commands.Contains(execution.CommandId)));
            return Limit(edges, maxEdges);
        }
        /// <summary>Traverses supplied report only. A truncated input stays explicitly truncated because omitted edges are unknown.</summary>
        public static CausalReport Traverse(CausalReport report, string node, bool upstream, int maxEdges = 1000)
        {
            if (report == null || node == null) throw new ArgumentNullException();
            ValidateLimit(maxEdges);
            var reached = new HashSet<string>(StringComparer.Ordinal) { node };
            var queue = new Queue<string>(); queue.Enqueue(node);
            var found = new HashSet<CausalEdge>();
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var edge in report.Edges.Where(e => (upstream ? e.To : e.From) == current))
                {
                    found.Add(edge);
                    if (reached.Add(upstream ? edge.From : edge.To)) queue.Enqueue(upstream ? edge.From : edge.To);
                }
            }
            var result = Limit(found, maxEdges);
            return report.Truncated ? new CausalReport(result.Edges, result.TotalEdges, true) : result;
        }
        private static void ValidateLimit(int limit) { if (limit < 1 || limit > 100000) throw new ArgumentOutOfRangeException(nameof(limit)); }
        private static CausalReport Limit(IEnumerable<CausalEdge> values, int limit)
        {
            var ordered = values.OrderBy(e => e.From, StringComparer.Ordinal).ThenBy(e => e.To, StringComparer.Ordinal).ThenBy(e => e.Kind, StringComparer.Ordinal).ToArray();
            return new CausalReport(ordered.Take(limit), ordered.Length);
        }
    }
    public sealed class JobInspection
    {
        public ScheduledJob Job { get; }
        public JobExecution? Execution { get; }
        public CommittedEvent? TerminalEvent { get; }
        public string? CancellationReason { get; }
        public string CancellationReasonStatus { get; }
        internal JobInspection(ScheduledJob job, JobExecution? execution, CommittedEvent? terminal)
        {
            Job = job; Execution = execution; TerminalEvent = terminal;
            CancellationReason = execution?.ErrorCode;
            CancellationReasonStatus = job.Status != ScheduledJobStatus.Cancelled ? "not-applicable" : CancellationReason == null ? "not-recorded" : "recorded";
        }
    }
    public static class SchedulerInspector
    {
        public static IReadOnlyList<JobInspection> Inspect(CommittedWorld state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            return Array.AsReadOnly(state.Scheduler.Jobs.Select(job => new JobInspection(job,
                state.Scheduler.Executions.SingleOrDefault(e => e.JobId.Equals(job.Id)),
                state.Journal.Events.LastOrDefault(e => e.AggregateId.Equals(job.Id.Value) && (e.EventType == "JobCancelled" || e.EventType == "JobCompleted")))).ToArray());
        }
    }
}
