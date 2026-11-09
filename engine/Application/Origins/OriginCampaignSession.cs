using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Application.CommandPipeline;
using Game.Application.SimulationScheduler;
using Game.Domain.Origins;

namespace Game.Application.Origins
{
    /// <summary>Trusted composition retains world/processor; presentation receives IOriginCampaignSession only.</summary>
    public sealed class OriginCampaignSession : IOriginCampaignSession
    {
        private readonly object gate = new object();
        private CommandProcessor processor;
        private readonly CampaignOriginState origin;
        private readonly Func<long, OriginRevealView> project;
        private readonly Func<CommittedWorld> load;
        private readonly Func<CommittedWorld, CommandProcessor> restore;
        private readonly Action<CommittedWorld> snapshot;
        private readonly Dictionary<string, CommandEnvelope> requests = new Dictionary<string, CommandEnvelope>(StringComparer.Ordinal);
        private static readonly Regex Token = new Regex(@"\A[a-zA-Z0-9][a-zA-Z0-9_.-]{0,79}\z", RegexOptions.CultureInvariant);
        public OriginCampaignSession(CommandProcessor processor, Func<long, OriginRevealView> project,
            Func<CommittedWorld> load, Func<CommittedWorld, CommandProcessor> restore, Action<CommittedWorld> snapshot)
        {
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
            origin = processor.InspectCommitted().World.Origin ?? throw new ArgumentException("Origin profile required");
            this.project = project ?? throw new ArgumentNullException(nameof(project));
            this.load = load ?? throw new ArgumentNullException(nameof(load));
            this.restore = restore ?? throw new ArgumentNullException(nameof(restore));
            this.snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        }
        public OriginRevealView Read() { lock (gate) return project(processor.InspectCommitted().World.Tick.Value); }
        public OriginResult AdvanceBy(long minutes, string requestId)
        {
            lock (gate)
            {
                if (requestId == null || !Token.IsMatch(requestId) || minutes < 1 || minutes > 1440) return new OriginResult(false, "invalid_request");
                var state = processor.InspectCommitted(); var id = "origin-client/" + requestId;
                if (!requests.TryGetValue(requestId, out var envelope))
                {
                    var receipt = state.AcceptedReceipts.Concat(state.MetadataReceipts).SingleOrDefault(r => r.CommandId == id);
                    if (receipt != null) envelope = receipt.OriginalEnvelope;
                    else
                    {
                        long target;
                        try { target = checked(state.World.Tick.Value + minutes); }
                        catch (OverflowException) { return new OriginResult(false, "unavailable"); }
                        envelope = new CommandEnvelope(id, "AdvanceTime", origin.Principal.ToString(), "Player",
                            state.World.Tick.Value, state.World.Revision.Value, 1,
                            new Dictionary<string, object?> { ["target_tick"] = target }, "origin-client");
                    }
                    requests.Add(requestId, envelope);
                }
                if (envelope.CommandType != "AdvanceTime" || envelope.IssuerId != origin.Principal.ToString() ||
                    envelope.SourceType != "Player" || envelope.PayloadVersion != 1 || envelope.CorrelationId != "origin-client" ||
                    envelope.Payload.Count != 1 || !envelope.Payload.TryGetValue("target_tick", out var value) || value is not long tick ||
                    tick - envelope.WorldTick != minutes)
                    return new OriginResult(false, "request_mismatch");
                try
                {
                    var principal = new ExecutionPrincipal(origin.Principal, CommandSource.Player);
                    var receipt = processor.Execute(envelope, principal);
                    if (receipt.Outcome == CommandOutcome.AcceptedPending) processor.DrainAdvance(principal);
                    return new OriginResult(receipt.Outcome != CommandOutcome.Rejected,
                        receipt.Outcome == CommandOutcome.Rejected ? "unavailable" : "ok", receipt.Outcome != CommandOutcome.Rejected);
                }
                catch (Exception) { return new OriginResult(false, "session_error"); }
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
                    var state = load(); var next = state.World.Origin;
                    if (next == null || next.Principal.ToString() != origin.Principal.ToString() ||
                        next.ActorDefinitionId != origin.ActorDefinitionId || next.CohortContentHash != origin.CohortContentHash ||
                        next.SourceContentHash != origin.SourceContentHash || next.DrawIndex != origin.DrawIndex)
                        return new OriginResult(false, "storage_error");
                    var restored = restore(state); processor = restored;
                    return new OriginResult(true, "loaded", true);
                }
                catch (Exception) { return new OriginResult(false, "storage_error"); }
            }
        }
    }
}
