using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using Game.Application.CommandPipeline;
using Game.Application.Origins;
using Game.Domain.Kernel;
using Game.Domain.Origins;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using Game.Infrastructure.Persistence;

namespace Game.Infrastructure.Origins
{
    /// <summary>Create-new durable campaigns; no UI-chosen seed, actor, path or overwrite operation.</summary>
    public sealed class OriginCampaignGateway : IOriginCampaignGateway
    {
        private readonly string root;
        private readonly OriginCohortRegistry registry;
        private readonly Func<string> seedFactory;
        private readonly Action<string>? probe;
        private static readonly Regex Request = new Regex(@"\A[0-9a-f]{32}\z", RegexOptions.CultureInvariant);
        private static readonly Regex Published = new Regex(@"\Aorigin-([0-9]{10})-([0-9a-f]{32})\.db\z", RegexOptions.CultureInvariant);
        public OriginCampaignGateway(string root, OriginCohortRegistry registry, Func<string>? seedFactory = null, Action<string>? publicationProbe = null)
        {
            if (string.IsNullOrWhiteSpace(root)) throw new ArgumentException("Origin root required", nameof(root));
            this.root = Path.GetFullPath(root); this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
            this.seedFactory = seedFactory ?? EntropySeed; probe = publicationProbe;
        }
        private static string EntropySeed()
        {
            var bytes = new byte[32]; using var random = RandomNumberGenerator.Create(); random.GetBytes(bytes);
            return string.Concat(bytes.Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));
        }
        private string[] Catalog()
        {
            if (!Directory.Exists(root)) return Array.Empty<string>();
            var paths = Directory.EnumerateFiles(root, "origin-*.db", SearchOption.TopDirectoryOnly)
                .Where(p => Published.IsMatch(Path.GetFileName(p))).OrderBy(p => Path.GetFileName(p), StringComparer.Ordinal).ToArray();
            var matches = paths.Select(p => Published.Match(Path.GetFileName(p))).ToArray();
            if (matches.Any(m => m.Groups[1].Value == "0000000000") ||
                matches.Select(m => m.Groups[1].Value).Distinct(StringComparer.Ordinal).Count() != paths.Length ||
                matches.Select(m => m.Groups[2].Value).Distinct(StringComparer.Ordinal).Count() != paths.Length)
                throw new InvalidDataException("ORIGIN_CATALOG_AMBIGUOUS");
            return paths;
        }
        public OriginMenuView ReadMenu()
        {
            try
            {
                var paths = Catalog(); if (paths.Length == 0) return new OriginMenuView(false);
                _ = LoadVerified(Store(paths[paths.Length - 1]));
                return new OriginMenuView(true);
            }
            catch (Exception) { return new OriginMenuView(false, "storage_error"); }
        }
        private static OriginOpenResult Failure(string code) => new OriginOpenResult(new OriginResult(false, code));
        public OriginOpenResult Continue()
        {
            try { var paths = Catalog(); return paths.Length == 0 ? Failure("no_campaign") : Open(paths[paths.Length - 1]); }
            catch (Exception) { return Failure("storage_error"); }
        }
        public OriginOpenResult NewCampaign(string requestId, CancellationToken cancellationToken)
        {
            if (requestId == null || !Request.IsMatch(requestId)) return Failure("invalid_request");
            string? pending = null; bool ownsPending = false;
            try
            {
                cancellationToken.ThrowIfCancellationRequested(); Directory.CreateDirectory(root);
                // One writer chooses catalog sequence. The file is only a lock, never gameplay entropy.
                using var writer = new FileStream(Path.Combine(root, ".origin-create.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                var catalog = Catalog();
                var existing = catalog.Where(p => Published.Match(Path.GetFileName(p)).Groups[2].Value == requestId).ToArray();
                if (existing.Length > 1) return Failure("storage_error");
                if (existing.Length == 1) return Open(existing[0]);
                long sequence = catalog.Length == 0 ? 1 : checked(long.Parse(Published.Match(Path.GetFileName(catalog[catalog.Length - 1])).Groups[1].Value, CultureInfo.InvariantCulture) + 1);
                if (sequence > 9999999999L) return Failure("storage_error");
                var destination = Path.Combine(root, "origin-" + sequence.ToString("D10", CultureInfo.InvariantCulture) + "-" + requestId + ".db");
                var seed = seedFactory();
                var selection = OriginSelector.Select(seed, registry.SourceContentHash, registry.ContentHash, registry.Candidates);
                var initial = Initial(selection);
                pending = Path.Combine(root, ".pending-" + Guid.NewGuid().ToString("N") + ".db");
                probe?.Invoke("before_initialize"); cancellationToken.ThrowIfCancellationRequested();
                // InitializeSave itself uses CreateNew. Nothing published points at this private path.
                if (File.Exists(pending)) throw new IOException("Pending path exists");
                var store = new SqliteWorldStore(pending, registry.ContentHash, probe: phase => { if (phase == "after_create") ownsPending = true; }, expectedRuleVersion: SqliteWorldStore.OriginRuleVersion);
                store.InitializeSave(initial, seed);
                probe?.Invoke("after_initialize"); cancellationToken.ThrowIfCancellationRequested();
                _ = LoadVerified(store);
                using (var flush = new FileStream(pending, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) flush.Flush(true);
                probe?.Invoke("before_publish"); cancellationToken.ThrowIfCancellationRequested();
                File.Move(pending, destination); ownsPending = false; pending = null;
                probe?.Invoke("after_publish");
                // Cancellation after publication cannot erase the committed campaign.
                return Open(destination);
            }
            catch (OperationCanceledException) { return Failure("cancelled"); }
            catch (Exception) { return Failure("storage_error"); }
            finally
            {
                if (ownsPending && pending != null)
                {
                    // Cleanup only the private file chosen by this operation. Crash leftovers are ignored, not scanned/deleted.
                    try { if (File.Exists(pending)) File.Delete(pending); } catch (IOException) { } catch (UnauthorizedAccessException) { }
                }
            }
        }
        private SqliteWorldStore Store(string path) => new SqliteWorldStore(path, registry.ContentHash, expectedRuleVersion: SqliteWorldStore.OriginRuleVersion);
        private CommittedWorld LoadVerified(SqliteWorldStore store)
        {
            var state = store.Load(); var origin = state.World.Origin ?? throw new InvalidDataException("ORIGIN_MISSING");
            var seed = store.GetSaveInfo().WorldSeed;
            if (seed == null || origin.SourceContentHash != registry.SourceContentHash || origin.CohortContentHash != registry.ContentHash ||
                state.World.Repair != null || state.World.Actors.Count != 1 || state.World.Lots.Count != 0 ||
                state.World.GetActor(origin.Principal).Location.ToString() != origin.Home.ToString())
                throw new InvalidDataException("ORIGIN_BINDING");
            var expected = OriginSelector.Select(seed, registry.SourceContentHash, registry.ContentHash, registry.Candidates);
            var e = expected.Origin;
            if (state.World.WorldId != OriginSelector.DeriveWorldId(seed) || origin.ActorDefinitionId != e.ActorDefinitionId ||
                origin.HouseholdDefinitionId != e.HouseholdDefinitionId || origin.CommunityDefinitionId != e.CommunityDefinitionId ||
                origin.Principal.ToString() != e.Principal.ToString() || origin.Household.ToString() != e.Household.ToString() ||
                origin.Home.ToString() != e.Home.ToString() || origin.DrawIndex != e.DrawIndex ||
                state.World.RngStates.Count != 1 || state.World.RngStates[0].State.Context.Seed != seed ||
                state.World.RngStates[0].State.DrawIndex != expected.Rng.State.DrawIndex)
                throw new InvalidDataException("ORIGIN_ASSIGNMENT");
            return state;
        }
        private OriginOpenResult Open(string path)
        {
            var store = Store(path); var state = LoadVerified(store); var record = registry.Get(state.World.Origin!.ActorDefinitionId);
            CommandProcessor Restore(CommittedWorld value) => new CommandProcessor(value, new RuleRegistry(new[] { new TransferResourceRule() }), store);
            var session = new OriginCampaignSession(Restore(state), record.Reveal, () => LoadVerified(store), Restore, store.CreateSnapshot);
            return new OriginOpenResult(new OriginResult(true, "ok", true), session);
        }
        private static CommittedWorld Initial(OriginSelection selection)
        {
            var origin = selection.Origin; var stage = IdentitySnapshot.Empty(origin.Principal.Value.World).BeginStage(1);
            var actor = stage.Allocate<Actor>(); _ = stage.Allocate<Household>(); var home = stage.Allocate<Settlement>();
            var world = new WorldSnapshot(stage.PrepareSnapshot(), new WorldTick(0),
                new[] { new ActorState(actor, true, home) }, Array.Empty<ResourceLot>(), new[] { selection.Rng }, origin: origin);
            return new CommandProcessor(world, new RuleRegistry(new[] { new TransferResourceRule() })).InspectCommitted();
        }
    }
}
