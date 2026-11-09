using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Game.Application.Origins;
using Game.Domain.Origins;
using Game.Foundation.Random;
using Game.Infrastructure.Origins;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.SettlementContent;

namespace Game.Headless
{
    internal static class OriginProbe
    {
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions { WriteIndented = true, IncludeFields = true };
        internal static int Run(string[] args)
        {
            try
            {
                if (args.Length < 2) return Usage();
                var document = OriginCohortCompiler.FromSettlement(SettlementCompiler.Compile(SettlementJson.Read(args[1])));
                var registry = OriginCohortCompiler.Compile(document);
                if (args[0] == "--origin-export" && args.Length == 2)
                { Console.WriteLine(JsonSerializer.Serialize(document, Json)); return 0; }
                if (args[0] == "--origin-open" && args.Length == 3)
                {
                    var opened = new OriginCampaignGateway(args[2], registry).Continue();
                    var truth = opened.Result.Success ? Inspect(args[2], registry) : null;
                    Console.WriteLine(JsonSerializer.Serialize(new { scope = "trusted-origin-diagnostic-not-player-DTO",
                        passed = opened.Result.Success, code = opened.Result.Code, reveal = opened.Session?.Read(), truth }, Json));
                    return opened.Result.Success ? 0 : 1;
                }
                if (args[0] == "--origin-crash" && args.Length == 7)
                {
                    var phase = args[5]; var ready = args[6];
                    if (!new[] { "before_initialize", "after_initialize", "before_publish", "after_publish" }.Contains(phase)) return Usage();
                    var gateway = new OriginCampaignGateway(args[2], registry, () => args[3], point =>
                    {
                        if (point != phase) return;
                        File.WriteAllText(ready, point);
                        // Deliberate trusted QA pause; never a gameplay clock.
                        while (true) Thread.Sleep(1000);
                    });
                    var result = gateway.NewCampaign(args[4], CancellationToken.None);
                    Console.WriteLine(JsonSerializer.Serialize(new { passed = result.Result.Success, code = result.Result.Code }, Json));
                    return 3; // Parent must SIGKILL at the observed fault hook.
                }
                if (args[0] != "--origin-probe" || args.Length != 5) return Usage();
                var root = Path.GetFullPath(args[2]); var seed = args[3]; var request = args[4];
                int seedCalls = 0;
                int priorFiles = Directory.Exists(root) ? Directory.GetFiles(root, "origin-*.db").Length : 0;
                var service = new OriginCampaignGateway(root, registry, () => { seedCalls++; return seed; });
                var created = service.NewCampaign(request, CancellationToken.None);
                Require(created.Result.Success && created.Result.Durable && created.Session != null, "NEW");
                var session = created.Session!; var first = session.Read();
                var path = Latest(root); var store = Store(path, registry); var initial = store.Load();
                var initialBytes = CommittedWorldCodec.Encode(initial); var initialHash = CommittedWorldCodec.Hash(initialBytes);
                var origin = initial.World.Origin ?? throw new InvalidDataException("MISSING_ORIGIN");
                var expected = OriginSelector.Select(seed, registry.SourceContentHash, registry.ContentHash, registry.Candidates);
                bool binding = origin.ActorDefinitionId == expected.Origin.ActorDefinitionId && initial.World.WorldId == OriginSelector.DeriveWorldId(seed);
                var again = service.NewCampaign(request, CancellationToken.None);
                bool idempotentNew = again.Result.Success && again.Session!.Read().ActorName == first.ActorName && seedCalls == 1
                    && Directory.GetFiles(root, "origin-*.db").Length == priorFiles + 1;
                var beforeRead = CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(store.Load()));
                for (int i = 0; i < 32; i++) _ = session.Read();
                bool pureRead = beforeRead == CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(store.Load()));
                Require(session.AdvanceBy(1, "probe-one").Success, "WAIT_ONE");
                Require(session.Save().Success, "SAVE");
                var reopened = new OriginCampaignGateway(root, registry).Continue();
                Require(reopened.Result.Success && reopened.Session != null, "CONTINUE");
                session = reopened.Session!;
                Require(session.AdvanceBy(1, "probe-one").Success && session.Read().Tick == 1, "ORIGINAL_RETRY");
                Require(session.AdvanceBy(60, "probe-sixty").Success && session.Read().Tick == 61, "WAIT_SIXTY");
                Require(session.Save().Success && session.Reload().Success, "SAVE_RELOAD");
                var resumed = new OriginCampaignGateway(root, registry).Continue();
                Require(resumed.Result.Success && resumed.Session != null, "RESUME");
                Require(resumed.Session!.AdvanceBy(60, "probe-sixty").Success, "REOPEN_RETRY");
                var final = store.Load(); var view = resumed.Session.Read();
                bool retained = view.ActorName == first.ActorName && view.Tick == 61
                    && final.World.Origin!.ActorDefinitionId == origin.ActorDefinitionId
                    && final.World.Origin.DrawIndex == origin.DrawIndex && final.World.RngStates.Single().State.DrawIndex == origin.DrawIndex;
                bool foreignRefused = false; var fileBefore = File.ReadAllBytes(path);
                try { _ = new SqliteWorldStore(path, registry.ContentHash).Load(); }
                catch (InvalidDataException) { foreignRefused = true; }
                bool unchanged = fileBefore.SequenceEqual(File.ReadAllBytes(path));
                bool passed = binding && idempotentNew && pureRead && retained && foreignRefused && unchanged;
                Console.WriteLine(JsonSerializer.Serialize(new { scope = "headless-origin-cohort-not-full-lifegen-or-viability",
                    passed, profile = "origin-cohort-v1", contentHash = registry.ContentHash, sourceHash = registry.SourceContentHash,
                    actor = origin.ActorDefinitionId, actorName = view.ActorName, world = final.World.WorldId.ToString("x16"),
                    initialHash, finalHash = CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(final)),
                    tick = view.Tick, drawIndex = final.World.RngStates.Single().State.DrawIndex, candidateCount = registry.Candidates.Count,
                    binding, idempotentNew, pureRead, retained, foreignRefused, unchanged, seedCalls,
                    publishedFiles = Directory.GetFiles(root, "origin-*.db").Length }, Json));
                return passed ? 0 : 1;
            }
            catch (Exception error)
            { Console.Error.WriteLine(JsonSerializer.Serialize(new { passed = false, error = error.GetType().Name + ": " + error.Message }, Json)); return 1; }
        }
        private static SqliteWorldStore Store(string path, OriginCohortRegistry registry)
            => new SqliteWorldStore(path, registry.ContentHash, expectedRuleVersion: SqliteWorldStore.OriginRuleVersion);
        private static string Latest(string root) => Directory.GetFiles(root, "origin-*.db").OrderBy(x => x, StringComparer.Ordinal).Last();
        private static object Inspect(string root, OriginCohortRegistry registry)
        {
            var path = Latest(root); var store = Store(path, registry); var state = store.Load();
            return new { world = state.World.WorldId.ToString("x16"), tick = state.World.Tick.Value,
                actor = state.World.Origin!.ActorDefinitionId, drawIndex = state.World.RngStates.Single().State.DrawIndex,
                contentHash = registry.ContentHash, sourceHash = registry.SourceContentHash,
                hash = CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(state)), seed = store.GetSaveInfo().WorldSeed };
        }
        private static void Require(bool value, string message) { if (!value) throw new InvalidOperationException("ORIGIN_PROBE_" + message); }
        private static int Usage()
        { Console.Error.WriteLine("Usage: --origin-export <pack> | --origin-probe <pack> <new-root> <seed> <request-id> | --origin-open <pack> <root> | --origin-crash <pack> <root> <seed> <request-id> <phase> <ready>"); return 2; }
    }
}
