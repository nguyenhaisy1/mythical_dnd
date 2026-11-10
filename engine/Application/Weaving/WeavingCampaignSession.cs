using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Application.CommandPipeline;
using Game.Application.Origins;
using Game.Application.SimulationScheduler;
using Game.Domain.Origins;
using Game.Domain.Weaving;

namespace Game.Application.Weaving
{
    /// <summary>Exact self-lot observations; authoritative changes stay behind the processor.</summary>
    public sealed class WeavingCampaignSession : IWeavingCampaignSession
    {
        private readonly object gate = new object();
        private CommandProcessor processor;
        private readonly CampaignOriginState origin;
        private readonly WeavingState genesis;
        private readonly Func<long, OriginRevealView> project;
        private readonly Func<CommittedWorld> load;
        private readonly Func<CommittedWorld, CommandProcessor> restore;
        private readonly Action<CommittedWorld> snapshot;
        private readonly Dictionary<string, Pin> previews = new Dictionary<string, Pin>(StringComparer.Ordinal);
        private readonly Dictionary<string, CommandEnvelope> requests = new Dictionary<string, CommandEnvelope>(StringComparer.Ordinal);
        private static readonly Regex Request = new Regex(@"\A[a-zA-Z0-9][a-zA-Z0-9_.-]{0,79}\z", RegexOptions.CultureInvariant);
        private static readonly Regex PreviewToken = new Regex(@"\A[0-9a-f]{32}\z", RegexOptions.CultureInvariant);
        private sealed class Pin
        {
            internal readonly ulong Revision;
            internal readonly long Tick;
            internal Pin(ulong revision, long tick) { Revision = revision; Tick = tick; }
        }
        public WeavingCampaignSession(CommandProcessor processor, Func<long, OriginRevealView> project,
            Func<CommittedWorld> load, Func<CommittedWorld, CommandProcessor> restore, Action<CommittedWorld> snapshot)
        {
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
            var state = processor.InspectCommitted();
            origin = state.World.Origin ?? throw new ArgumentException("Origin required");
            genesis = state.World.Weaving ?? throw new ArgumentException("Weaving profile required");
            this.project = project ?? throw new ArgumentNullException(nameof(project));
            this.load = load ?? throw new ArgumentNullException(nameof(load));
            this.restore = restore ?? throw new ArgumentNullException(nameof(restore));
            this.snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
            ValidateBinding(state); ResumeAcceptedAdvance(processor);
        }
        private void ValidateBinding(CommittedWorld state)
        {
            var o = state.World.Origin; var w = state.World.Weaving;
            if (o == null || w == null || o.Principal.ToString() != origin.Principal.ToString() ||
                o.ActorDefinitionId != origin.ActorDefinitionId || o.HouseholdDefinitionId != origin.HouseholdDefinitionId ||
                o.CommunityDefinitionId != origin.CommunityDefinitionId || o.SourceContentHash != origin.SourceContentHash ||
                o.CohortContentHash != origin.CohortContentHash || o.DrawIndex != origin.DrawIndex ||
                w.ContentHash != genesis.ContentHash || w.Recipe.Fingerprint != genesis.Recipe.Fingerprint ||
                !w.ActorId.Equals(genesis.ActorId) || !w.Worksite.Equals(genesis.Worksite) ||
                !w.InputLotId.Equals(genesis.InputLotId) || !w.OutputLotId.Equals(genesis.OutputLotId) || w.SkillValue != genesis.SkillValue)
                throw new ArgumentException("Session binding mismatch");
        }
        private void ResumeAcceptedAdvance(CommandProcessor candidate)
        {
            var flow = candidate.InspectCommitted().Scheduler.Workflow;
            if (flow == null || flow.Status == AdvanceStatus.Completed) return;
            if (!flow.OwnerId.Equals(origin.Principal) || flow.OwnerSource != CommandSource.Player || flow.Status == AdvanceStatus.PausedDecision)
                throw new ArgumentException("Unowned or unsupported pending intent");
            candidate.DrainAdvance(new ExecutionPrincipal(origin.Principal, CommandSource.Player));
            if (candidate.InspectCommitted().Scheduler.Workflow!.Status != AdvanceStatus.Completed)
                throw new InvalidOperationException("Bounded pending advance not finished");
        }
        public OriginRevealView Read() { lock (gate) return project(processor.InspectCommitted().World.Tick.Value); }
        public WeavingWorkView ReadWork() { lock (gate) return Work(processor.InspectCommitted()); }
        private WeavingWorkView Work(CommittedWorld state)
        {
            var world = state.World; var work = world.Weaving!; var actor = world.GetActor(work.ActorId);
            var input = world.GetLot(work.InputLotId); var output = world.GetLot(work.OutputLotId);
            var latest = work.Activities.OrderByDescending(a => a.Id.Value.Counter).FirstOrDefault();
            var running = latest != null && latest.Status == WeavingActivityStatus.Running;
            var qualified = work.SkillValue >= work.Recipe.MinimumSkill;
            var canStart = actor.Alive && actor.Location.Equals(work.Worksite) && qualified && !running &&
                actor.Controls(input.Owner) && actor.Controls(output.Owner) && input.Location.Equals(work.Worksite) && output.Location.Equals(work.Worksite) &&
                input.Commodity.ToString() == "commodity.fiber@1" && output.Commodity.ToString() == "commodity.cord@1" &&
                input.Available >= work.Recipe.FiberCost && output.OnHand <= long.MaxValue - work.Recipe.CordYield &&
                world.Tick.Value <= long.MaxValue - work.Recipe.DurationTicks &&
                (state.Scheduler.Workflow == null || state.Scheduler.Workflow.Status == AdvanceStatus.Completed);
            var status = latest == null ? "idle" : running ? "running" : latest!.Status == WeavingActivityStatus.Completed ? "completed" : "cancelled";
            return new WeavingWorkView(qualified, work.SkillValue, work.Recipe.MinimumSkill, input.OnHand, input.Reserved,
                output.OnHand, status, world.Tick.Value, latest?.DueTick.Value ?? 0, canStart, running && actor.Alive,
                "Vật liệu cá nhân của bản thử mới; đan bằng tay. Không phải quyền dùng kho hộ hoặc toàn bộ hệ nghề nghiệp.");
        }
        public WeavingPreview Preview()
        {
            lock (gate)
            {
                var state = processor.InspectCommitted(); var work = Work(state); var recipe = state.World.Weaving!.Recipe;
                string? token = null;
                if (work.CanStart)
                {
                    // Presentation-only opaque capability, not gameplay RNG or an origin reroll.
                    token = Guid.NewGuid().ToString("N");
                    if (previews.Count >= 256) previews.Clear();
                    previews.Add(token, new Pin(state.World.Revision.Value, state.World.Tick.Value));
                }
                return new WeavingPreview(work.CanStart, work.CanStart ? "ok" : !work.Qualified ? "unsupported_origin" : "unavailable",
                    token, recipe.FiberCost, recipe.DurationTicks, recipe.CordYield);
            }
        }
        private static bool Valid(string? request) => request != null && Request.IsMatch(request);
        private static string CommandId(string request) => "weaving-client/" + request;
        private CommandEnvelope? Existing(string request)
        {
            if (requests.TryGetValue(request, out var found)) return found;
            var receipt = processor.InspectCommitted().AcceptedReceipts.Concat(processor.InspectCommitted().MetadataReceipts)
                .SingleOrDefault(r => r.CommandId == CommandId(request));
            return receipt?.OriginalEnvelope;
        }
        private bool Bound(CommandEnvelope e, string type, string correlation)
            => e.CommandType == type && e.IssuerId == origin.Principal.ToString() && e.SourceType == "Player" &&
                e.PayloadVersion == 1 && e.CorrelationId == correlation;
        private bool Field(CommandEnvelope e, string name, string value) => e.Payload.TryGetValue(name, out var actual) && Equals(actual, value);
        private bool StartPayload(CommandEnvelope e) => e.Payload.Count == 3 && Field(e, "actor_id", genesis.ActorId.ToString()) &&
            Field(e, "input_lot_id", genesis.InputLotId.ToString()) && Field(e, "output_lot_id", genesis.OutputLotId.ToString());
        private CommandEnvelope Envelope(string request, string type, string correlation, Dictionary<string, object?> payload)
        {
            var state = processor.InspectCommitted();
            return new CommandEnvelope(CommandId(request), type, origin.Principal.ToString(), "Player", state.World.Tick.Value,
                state.World.Revision.Value, 1, payload, correlation);
        }
        public OriginResult Start(string previewToken, string requestId)
        {
            lock (gate)
            {
                if (!Valid(requestId)) return new OriginResult(false, "invalid_request");
                var correlation = "weaving-client/start/" + previewToken;
                var old = Existing(requestId);
                // Durable original intent is checked before current token freshness, including after response loss/reopen.
                if (old != null) return Bound(old, "StartWeaving", correlation) && StartPayload(old) ? Execute(old, requestId) : new OriginResult(false, "request_mismatch");
                if (previewToken == null || !PreviewToken.IsMatch(previewToken) || !previews.TryGetValue(previewToken, out var pin))
                    return new OriginResult(false, "stale_preview");
                var state = processor.InspectCommitted();
                if (pin.Revision != state.World.Revision.Value || pin.Tick != state.World.Tick.Value) return new OriginResult(false, "stale_preview");
                if (!Work(state).CanStart) return new OriginResult(false, "unavailable");
                var command = Envelope(requestId, "StartWeaving", correlation, new Dictionary<string, object?> {
                    ["actor_id"] = genesis.ActorId.ToString(), ["input_lot_id"] = genesis.InputLotId.ToString(), ["output_lot_id"] = genesis.OutputLotId.ToString() });
                return Execute(command, requestId);
            }
        }
        public OriginResult Cancel(string requestId)
        {
            lock (gate)
            {
                if (!Valid(requestId)) return new OriginResult(false, "invalid_request");
                const string correlation = "weaving-client/cancel";
                var old = Existing(requestId);
                if (old != null)
                {
                    var valid = Bound(old, "CancelWeaving", correlation) && old.Payload.Count == 2 && Field(old, "reason", "player_cancel") &&
                        processor.InspectCommitted().World.Weaving!.Activities.Any(a => a.ActorId.Equals(origin.Principal) && Field(old, "activity_id", a.Id.ToString()));
                    return valid ? Execute(old, requestId) : new OriginResult(false, "request_mismatch");
                }
                var state = processor.InspectCommitted();
                var activity = state.World.Weaving!.Activities.SingleOrDefault(a => a.Status == WeavingActivityStatus.Running && a.ActorId.Equals(origin.Principal));
                if (activity == null || !Work(state).CanCancel) return new OriginResult(false, "unavailable");
                return Execute(Envelope(requestId, "CancelWeaving", correlation, new Dictionary<string, object?> {
                    ["activity_id"] = activity.Id.ToString(), ["reason"] = "player_cancel" }), requestId);
            }
        }
        public OriginResult AdvanceBy(long minutes, string requestId)
        {
            lock (gate)
            {
                if (!Valid(requestId) || minutes < 1 || minutes > 1440) return new OriginResult(false, "invalid_request");
                const string correlation = "weaving-client/advance";
                var old = Existing(requestId);
                if (old != null) return Bound(old, "AdvanceTime", correlation) && old.Payload.Count == 1 &&
                    old.Payload.TryGetValue("target_tick", out var target) && target is long tick && tick - old.WorldTick == minutes
                    ? Execute(old, requestId) : new OriginResult(false, "request_mismatch");
                long next;
                try { next = checked(processor.InspectCommitted().World.Tick.Value + minutes); }
                catch (OverflowException) { return new OriginResult(false, "unavailable"); }
                return Execute(Envelope(requestId, "AdvanceTime", correlation, new Dictionary<string, object?> { ["target_tick"] = next }), requestId);
            }
        }
        private OriginResult Execute(CommandEnvelope envelope, string request)
        {
            requests[request] = envelope;
            try
            {
                var principal = new ExecutionPrincipal(origin.Principal, CommandSource.Player);
                var receipt = processor.Execute(envelope, principal);
                if (receipt.Outcome == CommandOutcome.AcceptedPending && envelope.CommandType == "AdvanceTime")
                {
                    var flow = processor.InspectCommitted().Scheduler.Workflow;
                    if (flow != null && flow.Id == envelope.CommandId && flow.Status != AdvanceStatus.Completed)
                    {
                        processor.DrainAdvance(principal);
                        if (processor.InspectCommitted().Scheduler.Workflow?.Status != AdvanceStatus.Completed) return new OriginResult(true, "advance_pending", true);
                    }
                }
                bool success = receipt.Outcome != CommandOutcome.Rejected;
                return new OriginResult(success, success ? "ok" : receipt.ErrorCode == "CMD_PAYLOAD_MISMATCH" ? "request_mismatch" :
                    receipt.ErrorCode == "STALE_REVISION" || receipt.ErrorCode == "TIME_MISMATCH" ? "stale_preview" : "unavailable", success);
            }
            catch (Exception)
            {
                // A post-commit response can be lost. Reconcile durable truth before later retries or reads.
                try { var state = load(); ValidateBinding(state); var recovered = restore(state); if (!ReferenceEquals(recovered.InspectCommitted(), state)) throw new InvalidOperationException("Restore mismatch"); ResumeAcceptedAdvance(recovered); processor = recovered; previews.Clear(); } catch (Exception) { }
                return new OriginResult(false, "session_error");
            }
        }
        public OriginResult Save()
        {
            lock (gate)
            {
                try { snapshot(processor.InspectCommitted()); return new OriginResult(true, "saved", true); }
                catch (Exception) { return new OriginResult(false, "storage_error"); }
            }
        }
        public OriginResult Reload()
        {
            lock (gate)
            {
                try
                {
                    var state = load(); ValidateBinding(state); var restored = restore(state);
                    if (!ReferenceEquals(restored.InspectCommitted(), state)) throw new InvalidOperationException("Restore mismatch");
                    ResumeAcceptedAdvance(restored); processor = restored; previews.Clear(); return new OriginResult(true, "loaded", true);
                }
                catch (Exception) { return new OriginResult(false, "storage_error"); }
            }
        }
    }
}
