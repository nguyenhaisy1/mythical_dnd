using System;
using System.Collections.Generic;
using System.Linq;
using Game.Foundation.Identity;

namespace Game.Domain.Kernel
{
    public interface IKernelCommand
    {
        string CommandType { get; }
        int PayloadVersion { get; }
    }
    public sealed class TransferResource : IKernelCommand
    {
        public string CommandType => "TransferResource";
        public int PayloadVersion => 1;
        public EntityId<Lot> SourceLotId { get; }
        public EntityId<Lot> DestinationLotId { get; }
        public long Quantity { get; }
        public TransferResource(EntityId<Lot> sourceLotId, EntityId<Lot> destinationLotId, long quantity)
        {
            SourceLotId = sourceLotId ?? throw new ArgumentNullException(nameof(sourceLotId));
            DestinationLotId = destinationLotId ?? throw new ArgumentNullException(nameof(destinationLotId)); Quantity = quantity;
        }
    }
    public sealed class LotChange
    {
        public EntityId<Lot> LotId { get; }
        public long ExpectedOnHand { get; }
        public long NewOnHand { get; }
        public string Owner { get; }
        public string FieldPath { get; }
        public LotChange(EntityId<Lot> lotId, long expectedOnHand, long newOnHand, string owner = "Assets", string fieldPath = "lots.on_hand")
        {
            LotId = lotId ?? throw new ArgumentNullException(nameof(lotId)); ExpectedOnHand = expectedOnHand;
            NewOnHand = newOnHand; Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            FieldPath = fieldPath ?? throw new ArgumentNullException(nameof(fieldPath));
        }
    }
    public sealed class ChangeSet
    {
        public IReadOnlyList<LotChange> LotChanges { get; }
        public ChangeSet(IEnumerable<LotChange> lotChanges)
        {
            var changes = (lotChanges ?? throw new ArgumentNullException(nameof(lotChanges))).ToArray();
            if (changes.Any(x => x == null)) throw new ArgumentException("Null change");
            LotChanges = Array.AsReadOnly(changes);
        }
    }
    public sealed class RuleCheck
    {
        public string Name { get; }
        public bool Passed { get; }
        public string? Detail { get; }
        public RuleCheck(string name, bool passed, string? detail = null)
        { Name = name; Passed = passed; Detail = detail; }
    }
    public sealed class RuleResult
    {
        public string? ErrorCode { get; }
        public ChangeSet? Changes { get; }
        public bool IsNoOp => ErrorCode == null && Changes!.LotChanges.Count == 0;
        public IReadOnlyList<RuleCheck> Checks { get; }
        private RuleResult(string? errorCode, ChangeSet? changes, IEnumerable<RuleCheck>? checks = null)
        { ErrorCode = errorCode; Changes = changes; Checks = Array.AsReadOnly((checks ?? Array.Empty<RuleCheck>()).ToArray()); }
        public RuleResult WithChecks(IEnumerable<RuleCheck> checks) => new RuleResult(ErrorCode, Changes, checks);
        public static RuleResult Reject(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Missing rejection code");
            return new RuleResult(code, null);
        }
        public static RuleResult Propose(ChangeSet changes) => new RuleResult(null, changes ?? throw new ArgumentNullException(nameof(changes)));
        public static RuleResult NoOp() => Propose(new ChangeSet(Array.Empty<LotChange>()));
    }
    public interface IRuleHandler
    {
        string CommandType { get; }
        int PayloadVersion { get; }
        Type PayloadType { get; }
        RuleResult Evaluate(WorldSnapshot world, EntityId<Actor> issuer, IKernelCommand command);
    }
    /// <summary>Only the composition root registers compiled handlers. Content cannot instantiate types.</summary>
    public sealed class RuleRegistry
    {
        private readonly Dictionary<string, Dictionary<int, IRuleHandler>> handlers = new Dictionary<string, Dictionary<int, IRuleHandler>>(StringComparer.Ordinal);
        public RuleRegistry(IEnumerable<IRuleHandler> approvedHandlers)
        {
            foreach (var handler in approvedHandlers ?? throw new ArgumentNullException(nameof(approvedHandlers)))
            {
                if (handler == null || string.IsNullOrWhiteSpace(handler.CommandType) || handler.PayloadVersion < 1 ||
                    !typeof(IKernelCommand).IsAssignableFrom(handler.PayloadType)) throw new ArgumentException("Invalid handler registration");
                if (!handlers.TryGetValue(handler.CommandType, out var versions))
                    handlers.Add(handler.CommandType, versions = new Dictionary<int, IRuleHandler>());
                versions.Add(handler.PayloadVersion, handler);
            }
        }
        public RuleResult Evaluate(WorldSnapshot world, EntityId<Actor> issuer, IKernelCommand command)
        {
            if (world == null || issuer == null || command == null) throw new ArgumentNullException();
            if (command.CommandType == null || !handlers.TryGetValue(command.CommandType, out var versions)) return RuleResult.Reject("COMMAND_UNKNOWN");
            if (!versions.TryGetValue(command.PayloadVersion, out var handler)) return RuleResult.Reject("PAYLOAD_VERSION_UNSUPPORTED");
            if (command.GetType() != handler.PayloadType) return RuleResult.Reject("INVALID_PAYLOAD");
            return handler.Evaluate(world, issuer, command) ?? throw new InvalidOperationException("Handler returned null");
        }
    }
    public sealed class TransferResourceRule : IRuleHandler
    {
        public string CommandType => "TransferResource";
        public int PayloadVersion => 1;
        public Type PayloadType => typeof(TransferResource);
        public RuleResult Evaluate(WorldSnapshot world, EntityId<Actor> issuer, IKernelCommand command)
        {
            var checks = new List<RuleCheck>();
            bool Check(string name, bool passed, string? detail = null)
            { checks.Add(new RuleCheck(name, passed, detail)); return passed; }
            RuleResult Reject(string code) => RuleResult.Reject(code).WithChecks(checks);
            if (!Check("payload_type", command is TransferResource)) return Reject("INVALID_PAYLOAD");
            var transfer = (TransferResource)command;
            if (!Check("quantity_positive", transfer.Quantity > 0)) return Reject("INVALID_PAYLOAD");
            try
            {
                var actor = world.GetActor(issuer);
                if (!Check("actor_alive", actor.Alive)) return Reject("ACTOR_DEAD");
                var source = world.GetLot(transfer.SourceLotId); var destination = world.GetLot(transfer.DestinationLotId);
                if (!Check("controls_source", actor.Controls(source.Owner))) return Reject("NOT_AUTHORIZED");
                if (!Check("commodity_match", source.Commodity.Equals(destination.Commodity))) return Reject("COMMODITY_MISMATCH");
                if (!Check("location_match", source.Location.Equals(destination.Location))) return Reject("LOCATION_MISMATCH");
                if (!Check("available_quantity", source.Available >= transfer.Quantity)) return Reject("INSUFFICIENT_RESOURCE");
                if (source.Id.Equals(destination.Id)) return RuleResult.NoOp().WithChecks(checks);
                var afterDestination = checked(destination.OnHand + transfer.Quantity);
                Check("destination_capacity", true);
                return RuleResult.Propose(new ChangeSet(new[] {
                    new LotChange(source.Id, source.OnHand, checked(source.OnHand - transfer.Quantity)),
                    new LotChange(destination.Id, destination.OnHand, afterDestination) })).WithChecks(checks);
            }
            catch (IdentityException ex) { Check("identity_resolution", false, ex.Code); return Reject(ex.Code); }
            catch (OverflowException) { Check("destination_capacity", false, "NUMERIC_OVERFLOW"); return Reject("NUMERIC_OVERFLOW"); }
        }
    }
}
