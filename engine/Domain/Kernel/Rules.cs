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
    public sealed class RuleResult
    {
        public string? ErrorCode { get; }
        public ChangeSet? Changes { get; }
        public bool IsNoOp => ErrorCode == null && Changes!.LotChanges.Count == 0;
        private RuleResult(string? errorCode, ChangeSet? changes) { ErrorCode = errorCode; Changes = changes; }
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
            if (!(command is TransferResource transfer)) return RuleResult.Reject("INVALID_PAYLOAD");
            if (transfer.Quantity <= 0) return RuleResult.Reject("INVALID_PAYLOAD");
            try
            {
                var actor = world.GetActor(issuer);
                if (!actor.Alive) return RuleResult.Reject("ACTOR_DEAD");
                var source = world.GetLot(transfer.SourceLotId); var destination = world.GetLot(transfer.DestinationLotId);
                if (!actor.Controls(source.Owner)) return RuleResult.Reject("NOT_AUTHORIZED");
                if (!source.Commodity.Equals(destination.Commodity)) return RuleResult.Reject("COMMODITY_MISMATCH");
                if (!source.Location.Equals(destination.Location)) return RuleResult.Reject("LOCATION_MISMATCH");
                if (source.Available < transfer.Quantity) return RuleResult.Reject("INSUFFICIENT_RESOURCE");
                if (source.Id.Equals(destination.Id)) return RuleResult.NoOp();
                var afterDestination = checked(destination.OnHand + transfer.Quantity);
                return RuleResult.Propose(new ChangeSet(new[] {
                    new LotChange(source.Id, source.OnHand, checked(source.OnHand - transfer.Quantity)),
                    new LotChange(destination.Id, destination.OnHand, afterDestination) }));
            }
            catch (IdentityException ex) { return RuleResult.Reject(ex.Code); }
            catch (OverflowException) { return RuleResult.Reject("NUMERIC_OVERFLOW"); }
        }
    }
}
