using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Game.Application.Events
{
    /// <summary>Concrete immutable numeric diagnostic projection; no authoritative world or actor knowledge.</summary>
    public sealed class ProjectionState
    {
        public IReadOnlyDictionary<string, long> Values { get; }
        public ProjectionState(IEnumerable<KeyValuePair<string, long>> values)
        {
            var copy = new Dictionary<string, long>(StringComparer.Ordinal);
            foreach (var pair in values ?? throw new ArgumentNullException(nameof(values))) copy.Add(pair.Key, pair.Value);
            Values = new ReadOnlyDictionary<string, long>(copy);
        }
        public static ProjectionState Empty => new ProjectionState(new Dictionary<string, long>());
    }
    public sealed class ConsumerPublication
    {
        public ProjectionState Projection { get; }
        public IReadOnlyDictionary<string, string> AppliedFingerprints { get; }
        public CommittedEvent? Checkpoint { get; }
        internal ConsumerPublication(ProjectionState projection, IDictionary<string, string> fingerprints, CommittedEvent? checkpoint)
        {
            Projection = projection; Checkpoint = checkpoint;
            AppliedFingerprints = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(fingerprints, StringComparer.Ordinal));
        }
    }
    /// <summary>In-memory projection+checkpoint atomicity. Proposer must be pure: external side effects cannot be rolled back.</summary>
    public sealed class InMemoryEventConsumer
    {
        private readonly object writer = new object();
        private ConsumerPublication current = new ConsumerPublication(ProjectionState.Empty, new Dictionary<string, string>(), null);
        private bool consuming;
        private readonly Action? beforeCommit;
        private readonly Action? afterCommit;
        public InMemoryEventConsumer(Action? beforeCommit = null, Action? afterCommit = null)
        { this.beforeCommit = beforeCommit; this.afterCommit = afterCommit; }
        public ConsumerPublication InspectCommitted() { lock (writer) return current; }
        public bool Consume(CommittedEvent fact, Func<ProjectionState, CommittedEvent, ProjectionState> propose)
        {
            if (fact == null || propose == null) throw new ArgumentNullException();
            lock (writer)
            {
                if (consuming) throw new InvalidOperationException("REENTRANT_CONSUMER");
                consuming = true;
                try
                {
                    if (current.Checkpoint != null && current.Checkpoint.Id.Value.World != fact.Id.Value.World)
                        throw new InvalidOperationException("EVENT_WORLD_MISMATCH");
                    var fingerprint = fact.Fingerprint();
                    if (current.AppliedFingerprints.TryGetValue(fact.Id.ToString(), out var original))
                    {
                        if (original != fingerprint) throw new InvalidOperationException("EVENT_IDENTITY_COLLISION");
                        return false;
                    }
                    if (current.Checkpoint != null && (current.Checkpoint.ComparePosition(fact) >= 0 || current.Checkpoint.Tick.Value > fact.Tick.Value))
                        throw new InvalidOperationException("EVENT_OUT_OF_ORDER");
                    var proposal = propose(current.Projection, fact) ?? throw new InvalidOperationException("MISSING_PROJECTION");
                    var applied = new Dictionary<string, string>(StringComparer.Ordinal);
                    foreach (var pair in current.AppliedFingerprints) applied.Add(pair.Key, pair.Value);
                    applied.Add(fact.Id.ToString(), fingerprint);
                    var candidate = new ConsumerPublication(proposal, applied, fact);
                    beforeCommit?.Invoke();
                    current = candidate;
                    afterCommit?.Invoke();
                    return true;
                }
                finally { consuming = false; }
            }
        }
    }
}
