using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using Game.Application.CommandPipeline;
using Game.Application.Origins;
using Game.Application.Weaving;
using Game.Domain.Kernel;
using Game.Infrastructure.Origins;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.SettlementContent;
using Game.Infrastructure.Weaving;

namespace Game.Headless
{
    internal static class WeavingProbe
    {
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow };
        internal static int Run(string[] args)
        {
            try
            {
                if (args.Length < 4) return Usage();
                var cohort = OriginCohortCompiler.Compile(OriginCohortCompiler.FromSettlement(SettlementCompiler.Compile(SettlementJson.Read(args[1]))));
                var document = JsonSerializer.Deserialize<WeavingProfileDocument>(File.ReadAllText(args[2]), Json) ?? throw new InvalidDataException("Parameters missing");
                var registry = WeavingContentRegistry.Compile(cohort, document);
                var root = Path.GetFullPath(args[3]);
                if (args[0] == "--weaving-inspect" && args.Length == 4)
                {
                    var state = Store(root, registry).Load(); var weaving = state.World.Weaving!;
                    var last = weaving.Activities.OrderByDescending(a => a.Id.Value.Counter).FirstOrDefault();
                    var status = last == null ? "idle" : last.Status.ToString().ToLowerInvariant();
                    Print(new { passed = true, scope = "trusted raw persisted inspection before recovery", work = new {
                        FiberOnHand = state.World.GetLot(weaving.InputLotId).OnHand, FiberReserved = state.World.GetLot(weaving.InputLotId).Reserved,
                        CordOnHand = state.World.GetLot(weaving.OutputLotId).OnHand, Status = status, Tick = state.World.Tick.Value,
                        DueTick = last?.DueTick.Value ?? 0 }, truth = Truth(root, registry) }); return 0;
                }
                if (args[0] == "--weaving-open" && args.Length == 4)
                {
                    var result = new WeavingCampaignGateway(root, registry).Continue();
                    Print(new { passed = result.Result.Success, code = result.Result.Code, origin = result.Session?.Read(), work = result.Session?.ReadWork(), truth = result.Result.Success ? Truth(root, registry) : null });
                    return result.Result.Success ? 0 : 1;
                }
                if (args[0] == "--weaving-crash" && args.Length == 8)
                {
                    var phase = args[6];
                    if (!new[] { "after_initialize", "before_publish", "after_publish" }.Contains(phase)) return Usage();
                    var gateway = new WeavingCampaignGateway(root, registry, () => args[4], point => { if (point == phase) Pause(args[7], point); });
                    _ = gateway.NewCampaign(args[5], CancellationToken.None); return 3;
                }
                if (args[0] == "--weaving-action-crash" && args.Length == 7)
                {
                    var phase = args[4]; var action = args[6];
                    if (!new[] { "before_commit", "after_commit" }.Contains(phase) || !new[] { "start", "complete", "cancel" }.Contains(action)) return Usage();
                    Require(new WeavingCampaignGateway(root, registry).Continue().Result.Success, "VERIFIED_OPEN");
                    bool armed = action != "complete";
                    var store = new SqliteWorldStore(Latest(root), registry.ContentHash, probe: point => { if (armed && point == phase) Pause(args[5], point); }, expectedRuleVersion: SqliteWorldStore.WeavingRuleVersion);
                    var state = store.Load(); var w = state.World; var work = w.Weaving!;
                    var processor = new CommandProcessor(state, new RuleRegistry(new[] { new TransferResourceRule() }), store, weavingRecipe: registry.Recipe);
                    var principal = new ExecutionPrincipal(w.Origin!.Principal, CommandSource.Player);
                    CommandEnvelope command;
                    if (action == "start")
                        command = new CommandEnvelope("weaving-client/crash-start", "StartWeaving", principal.ActorId.ToString(), "Player", w.Tick.Value, w.Revision.Value, 1,
                            new Dictionary<string, object?> { ["actor_id"] = principal.ActorId.ToString(), ["input_lot_id"] = work.InputLotId.ToString(), ["output_lot_id"] = work.OutputLotId.ToString() }, "weaving-client/start/" + new string('c', 32));
                    else if (action == "cancel")
                        command = new CommandEnvelope("weaving-client/crash-cancel", "CancelWeaving", principal.ActorId.ToString(), "Player", w.Tick.Value, w.Revision.Value, 1,
                            new Dictionary<string, object?> { ["activity_id"] = work.Activities.Single(a => a.ReservedQuantity != 0).Id.ToString(), ["reason"] = "player_cancelled" }, "weaving-client/cancel");
                    else
                        command = new CommandEnvelope("weaving-client/crash-advance", "AdvanceTime", principal.ActorId.ToString(), "Player", w.Tick.Value, w.Revision.Value, 1,
                            new Dictionary<string, object?> { ["target_tick"] = 30L }, "weaving-client/advance");
                    var receipt = processor.Execute(command, principal);
                    if (action == "complete") { Require(receipt.Outcome == CommandOutcome.AcceptedPending, "ADVANCE_PENDING"); armed = true; processor.DrainAdvance(principal); }
                    return 3;
                }
                if (args[0] == "--weaving-retry-start" && args.Length == 4)
                {
                    var opened = new WeavingCampaignGateway(root, registry).Continue(); Require(opened.Result.Success, "OPEN");
                    var before = Hash(root, registry); var result = opened.Session!.Start(new string('c', 32), "crash-start");
                    bool same = before == Hash(root, registry);
                    Print(new { passed = result.Success && same, result, unchanged = same, work = opened.Session.ReadWork(), truth = Truth(root, registry) });
                    return result.Success && same ? 0 : 1;
                }
                if ((args[0] == "--weaving-new" || args[0] == "--weaving-probe") && (args.Length == 6 || args.Length == 7))
                {
                    int seedCalls = 0; var seed = args[4]; var request = args[5];
                    var gateway = new WeavingCampaignGateway(root, registry, () => { seedCalls++; return seed; });
                    var opened = gateway.NewCampaign(request, CancellationToken.None); Require(opened.Result.Success && opened.Result.Durable, "NEW");
                    var session = opened.Session!; var origin = session.Read(); var initial = session.ReadWork(); var initialHash = Hash(root, registry);
                    var duplicate = gateway.NewCampaign(request, CancellationToken.None);
                    Require(duplicate.Result.Success && seedCalls == 1, "IDEMPOTENT_NEW");
                    if (args[0] == "--weaving-new")
                    { Print(new { passed = true, origin, work = initial, truth = Truth(root, registry), initialHash }); return 0; }
                    if (args.Length != 7) return Usage();
                    var mode = args[6]; if (!new[] { "complete", "cancel", "blocked", "running", "before-due" }.Contains(mode)) return Usage();
                    for (int i = 0; i < 16; i++) { _ = session.Read(); _ = session.ReadWork(); _ = session.Preview(); }
                    Require(initialHash == Hash(root, registry), "PURE_PREVIEW");
                    var preview = session.Preview();
                    if (mode == "blocked")
                    {
                        Require(!initial.Qualified && !initial.CanStart && !preview.Allowed, "NONWEAVER");
                        Require(!session.Start(preview.Token ?? "invalid", "probe-start").Success, "DENIED_START");
                        Require(initialHash == Hash(root, registry), "DENIED_PURE");
                    }
                    else
                    {
                        Require(initial.Qualified && preview.Allowed && preview.Token != null, "QUALIFIED");
                        Require(session.Start(preview.Token!, "probe-start").Success, "START");
                        var started = session.ReadWork(); Require(started.Status == "running" && started.FiberOnHand == 8 && started.FiberReserved == 2 && started.CordOnHand == 0 && started.DueTick == 30, "RUNNING");
                        var runningHash = Hash(root, registry);
                        Require(session.Start(preview.Token!, "probe-start").Success && runningHash == Hash(root, registry), "START_RETRY");
                        Require(!session.Start(new string('d', 32), "probe-start").Success && runningHash == Hash(root, registry), "CHANGED_TOKEN");
                        Require(session.Save().Success, "SAVE_RUNNING");
                        var reopened = new WeavingCampaignGateway(root, registry).Continue(); Require(reopened.Result.Success, "REOPEN_RUNNING"); session = reopened.Session!;
                        Require(session.ReadWork().Status == "running" && session.Start(preview.Token!, "probe-start").Success, "DURABLE_RETRY");
                        if (mode == "cancel")
                        {
                            Require(session.Cancel("probe-cancel").Success, "CANCEL");
                            var cancelledHash = Hash(root, registry); Require(session.Cancel("probe-cancel").Success && cancelledHash == Hash(root, registry), "CANCEL_RETRY");
                            Require(session.AdvanceBy(60, "probe-after-cancel").Success, "WAIT_CANCELLED");
                            var cancelled = session.ReadWork(); Require(cancelled.Status == "cancelled" && cancelled.FiberOnHand == 8 && cancelled.FiberReserved == 0 && cancelled.CordOnHand == 0 && cancelled.Tick == 60, "CANCEL_NO_OUTPUT");
                        }
                        else if (mode != "running")
                        {
                            Require(session.AdvanceBy(29, "probe-before-due").Success, "WAIT_29");
                            var early = session.ReadWork(); Require(early.Tick == 29 && early.Status == "running" && early.FiberOnHand == 8 && early.FiberReserved == 2 && early.CordOnHand == 0, "BEFORE_DUE");
                            if (mode == "complete")
                            {
                                Require(session.AdvanceBy(1, "probe-due").Success, "WAIT_DUE");
                                var complete = session.ReadWork(); Require(complete.Tick == 30 && complete.Status == "completed" && complete.FiberOnHand == 6 && complete.FiberReserved == 0 && complete.CordOnHand == 1, "COMPLETION");
                                Require(session.AdvanceBy(1, "probe-due").Success && session.ReadWork().Tick == 30, "DUE_RETRY");
                            }
                        }
                    }
                    Require(session.Save().Success && session.Reload().Success, "SAVE_RELOAD");
                    var resumed = new WeavingCampaignGateway(root, registry).Continue(); Require(resumed.Result.Success, "CONTINUE");
                    Require(resumed.Session!.Read().ActorName == origin.ActorName && resumed.Session.Read().Age == origin.Age, "SAME_ORIGIN");
                    var state = Store(root, registry).Load(); Require(state.World.RngStates.Single().State.DrawIndex == 1, "NO_NEW_RNG");
                    bool refused = false; var bytes = File.ReadAllBytes(Latest(root));
                    try { _ = new SqliteWorldStore(Latest(root), cohort.ContentHash, expectedRuleVersion: SqliteWorldStore.OriginRuleVersion).Load(); }
                    catch (InvalidDataException) { refused = true; }
                    Require(refused && bytes.SequenceEqual(File.ReadAllBytes(Latest(root))), "LEGACY_REFUSAL");
                    Print(new { passed = true, mode, profile = "personal-weaving-v1", initialHash, finalHash = Hash(root, registry), seedCalls, purePreview = true, origin = resumed.Session.Read(), work = resumed.Session.ReadWork(), truth = Truth(root, registry) });
                    return 0;
                }
                return Usage();
            }
            catch (Exception error) { Console.Error.WriteLine(JsonSerializer.Serialize(new { passed = false, error = error.GetType().Name + ": " + error.Message }, Json)); return 1; }
        }
        private static SqliteWorldStore Store(string root, WeavingContentRegistry registry) => new SqliteWorldStore(Latest(root), registry.ContentHash, expectedRuleVersion: SqliteWorldStore.WeavingRuleVersion);
        private static string Latest(string root) => Directory.GetFiles(root, "origin-*.db").OrderBy(x => x, StringComparer.Ordinal).Last();
        private static string Hash(string root, WeavingContentRegistry registry) => CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(Store(root, registry).Load()));
        private static object Truth(string root, WeavingContentRegistry registry)
        {
            var store = Store(root, registry); var s = store.Load(); var o = s.World.Origin!;
            return new { world = s.World.WorldId.ToString("x16"), tick = s.World.Tick.Value, actor = o.ActorDefinitionId, draw = s.World.RngStates.Single().State.DrawIndex,
                contentHash = registry.ContentHash, cohortHash = o.CohortContentHash, sourceHash = o.SourceContentHash, hash = Hash(root, registry), seed = store.GetSaveInfo().WorldSeed,
                accepted = s.AcceptedReceipts.Count, commands = s.AcceptedReceipts.Select(r => r.OriginalEnvelope.CommandType).ToArray() };
        }
        private static void Pause(string file, string phase) { File.WriteAllText(file, phase); while (true) Thread.Sleep(1000); }
        private static void Require(bool result, string label) { if (!result) throw new InvalidDataException("WEAVING_PROBE_" + label); }
        private static void Print(object value) => Console.WriteLine(JsonSerializer.Serialize(value, Json));
        private static int Usage() { Console.Error.WriteLine("Trusted QA: --weaving-new/probe/open/crash/action-crash/retry-start <pack> <parameters> <root> [args]"); return 2; }
    }
}
