using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Foundation.Time;

namespace Game.Application.CommandPipeline
{
    public enum CommandSource { Player, Npc, Scheduler }
    public enum CommandOutcome { Accepted, NoOp, Rejected }

    /// <summary>Created by a trusted host after authentication, never deserialized from command data.</summary>
    public sealed class ExecutionPrincipal
    {
        public EntityId<Actor> ActorId { get; }
        public CommandSource Source { get; }
        public ExecutionPrincipal(EntityId<Actor> actorId, CommandSource source)
        {
            ActorId = actorId ?? throw new ArgumentNullException(nameof(actorId));
            if (!Enum.IsDefined(typeof(CommandSource), source)) throw new ArgumentOutOfRangeException(nameof(source));
            Source = source;
        }
    }

    /// <summary>Immutable wire envelope. Payload v1 permits only scalar fields; unsupported schemas are rejected.</summary>
    public sealed class CommandEnvelope
    {
        public string CommandId { get; }
        public string CommandType { get; }
        public string IssuerId { get; }
        public string SourceType { get; }
        public long WorldTick { get; }
        public ulong ExpectedRevision { get; }
        public int PayloadVersion { get; }
        public IReadOnlyDictionary<string, object?> Payload { get; }
        public string CorrelationId { get; }
        public CommandEnvelope(string commandId, string commandType, string issuerId, string sourceType,
            long worldTick, ulong expectedRevision, int payloadVersion, IEnumerable<KeyValuePair<string, object?>> payload, string correlationId)
        {
            CommandId = commandId; CommandType = commandType; IssuerId = issuerId; SourceType = sourceType;
            WorldTick = worldTick; ExpectedRevision = expectedRevision; PayloadVersion = payloadVersion; CorrelationId = correlationId;
            var copied = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var pair in payload ?? throw new ArgumentNullException(nameof(payload)))
            {
                if (!(pair.Value is null || pair.Value is string || pair.Value is bool || pair.Value is long || pair.Value is int))
                    throw new ArgumentException("Envelope payload v1 requires immutable scalar values");
                copied.Add(pair.Key, pair.Value);
            }
            Payload = new ReadOnlyDictionary<string, object?>(copied);
        }
        public string Fingerprint() => CanonicalCodec.Hash(new Dictionary<string, object?> {
            ["command_id"] = CommandId, ["command_type"] = CommandType, ["issuer_id"] = IssuerId,
            ["source_type"] = SourceType, ["world_tick"] = WorldTick, ["expected_revision"] = ExpectedRevision,
            ["payload_version"] = PayloadVersion, ["payload"] = Payload, ["correlation_id"] = CorrelationId });
    }

    public sealed class CommandReceipt
    {
        public string CommandId { get; }
        public string Fingerprint { get; }
        public CommandOutcome Outcome { get; }
        public string? ErrorCode { get; }
        public ulong BeforeRevision { get; }
        public ulong AfterRevision { get; }
        public WorldTick CommittedTick { get; }
        public IReadOnlyList<EntityId<Event>> OrderedEventIds { get; }
        internal CommandReceipt(CommandEnvelope envelope, string fingerprint, CommandOutcome outcome, string? error,
            ulong before, ulong after, WorldTick tick, IEnumerable<EntityId<Event>> events)
        {
            CommandId = envelope.CommandId; Fingerprint = fingerprint; Outcome = outcome; ErrorCode = error;
            BeforeRevision = before; AfterRevision = after; CommittedTick = tick;
            OrderedEventIds = Array.AsReadOnly(events.ToArray());
        }
    }

    /// <summary>Minimal committed fact for this transfer slice; not the FND-09 journal/event schema.</summary>
    public sealed class CommittedTransfer
    {
        public EntityId<Event> Id { get; }
        public ulong Revision { get; }
        public WorldTick Tick { get; }
        public string CommandId { get; }
        public string CorrelationId { get; }
        public EntityId<Lot> SourceLotId { get; }
        public EntityId<Lot> DestinationLotId { get; }
        public long Quantity { get; }
        internal CommittedTransfer(EntityId<Event> id, ulong revision, WorldTick tick, CommandEnvelope command, TransferResource transfer)
        {
            Id = id; Revision = revision; Tick = tick; CommandId = command.CommandId; CorrelationId = command.CorrelationId;
            SourceLotId = transfer.SourceLotId; DestinationLotId = transfer.DestinationLotId; Quantity = transfer.Quantity;
        }
    }

    /// <summary>One immutable publication unit. Metadata receipts do not alter World or accepted history.</summary>
    public sealed class CommittedWorld
    {
        public WorldSnapshot World { get; }
        public IReadOnlyList<CommandReceipt> AcceptedReceipts { get; }
        public IReadOnlyList<CommandReceipt> MetadataReceipts { get; }
        public IReadOnlyList<CommittedTransfer> Events { get; }
        internal CommittedWorld(WorldSnapshot world, IEnumerable<CommandReceipt> accepted,
            IEnumerable<CommandReceipt> metadata, IEnumerable<CommittedTransfer> events)
        {
            World = world;
            AcceptedReceipts = Array.AsReadOnly(accepted.ToArray());
            MetadataReceipts = Array.AsReadOnly(metadata.ToArray());
            Events = Array.AsReadOnly(events.ToArray());
        }
    }

    public sealed class PipelineFailureException : InvalidOperationException
    {
        public string Code { get; }
        public PipelineFailureException(string code) : base(code) { Code = code; }
    }

    /// <summary>In-process atomicity only. No durability, scheduler, UI projections or storage recovery claims.</summary>
    public sealed class CommandProcessor
    {
        private static readonly Regex Token = new Regex(@"\A[a-zA-Z0-9][a-zA-Z0-9_.:/-]{0,127}\z", RegexOptions.CultureInvariant);
        private readonly object writer = new object();
        private readonly RuleRegistry rules;
        private CommittedWorld current;
        private bool executing;
        // Failure probes are host-only test seams; no proposed facts are delivered to them.
        private readonly Action? beforeCommit;
        private readonly Action? afterCommit;
        public CommandProcessor(WorldSnapshot initial, RuleRegistry rules, Action? beforeCommit = null, Action? afterCommit = null)
        {
            current = new CommittedWorld(initial ?? throw new ArgumentNullException(nameof(initial)), Array.Empty<CommandReceipt>(),
                Array.Empty<CommandReceipt>(), Array.Empty<CommittedTransfer>());
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.beforeCommit = beforeCommit; this.afterCommit = afterCommit;
        }
        // Trusted diagnostics/composition only: this omniscient view must not be exposed to UI/actor decisions.
        public CommittedWorld InspectCommitted() { lock (writer) return current; }

        public CommandReceipt Execute(CommandEnvelope envelope, ExecutionPrincipal principal)
        {
            if (envelope == null) throw new ArgumentNullException(nameof(envelope));
            if (principal == null) throw new ArgumentNullException(nameof(principal));
            lock (writer)
            {
                if (executing) throw new PipelineFailureException("REENTRANT_COMMAND");
                executing = true;
                try { return ExecuteLocked(envelope, principal); }
                finally { executing = false; }
            }
        }
        private CommandReceipt ExecuteLocked(CommandEnvelope envelope, ExecutionPrincipal principal)
        {
            var before = current;
            var world = before.World;
            if (envelope.CommandId == null || !Token.IsMatch(envelope.CommandId))
                return Receipt(envelope, "", CommandOutcome.Rejected, "INVALID_ENVELOPE", world);
            string fingerprint;
            try { fingerprint = envelope.Fingerprint(); }
            catch (ArgumentException) { return Receipt(envelope, "", CommandOutcome.Rejected, "INVALID_ENVELOPE", world); }

            // Authenticate independently of caller-provided source. Never disclose another principal's receipt.
            if (envelope.IssuerId != principal.ActorId.ToString() || envelope.SourceType != SourceName(principal.Source))
                return Receipt(envelope, fingerprint, CommandOutcome.Rejected, "NOT_AUTHORIZED", world);
            var existing = before.AcceptedReceipts.Concat(before.MetadataReceipts).FirstOrDefault(r => r.CommandId == envelope.CommandId);
            if (existing != null)
                return existing.Fingerprint == fingerprint ? existing : Receipt(envelope, fingerprint, CommandOutcome.Rejected, "CMD_PAYLOAD_MISMATCH", world);
            if (envelope.CorrelationId == null || !Token.IsMatch(envelope.CorrelationId) || string.IsNullOrEmpty(envelope.CommandType))
                return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "INVALID_ENVELOPE");
            if (envelope.ExpectedRevision != world.Revision.Value)
                return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "STALE_REVISION");
            if (envelope.WorldTick != world.Tick.Value)
                return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "TIME_MISMATCH");
            if (envelope.CommandType != "TransferResource")
                return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "COMMAND_UNKNOWN");
            if (envelope.PayloadVersion != 1)
                return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "PAYLOAD_VERSION_UNSUPPORTED");
            TransferResource transfer;
            try { transfer = ParseTransfer(envelope.Payload); }
            catch (IdentityException error) { return Metadata(envelope, fingerprint, CommandOutcome.Rejected, error.Code); }
            catch (Exception error) when (error is ArgumentException || error is FormatException)
            { return Metadata(envelope, fingerprint, CommandOutcome.Rejected, "INVALID_PAYLOAD"); }
            var result = rules.Evaluate(world, principal.ActorId, transfer);
            if (result.ErrorCode != null) return Metadata(envelope, fingerprint, CommandOutcome.Rejected, result.ErrorCode);
            if (result.Changes == null) throw new PipelineFailureException("MISSING_PROPOSAL");
            // Independently verify permissions and exact effect even for a registered but buggy handler.
            ValidateTransfer(world, principal.ActorId, transfer, result.Changes);
            if (result.Changes.LotChanges.Count == 0)
                return Metadata(envelope, fingerprint, CommandOutcome.NoOp, null);
            ulong revision;
            try { revision = checked(world.Revision.Value + 1); }
            catch (OverflowException) { throw new PipelineFailureException("REVISION_EXHAUSTED"); }
            var identity = world.Identity.BeginStage(revision);
            var eventId = identity.Allocate<Event>();
            var updates = result.Changes.LotChanges.ToDictionary(c => c.LotId, c => c.NewOnHand);
            var lots = world.Lots.Select(l => updates.TryGetValue(l.Id, out var quantity)
                ? new ResourceLot(l.Id, l.Commodity, l.Owner, l.Location, quantity, l.Reserved) : l).ToArray();
            var candidate = new WorldSnapshot(identity.PrepareSnapshot(), world.Tick, world.Actors, lots, world.RngStates);
            var receipt = new CommandReceipt(envelope, fingerprint, CommandOutcome.Accepted, null, world.Revision.Value, revision,
                world.Tick, new[] { eventId });
            var fact = new CommittedTransfer(eventId, revision, world.Tick, envelope, transfer);
            var publication = new CommittedWorld(candidate, before.AcceptedReceipts.Concat(new[] { receipt }), before.MetadataReceipts,
                before.Events.Concat(new[] { fact }));
            beforeCommit?.Invoke();
            if (!ReferenceEquals(current, before)) throw new PipelineFailureException("COMMIT_CONFLICT");
            current = publication; // Single atomic reference publication under the writer lock.
            afterCommit?.Invoke(); // A lost response cannot roll back this already committed action.
            return receipt;
        }
        public static string SourceName(CommandSource source) => source == CommandSource.Player ? "Player" : source == CommandSource.Npc ? "NPC" : "Scheduler";
        private static TransferResource ParseTransfer(IReadOnlyDictionary<string, object?> payload)
        {
            if (payload.Count != 3 || !payload.TryGetValue("source_lot_id", out var source) || !(source is string sourceId) ||
                !payload.TryGetValue("destination_lot_id", out var target) || !(target is string targetId) ||
                !payload.TryGetValue("quantity", out var number) || !(number is long || number is int))
                throw new ArgumentException("Invalid transfer schema");
            return new TransferResource(EntityId<Lot>.Parse(sourceId), EntityId<Lot>.Parse(targetId), Convert.ToInt64(number));
        }
        private static CommandReceipt Receipt(CommandEnvelope command, string fingerprint, CommandOutcome outcome, string? code, WorldSnapshot world)
            => new CommandReceipt(command, fingerprint, outcome, code, world.Revision.Value, world.Revision.Value, world.Tick, Array.Empty<EntityId<Event>>());
        private CommandReceipt Metadata(CommandEnvelope command, string fingerprint, CommandOutcome outcome, string? code)
        {
            var receipt = Receipt(command, fingerprint, outcome, code, current.World);
            var candidate = new CommittedWorld(current.World, current.AcceptedReceipts, current.MetadataReceipts.Concat(new[] { receipt }), current.Events);
            beforeCommit?.Invoke();
            current = candidate;
            afterCommit?.Invoke();
            return receipt;
        }
        private static void ValidateTransfer(WorldSnapshot world, EntityId<Actor> issuer, TransferResource transfer, ChangeSet proposal)
        {
            try
            {
                var actor = world.GetActor(issuer);
                var source = world.GetLot(transfer.SourceLotId);
                var destination = world.GetLot(transfer.DestinationLotId);
                if (!actor.Alive || !actor.ControlledOwnerIds.Contains(source.Owner) || transfer.Quantity <= 0 ||
                    !source.Commodity.Equals(destination.Commodity) || !source.Location.Equals(destination.Location) ||
                    source.OnHand - source.Reserved < transfer.Quantity)
                    throw new PipelineFailureException("PROPOSAL_PRECONDITION");
                if (source.Id.Equals(destination.Id))
                {
                    if (proposal.LotChanges.Count != 0) throw new PipelineFailureException("PROPOSAL_NOOP");
                    return;
                }
                if (proposal.LotChanges.Count != 2 || proposal.LotChanges.Select(c => c.LotId).Distinct().Count() != 2)
                    throw new PipelineFailureException("PROPOSAL_EFFECT");
                foreach (var change in proposal.LotChanges)
                {
                    if (change.Owner != "Assets" || change.FieldPath != "lots.on_hand") throw new PipelineFailureException("PROPOSAL_OWNERSHIP");
                    var lot = world.GetLot(change.LotId);
                    if (lot.OnHand != change.ExpectedOnHand || change.NewOnHand < lot.Reserved || change.NewOnHand < 0)
                        throw new PipelineFailureException("PROPOSAL_INVARIANT");
                    var expected = lot.Id.Equals(source.Id) ? checked(source.OnHand - transfer.Quantity)
                        : lot.Id.Equals(destination.Id) ? checked(destination.OnHand + transfer.Quantity)
                        : throw new PipelineFailureException("PROPOSAL_REFERENCE");
                    if (change.NewOnHand != expected) throw new PipelineFailureException("PROPOSAL_CONSERVATION");
                }
            }
            catch (IdentityException) { throw new PipelineFailureException("PROPOSAL_REFERENCE"); }
            catch (OverflowException) { throw new PipelineFailureException("PROPOSAL_OVERFLOW"); }
        }
    }
}
