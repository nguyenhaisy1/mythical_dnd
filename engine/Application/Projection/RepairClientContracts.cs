using System;

namespace Game.Application.Projection
{
    /// <summary>Only the actor-scoped client surface; never returns engine state or command envelopes.</summary>
    public interface IRepairClientSession
    {
        RepairPlayerView Read();
        RepairClientPreview PreviewRepair();
        RepairClientResult ConfirmRepair(string previewToken, string requestId);
        RepairClientResult Retry(string requestId);
        RepairClientResult CancelOwnRepair(string requestId);
        RepairClientResult AdvanceBy(long minutes, string requestId);
        RepairClientResult SaveSnapshot();
        RepairClientResult Reload();
    }

    [Flags]
    public enum RepairObservationFields
    {
        None = 0, ActorStatus = 1, BoatPresence = 2, BoatCondition = 4,
        MaterialOnHand = 8, MaterialReserved = 16, ToolUsable = 32,
        Qualification = 64, OwnActivity = 128, Recipe = 256,
        All = ActorStatus | BoatPresence | BoatCondition | MaterialOnHand | MaterialReserved |
            ToolUsable | Qualification | OwnActivity | Recipe
    }

    public sealed class RepairObservationInfo
    {
        public string Source { get; }
        public long ObservedTick { get; }
        internal RepairObservationInfo(string source, long tick) { Source = source; ObservedTick = tick; }
    }

    public sealed class RepairPlayerView
    {
        public string ProfileId => "repair-client-2d-v1";
        public string ArtProfileId => "earth-monument-2d-v1";
        public string ActorLabel { get; }
        public string LocationLabel { get; }
        public long Tick { get; }
        public bool? ActorAlive { get; }
        public bool BoatVisible { get; }
        public int? BoatCondition { get; }
        public long? TimberOnHand { get; }
        public long? TimberReserved { get; }
        public long? TimberAvailable => TimberOnHand.HasValue && TimberReserved.HasValue ? TimberOnHand.Value - TimberReserved.Value : (long?)null;
        public bool? ToolUsable { get; }
        public bool? Qualified { get; }
        public string? ActivityStatus { get; }
        public long? ActivityDueTick { get; }
        public bool CanCancel { get; }
        public bool? CanRepair { get; }
        public RepairObservationInfo Observation { get; }
        internal RepairPlayerView(string actor, string location, long tick, bool? alive, bool boatVisible,
            int? condition, long? onHand, long? reserved, bool? tool, bool? qualified,
            string? activity, long? due, bool canCancel, bool? canRepair, RepairObservationInfo observation)
        {
            ActorLabel = actor; LocationLabel = location; Tick = tick; ActorAlive = alive; BoatVisible = boatVisible;
            BoatCondition = condition; TimberOnHand = onHand; TimberReserved = reserved; ToolUsable = tool;
            Qualified = qualified; ActivityStatus = activity; ActivityDueTick = due;
            CanCancel = canCancel; CanRepair = canRepair; Observation = observation;
        }
    }

    public sealed class RepairClientPreview
    {
        public string? Token { get; }
        public bool CanConfirm { get; }
        public bool RequirementsKnown { get; }
        public long? CostTimber { get; }
        public long? DurationTicks { get; }
        public int? ConditionGain { get; }
        public int? ConditionCap { get; }
        public string Code { get; }
        internal RepairClientPreview(string? token, bool canConfirm, bool known, long? cost, long? duration,
            int? gain, int? cap, string code)
        { Token = token; CanConfirm = canConfirm; RequirementsKnown = known; CostTimber = cost; DurationTicks = duration;
            ConditionGain = gain; ConditionCap = cap; Code = code; }
    }

    /// <summary>Codes are a fixed UI vocabulary, not domain diagnostics or exception messages.</summary>
    public sealed class RepairClientResult
    {
        public bool Success { get; }
        public string Code { get; }
        public string? RequestId { get; }
        public bool Durable { get; }
        internal RepairClientResult(bool success, string code, string? requestId, bool durable)
        { Success = success; Code = code; RequestId = requestId; Durable = durable; }
    }
}
