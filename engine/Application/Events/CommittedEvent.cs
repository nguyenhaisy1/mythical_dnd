using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Foundation.Time;

namespace Game.Application.Events
{
    /// <summary>Immutable trusted diagnostic fact. Construction alone does not make it a journal commit.</summary>
    public sealed class CommittedEvent
    {
        public EntityId<Event> Id { get; }
        public string EventType { get; }
        public EntityId AggregateId { get; }
        public WorldTick Tick { get; }
        public ulong CommitSequence { get; }
        public uint EventIndex { get; }
        public string CausationId { get; }
        public string CorrelationId { get; }
        public string RuleVersion { get; }
        public int EnvelopeVersion { get; }
        public int PayloadVersion { get; }
        public IReadOnlyDictionary<string, object?> Payload { get; }
        public CommittedEvent(EntityId<Event> id, string eventType, EntityId aggregateId, WorldTick tick,
            ulong commitSequence, uint eventIndex, string causationId, string correlationId, string ruleVersion,
            int payloadVersion, IEnumerable<KeyValuePair<string, object?>> payload, int envelopeVersion = 1)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            AggregateId = aggregateId ?? throw new ArgumentNullException(nameof(aggregateId));
            if (string.IsNullOrWhiteSpace(eventType) || string.IsNullOrWhiteSpace(causationId) ||
                string.IsNullOrWhiteSpace(correlationId) || string.IsNullOrWhiteSpace(ruleVersion)) throw new ArgumentException("Missing event identity/version metadata");
            if (commitSequence == 0 || payloadVersion < 1 || envelopeVersion != 1) throw new ArgumentOutOfRangeException(nameof(commitSequence));
            if (Id.Value.World != AggregateId.World) throw new ArgumentException("EVENT_WORLD_MISMATCH");
            EventType = eventType; Tick = tick; CommitSequence = commitSequence; EventIndex = eventIndex;
            CausationId = causationId; CorrelationId = correlationId; RuleVersion = ruleVersion;
            PayloadVersion = payloadVersion; EnvelopeVersion = envelopeVersion;
            var copy = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var entry in payload ?? throw new ArgumentNullException(nameof(payload)))
            {
                if (!(entry.Value is null || entry.Value is string || entry.Value is bool || entry.Value is int || entry.Value is long))
                    throw new ArgumentException("Event payload requires immutable scalar values");
                copy.Add(entry.Key, entry.Value);
            }
            Payload = new ReadOnlyDictionary<string, object?>(copy);
        }
        public string Fingerprint() => CanonicalCodec.Hash(new Dictionary<string, object?> {
            ["event_id"] = Id.ToString(), ["event_type"] = EventType, ["aggregate_id"] = AggregateId.ToString(),
            ["world_tick"] = Tick.Value, ["commit_sequence"] = CommitSequence, ["event_index"] = (ulong)EventIndex,
            ["causation_id"] = CausationId, ["correlation_id"] = CorrelationId, ["rule_version"] = RuleVersion,
            ["envelope_version"] = EnvelopeVersion, ["payload_version"] = PayloadVersion, ["payload"] = Payload });
        public int ComparePosition(CommittedEvent other)
        {
            var sequence = CommitSequence.CompareTo(other.CommitSequence);
            return sequence == 0 ? EventIndex.CompareTo(other.EventIndex) : sequence;
        }
    }

    /// <summary>Immutable ordered committed facts; omniscient, trusted diagnostics only, never a player projection.</summary>
    public sealed class CommittedEventJournal
    {
        public IReadOnlyList<CommittedEvent> Events { get; }
        internal CommittedEventJournal(IEnumerable<CommittedEvent> events)
        {
            var copy = events.ToArray(); var ids = new HashSet<EntityId<Event>>();
            for (var i = 0; i < copy.Length; i++)
            {
                if (!ids.Add(copy[i].Id) || (i > 0 && (copy[i - 1].ComparePosition(copy[i]) >= 0 || copy[i - 1].Id.Value.World != copy[i].Id.Value.World || copy[i - 1].Tick.Value > copy[i].Tick.Value)))
                    throw new InvalidOperationException("EVENT_JOURNAL_ORDER_OR_ID_COLLISION");
            }
            Events = Array.AsReadOnly(copy);
        }
        public IReadOnlyList<CommittedEvent> QueryByCausation(string commandId) => Select(e => e.CausationId == commandId);
        public IReadOnlyList<CommittedEvent> QueryByCorrelation(string correlationId) => Select(e => e.CorrelationId == correlationId);
        public IReadOnlyList<CommittedEvent> QueryByAggregate(EntityId aggregateId) => Select(e => e.AggregateId.Equals(aggregateId));
        private IReadOnlyList<CommittedEvent> Select(Func<CommittedEvent, bool> predicate) => Array.AsReadOnly(Events.Where(predicate).ToArray());
    }
}
