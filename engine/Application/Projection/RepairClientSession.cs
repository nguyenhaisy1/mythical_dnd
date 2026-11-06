using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Application.CommandPipeline;
using Game.Application.SimulationScheduler;
using Game.Domain.Repair;

namespace Game.Application.Projection
{
    /// <summary>Trusted composition owns the processor and persistence callbacks; UI receives IRepairClientSession only.</summary>
    public sealed class RepairClientSession : IRepairClientSession
    {
        private readonly object gate = new object();
        private CommandProcessor processor;
        private readonly RepairObservationScope scope;
        private readonly Func<CommittedWorld, CommandProcessor> restoreFactory;
        private readonly Func<CommittedWorld> load;
        private readonly Action<CommittedWorld> snapshot;
        private readonly bool durableCommands;
        private readonly Dictionary<string, PreviewPin> previews = new Dictionary<string, PreviewPin>(StringComparer.Ordinal);
        private readonly Dictionary<string, Intent> requests = new Dictionary<string, Intent>(StringComparer.Ordinal);
        private ulong previewSequence;
        private static readonly Regex RequestToken = new Regex(@"\A[a-zA-Z0-9][a-zA-Z0-9_.-]{0,79}\z", RegexOptions.CultureInvariant);
        private sealed class PreviewPin
        {
            internal ulong Revision { get; }
            internal long Tick { get; }
            internal PreviewPin(ulong revision, long tick) { Revision = revision; Tick = tick; }
        }
        private sealed class Intent
        {
            internal string Kind { get; }
            internal string Argument { get; }
            internal CommandEnvelope Envelope { get; }
            internal Intent(string kind, string argument, CommandEnvelope envelope) { Kind = kind; Argument = argument; Envelope = envelope; }
        }
        public RepairClientSession(CommandProcessor processor, RepairObservationScope scope,
            Func<CommittedWorld, CommandProcessor> restoreFactory, Func<CommittedWorld> load, Action<CommittedWorld> snapshot,
            bool durableCommands = false)
        {
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
            this.scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.restoreFactory = restoreFactory ?? throw new ArgumentNullException(nameof(restoreFactory));
            this.load = load ?? throw new ArgumentNullException(nameof(load));
            this.snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
            this.durableCommands = durableCommands;
            ValidateBinding(processor.InspectCommitted());
        }
        private void ValidateBinding(CommittedWorld state)
        {
            var repair = state.World.Repair ?? throw new ArgumentException("Repair profile required");
            state.World.GetActor(scope.Principal.ActorId); state.World.GetLot(scope.MaterialLotId);
            if (repair.GetItem(scope.BoatId).Kind != RepairItemKind.Boat || repair.GetItem(scope.ToolId).Kind != RepairItemKind.RepairTool ||
                repair.Recipe.MaterialQuantity != 2 || repair.Recipe.DurationTicks != 60 ||
                repair.Recipe.ConditionGain != 200 || repair.Recipe.ConditionMax != 1000)
                throw new ArgumentException("Bounded repair client profile mismatch");
        }
        public RepairPlayerView Read()
        { lock (gate) return Project(processor.InspectCommitted()); }
        private RepairPlayerView Project(CommittedWorld state)
        {
            var world = state.World; var repair = world.Repair!; var actor = world.GetActor(scope.Principal.ActorId);
            var here = actor.Location.Equals(scope.Worksite);
            bool Known(RepairObservationFields field) => here && scope.Grants(field);
            var boat = repair.GetItem(scope.BoatId); var material = world.GetLot(scope.MaterialLotId); var tool = repair.GetItem(scope.ToolId);
            bool visible = Known(RepairObservationFields.BoatPresence) && boat.Location.Equals(scope.Worksite);
            bool? alive = Known(RepairObservationFields.ActorStatus) ? actor.Alive : (bool?)null;
            int? condition = visible && Known(RepairObservationFields.BoatCondition) ? boat.Condition : (int?)null;
            long? onHand = Known(RepairObservationFields.MaterialOnHand) && material.Location.Equals(scope.Worksite) ? material.OnHand : (long?)null;
            long? reserved = Known(RepairObservationFields.MaterialReserved) && material.Location.Equals(scope.Worksite) ? material.Reserved : (long?)null;
            bool? usable = Known(RepairObservationFields.ToolUsable) && tool.Location.Equals(scope.Worksite)
                ? tool.Kind == RepairItemKind.RepairTool && !tool.Destroyed && tool.Condition > 0 : (bool?)null;
            var capability = repair.Capabilities.FirstOrDefault(c => c.ActorId.Equals(scope.Principal.ActorId));
            bool? qualified = Known(RepairObservationFields.Qualification) ? capability != null && capability.Qualified && capability.ToolItemId.Equals(scope.ToolId) : (bool?)null;
            var own = repair.Activities.Where(a => a.ActorId.Equals(scope.Principal.ActorId) && a.ItemId.Equals(scope.BoatId))
                .OrderByDescending(a => a.Id.Value.Counter).FirstOrDefault();
            var activityKnown = Known(RepairObservationFields.OwnActivity);
            string? activity = activityKnown ? own == null ? "none" : own.Status == RepairActivityStatus.Running ? "running"
                : own.Status == RepairActivityStatus.Completed ? "completed" : "cancelled" : null;
            long? due = activityKnown && own != null ? own.DueTick.Value : (long?)null;
            var known = alive.HasValue && visible && condition.HasValue && onHand.HasValue && reserved.HasValue && usable.HasValue && qualified.HasValue && activityKnown && Known(RepairObservationFields.Recipe);
            bool? canRepair = known ? alive!.Value && usable!.Value && qualified!.Value && condition!.Value < repair.Recipe.ConditionMax &&
                onHand!.Value - reserved!.Value >= repair.Recipe.MaterialQuantity && activity != "running" : (bool?)null;
            return new RepairPlayerView(scope.ActorLabel, here ? scope.LocationLabel : "Chưa rõ", world.Tick.Value, alive, visible,
                condition, onHand, reserved, usable, qualified, activity, due,
                activityKnown && own != null && own.ItemId.Equals(scope.BoatId) && own.Status == RepairActivityStatus.Running && alive == true,
                canRepair, new RepairObservationInfo(here && scope.Fields != RepairObservationFields.None ? "direct-worksite-fixture" : "unknown", world.Tick.Value));
        }
        public RepairClientPreview PreviewRepair()
        {
            lock (gate)
            {
                var state = processor.InspectCommitted(); var view = Project(state); var recipe = state.World.Repair!.Recipe;
                var recipeKnown = scope.Grants(RepairObservationFields.Recipe) &&
                    state.World.GetActor(scope.Principal.ActorId).Location.Equals(scope.Worksite);
                string? token = null;
                if (view.CanRepair == true)
                {
                    if (previewSequence == ulong.MaxValue) throw new InvalidOperationException("Preview capacity exhausted");
                    token = "preview/" + (++previewSequence).ToString("x16", CultureInfo.InvariantCulture);
                    previews.Add(token, new PreviewPin(state.World.Revision.Value, state.World.Tick.Value));
                }
                return new RepairClientPreview(token, view.CanRepair == true, view.CanRepair.HasValue,
                    recipeKnown ? recipe.MaterialQuantity : (long?)null, recipeKnown ? recipe.DurationTicks : (long?)null,
                    recipeKnown ? recipe.ConditionGain : (int?)null, recipeKnown ? recipe.ConditionMax : (int?)null,
                    view.CanRepair == true ? "ok" : view.CanRepair.HasValue ? "unavailable" : "not_observed");
            }
        }
        public RepairClientResult ConfirmRepair(string previewToken, string requestId)
        {
            lock (gate)
            {
                if (!ValidRequest(requestId)) return Result(false, "invalid_request", null);
                if (requests.TryGetValue(requestId, out var old)) return old.Kind == "repair" && old.Argument == previewToken ? Execute(old, requestId) : Result(false, "request_mismatch", requestId);
                if (previewToken == null || !previews.TryGetValue(previewToken, out var pin)) return Result(false, "stale_preview", requestId);
                var state = processor.InspectCommitted();
                if (state.World.Revision.Value != pin.Revision || state.World.Tick.Value != pin.Tick)
                {
                    var frozen = new CommandEnvelope(CommandId(requestId), "StartRepair", scope.Principal.ActorId.ToString(), "Player",
                        pin.Tick, pin.Revision, 1, StartPayload(state), "repair-client");
                    requests.Add(requestId, new Intent("repair", previewToken, frozen));
                    return Result(false, "stale_preview", requestId);
                }
                if (Project(state).CanRepair != true) return Result(false, "not_observed", requestId);
                return RememberAndExecute(requestId, "repair", previewToken, Envelope(state, requestId, "StartRepair", StartPayload(state)));
            }
        }
        public RepairClientResult Retry(string requestId)
        {
            lock (gate)
            {
                if (!ValidRequest(requestId)) return Result(false, "invalid_request", null);
                if (!requests.TryGetValue(requestId, out var intent))
                {
                    var state = processor.InspectCommitted();
                    var receipt = state.AcceptedReceipts.Concat(state.MetadataReceipts)
                        .FirstOrDefault(r => r.CommandId == CommandId(requestId) && IsBoundClientCommand(state, r.OriginalEnvelope));
                    if (receipt == null) return Result(false, "not_found", requestId);
                    intent = new Intent("retry", "", receipt.OriginalEnvelope); requests.Add(requestId, intent);
                }
                return Execute(intent, requestId);
            }
        }
        public RepairClientResult CancelOwnRepair(string requestId)
        {
            lock (gate)
            {
                if (!ValidRequest(requestId)) return Result(false, "invalid_request", null);
                if (requests.TryGetValue(requestId, out var old)) return old.Kind == "cancel" ? Execute(old, requestId) : Result(false, "request_mismatch", requestId);
                var state = processor.InspectCommitted(); if (!Project(state).CanCancel) return Result(false, "unavailable", requestId);
                var activity = state.World.Repair!.Activities.Single(a => a.ActorId.Equals(scope.Principal.ActorId) && a.ItemId.Equals(scope.BoatId) && a.Status == RepairActivityStatus.Running);
                return RememberAndExecute(requestId, "cancel", "", Envelope(state, requestId, "CancelActivity", new Dictionary<string, object?> {
                    ["activity_id"] = activity.Id.ToString(), ["reason"] = "player_cancel" }));
            }
        }
        public RepairClientResult AdvanceBy(long minutes, string requestId)
        {
            lock (gate)
            {
                if (!ValidRequest(requestId)) return Result(false, "invalid_request", null);
                var argument = minutes.ToString(CultureInfo.InvariantCulture);
                if (requests.TryGetValue(requestId, out var old)) return old.Kind == "advance" && old.Argument == argument ? Execute(old, requestId) : Result(false, "request_mismatch", requestId);
                if (minutes < 1 || minutes > 1440) return Result(false, "invalid_request", requestId);
                var state = processor.InspectCommitted(); long target;
                try { target = checked(state.World.Tick.Value + minutes); }
                catch (OverflowException) { return Result(false, "unavailable", requestId); }
                return RememberAndExecute(requestId, "advance", argument, Envelope(state, requestId, "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = target }));
            }
        }
        private Dictionary<string, object?> StartPayload(CommittedWorld state) => new Dictionary<string, object?> {
            ["actor_id"] = scope.Principal.ActorId.ToString(), ["item_id"] = scope.BoatId.ToString(),
            ["material_lot_id"] = scope.MaterialLotId.ToString(), ["recipe_id"] = state.World.Repair!.Recipe.Id.ToString() };
        private RepairClientResult RememberAndExecute(string requestId, string kind, string argument, CommandEnvelope envelope)
        {
            var intent = new Intent(kind, argument, envelope); requests.Add(requestId, intent); return Execute(intent, requestId);
        }
        private CommandEnvelope Envelope(CommittedWorld state, string id, string type, Dictionary<string, object?> payload)
            => new CommandEnvelope(CommandId(id), type, scope.Principal.ActorId.ToString(), "Player", state.World.Tick.Value,
                state.World.Revision.Value, 1, payload, "repair-client");
        private static string CommandId(string requestId) => "client/" + requestId;
        private static bool ValidRequest(string? id) => id != null && RequestToken.IsMatch(id);
        private bool IsBoundClientCommand(CommittedWorld state, CommandEnvelope command)
        {
            if (command.WorldTick < 0 || command.IssuerId != scope.Principal.ActorId.ToString() || command.SourceType != "Player" ||
                command.PayloadVersion != 1 || command.CorrelationId != "repair-client") return false;
            var payload = command.Payload;
            bool Text(string key, string expected) => payload.TryGetValue(key, out var value) && value is string text && text == expected;
            if (command.CommandType == "StartRepair")
                return payload.Count == 4 && Text("actor_id", scope.Principal.ActorId.ToString()) &&
                    Text("item_id", scope.BoatId.ToString()) && Text("material_lot_id", scope.MaterialLotId.ToString()) &&
                    Text("recipe_id", state.World.Repair!.Recipe.Id.ToString());
            if (command.CommandType == "CancelActivity")
                return payload.Count == 2 && Text("reason", "player_cancel") &&
                    state.World.Repair!.Activities.Any(a => a.ActorId.Equals(scope.Principal.ActorId) &&
                        a.ItemId.Equals(scope.BoatId) && Text("activity_id", a.Id.ToString()));
            if (command.CommandType == "AdvanceTime" && payload.Count == 1 &&
                payload.TryGetValue("target_tick", out var target) && target is long tick && tick > command.WorldTick)
                return tick - command.WorldTick <= 1440;
            return false;
        }
        private RepairClientResult Execute(Intent intent, string requestId)
        {
            try
            {
                var receipt = processor.Execute(intent.Envelope, scope.Principal);
                if (receipt.Outcome == CommandOutcome.AcceptedPending && intent.Envelope.CommandType == "AdvanceTime")
                {
                    var workflow = processor.InspectCommitted().Scheduler.Workflow;
                    if (workflow != null && workflow.Id == intent.Envelope.CommandId && workflow.Status != AdvanceStatus.Completed)
                    {
                        processor.DrainAdvance(scope.Principal);
                        if (processor.InspectCommitted().Scheduler.Workflow!.Status != AdvanceStatus.Completed) return Result(true, "advance_pending", requestId, durableCommands);
                    }
                }
                var success = receipt.Outcome != CommandOutcome.Rejected;
                return Result(success, success ? "ok" : receipt.ErrorCode == "STALE_REVISION" || receipt.ErrorCode == "TIME_MISMATCH" ? "stale_preview"
                    : receipt.ErrorCode == "CMD_PAYLOAD_MISMATCH" ? "request_mismatch" : "unavailable", requestId, success && durableCommands);
            }
            catch (Exception) { return Result(false, "session_error", requestId); }
        }
        public RepairClientResult SaveSnapshot()
        {
            lock (gate)
            {
                try { snapshot(processor.InspectCommitted()); return Result(true, "saved", null, true); }
                catch (Exception) { return Result(false, "storage_error", null); }
            }
        }
        public RepairClientResult Reload()
        {
            lock (gate)
            {
                try
                {
                    var state = load(); ValidateBinding(state); var restored = restoreFactory(state);
                    if (!ReferenceEquals(restored.InspectCommitted(), state)) throw new InvalidOperationException("Restore factory did not restore loaded state");
                    processor = restored; previews.Clear(); return Result(true, "loaded", null, true);
                }
                catch (Exception) { return Result(false, "storage_error", null); }
            }
        }
        private static RepairClientResult Result(bool success, string code, string? requestId, bool durable = false)
            => new RepairClientResult(success, code, requestId, durable);
    }
}
