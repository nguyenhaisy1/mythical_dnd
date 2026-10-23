using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Infrastructure.Content;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Replay;
using Microsoft.Data.Sqlite;

namespace Game.Infrastructure.Conformance
{
    public sealed class ConformanceResult
    {
        public IReadOnlyList<string> Checks { get; }
        public string Fingerprint { get; }
        public string SqliteVersion { get; }
        internal ConformanceResult(IEnumerable<string> checks, string sqliteVersion)
        {
            Checks = Array.AsReadOnly(checks.OrderBy(x => x, StringComparer.Ordinal).ToArray());
            Fingerprint = CanonicalCodec.Hash(Checks); SqliteVersion = sqliteVersion;
        }
    }
    /// <summary>Same portable assembly entry point used by headless host and actual Unity Editor; no Unity dependency.</summary>
    public static class RuntimeConformance
    {
        public const string Profile = "kernel-conformance-v1";
        private static void Require(bool value, string code) { if (!value) throw new InvalidDataException(code); }
        public static ConformanceResult Run(string replayArtifactPath, string newDatabasePath)
        {
            Require(!File.Exists(newDatabasePath), "CONFORMANCE_REQUIRES_NEW_DATABASE");
            var checks = new List<string>();
            var codec = CanonicalCodec.Hash(new Dictionary<string, object?> { ["name"] = "Vạc Mộc", ["tick"] = 60L });
            Require(codec == "9a796de42d57bbbfac0ee99fe56873345e7078678f5fc628d4215d0b663ac845", "CONFORMANCE_CODEC");
            checks.Add("codec:" + codec);
            var seed = new string('0', 64);
            var context = new RngContext(seed, "foundation", "fixture", "command_1", EntityId.Parse("w0000000000000001/actor/0000000000000001"), "resolution");
            var words = context.Block(0);
            Require(words.SequenceEqual(new uint[] { 3506563779, 778542440, 2081392988, 3413229830, 2897818418, 2296882002, 469206908, 2995364543 }), "CONFORMANCE_RNG");
            checks.Add("rng:" + string.Join(",", words.Select(x => x.ToString(CultureInfo.InvariantCulture))));
            byte[] bytes;
            using (var file = new FileStream(replayArtifactPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long length = file.Length;
                Require(length <= 1024 * 1024, "CONFORMANCE_FIXTURE_SIZE");
                bytes = new byte[checked((int)length)]; int offset = 0;
                while (offset < bytes.Length) { int count = file.Read(bytes, offset, bytes.Length - offset); Require(count > 0, "CONFORMANCE_TRUNCATED"); offset += count; }
                Require(file.ReadByte() == -1, "CONFORMANCE_FILE_CHANGED");
            }
            Require(CommittedWorldCodec.Hash(bytes) == "bc82e0428cfd260d93b6a62a4eb09e472e73c2e15f8b7dab2b2f27d536423d2b", "CONFORMANCE_FIXTURE_HASH");
            var artifact = ReplayArtifactCodec.Decode(bytes);
            var rules = new RuleRegistry(new[] { new TransferResourceRule() });
            var replay = ReplayHarness.Replay(artifact, new ReplayManifest("e1ec516b24221e6820ab3bb2cea3cad8c874a98872844a89902497158dec4848", worldSeed: seed), rules);
            var replayHash = CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(replay.FinalState));
            Require(replay.Success && replayHash == "f89945aab9028bd7e847f4f30cca5f0b9d397486ac42d25eb266ac1ebfb7644c", "CONFORMANCE_REPLAY");
            checks.Add("replay:" + replayHash);
            const string source = "prototype-authored: FND14 kernel transfer fixture";
            var registry = ContentCompiler.Compile(new ContentPackInput("kernel.demo", 1, 1, ContentCompiler.RuleVersion,
                Array.Empty<ContentDependency>(), new[] {
                    new ContentDefinitionInput("commodity.timber@1", 1, "commodity", 1, source, "game-canon", "Assets", "commodity.timber", unit: "count"),
                    new ContentDefinitionInput("transfer.timber@1", 1, "transfer-template", 1, source, "game-canon", "Assets", "transfer.timber", commodityId: "commodity.timber@1", handler: "TransferResource@1", quantity: 3)
                }));
            Require(registry.ContentHash == "e45fa90669459e4516323a71ff959d7f9b6dd637d7e33c148306ba0cb9808b72", "CONFORMANCE_CONTENT");
            checks.Add("content:" + registry.ContentHash);
            var initial = CommittedWorldCodec.Decode(artifact.InitialImage); registry.ValidateWorld(initial.World);
            var store = new SqliteWorldStore(newDatabasePath, registry.ContentHash); store.InitializeSave(initial, seed);
            var processor = new CommandProcessor(store.Load(), registry.CreateRuleRegistry(), store);
            var action = artifact.Actions[1];
            var accepted = processor.Execute(action.Envelope!, action.Principal);
            Require(accepted.Outcome == CommandOutcome.Accepted, "CONFORMANCE_COMMIT");
            var reopened = new SqliteWorldStore(newDatabasePath, registry.ContentHash).Load();
            Require(reopened.World.Lots.Select(x => x.OnHand).SequenceEqual(new long[] { 7, 3 }) && reopened.World.Revision.Value == 2, "CONFORMANCE_REOPEN");
            var image = CommittedWorldCodec.Encode(reopened);
            var snapshotCount = store.GetSaveInfo().SnapshotCount;
            store.CreateSnapshot(reopened);
            Require(store.GetSaveInfo().SnapshotCount == snapshotCount + 1, "CONFORMANCE_SNAPSHOT_PERSISTED");
            Require(CommittedWorldCodec.Encode(store.Load()).SequenceEqual(image), "CONFORMANCE_SNAPSHOT");
            var before = File.ReadAllBytes(newDatabasePath); bool rejected = false;
            try { new SqliteWorldStore(newDatabasePath, new string('0', 64)).Load(); }
            catch (InvalidDataException) { rejected = true; }
            Require(rejected && File.ReadAllBytes(newDatabasePath).SequenceEqual(before), "CONFORMANCE_COMPATIBILITY");
            checks.Add("sqlite-commit-reopen-snapshot-refusal:" + CommittedWorldCodec.Hash(image));
            using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = newDatabasePath, Mode = SqliteOpenMode.ReadOnly, Pooling = false }.ToString());
            connection.Open(); using var query = connection.CreateCommand(); query.CommandText = "SELECT sqlite_version()";
            return new ConformanceResult(checks, (string)query.ExecuteScalar()!);
        }
    }
}
