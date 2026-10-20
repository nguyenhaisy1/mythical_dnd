using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Game.Application.CommandPipeline;
using Microsoft.Data.Sqlite;

namespace Game.Infrastructure.Persistence
{
    public sealed class SaveInfo
    {
        public int SchemaVersion => 2;
        public long Sequence { get; }
        public long AcceptedMutationCount { get; }
        public long AcceptedSinceSnapshot => AcceptedMutationCount % 100;
        public long SnapshotCount { get; }
        public string? WorldSeed { get; }
        internal SaveInfo(long sequence, long count, long snapshots, string? seed)
        { Sequence = sequence; AcceptedMutationCount = count; SnapshotCount = snapshots; WorldSeed = seed; }
    }

    public sealed partial class SqliteWorldStore
    {
        private static bool HasRecovery(SqliteConnection c, SqliteTransaction t) =>
            Convert.ToInt64(Scalar(c, t, "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='save_manifest'"), CultureInfo.InvariantCulture) == 1;

        private void InitializeRecovery(SqliteConnection c, SqliteTransaction t, CommittedWorld state, byte[] bytes, string? worldSeed, long acceptedCount)
        {
            Exec(c, t, @"CREATE TABLE save_manifest(singleton INTEGER PRIMARY KEY CHECK(singleton=1),format_version INTEGER NOT NULL,head_sequence INTEGER NOT NULL,head_sha TEXT NOT NULL,accepted_mutation_count INTEGER NOT NULL,world_id TEXT NOT NULL,codec_version TEXT NOT NULL,rule_version TEXT NOT NULL,content_hash TEXT NOT NULL,rng_version TEXT NOT NULL,world_seed TEXT,manifest_sha TEXT NOT NULL);
CREATE TABLE recovery_images(sequence INTEGER PRIMARY KEY,image BLOB NOT NULL,sha256 TEXT NOT NULL,previous_sha TEXT NOT NULL);
CREATE TABLE save_points(sequence INTEGER PRIMARY KEY,image BLOB NOT NULL,sha256 TEXT NOT NULL,kind TEXT NOT NULL);");
            Exec(c, t, "INSERT INTO save_manifest VALUES(1,1,0,$sha,$count,$world,'mdc1',$rule,$content,'sha256-counter-v1',$seed,'')",
                ("$count", acceptedCount), ("$sha", CommittedWorldCodec.Hash(bytes)), ("$world", state.World.WorldId.ToString("x16", CultureInfo.InvariantCulture)),
                ("$rule", RuleVersion), ("$content", contentHash), ("$seed", (object?)worldSeed ?? DBNull.Value));
            SignManifest(c, t); PutSnapshot(c, t, 0, bytes, "initial");
        }
        private sealed class Manifest
        {
            public long Sequence; public long Count; public string Sha = ""; public string World = ""; public string? Seed;
        }
        private Manifest ReadManifest(SqliteConnection c, SqliteTransaction t)
        {
            if (!HasRecovery(c, t)) throw new InvalidDataException("SAVE_UPGRADE_REQUIRED");
            if (Convert.ToInt64(Scalar(c, t, "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name IN ('recovery_images','save_points')"), CultureInfo.InvariantCulture) != 2) throw new InvalidDataException("SAVE_TABLES_MISSING");
            using var cmd = c.CreateCommand(); cmd.Transaction = t; cmd.CommandText = "SELECT * FROM save_manifest WHERE singleton=1";
            using var r = cmd.ExecuteReader(); if (!r.Read()) throw new InvalidDataException("SAVE_MANIFEST_MISSING");
            if (r.GetInt64(1) != 1 || r.GetString(6) != "mdc1" || r.GetString(7) != RuleVersion || r.GetString(8) != contentHash || r.GetString(9) != "sha256-counter-v1") throw new InvalidDataException("INCOMPATIBLE_SAVE_MANIFEST");
            var m = new Manifest { Sequence = r.GetInt64(2), Sha = r.GetString(3), Count = r.GetInt64(4), World = r.GetString(5), Seed = r.IsDBNull(10) ? null : r.GetString(10) };
            if (m.Sequence < 0 || m.Count < 0 || m.Sha.Length != 64) throw new InvalidDataException("CORRUPT_SAVE_MANIFEST");
            if (r.GetString(11) != ManifestHash(m)) throw new InvalidDataException("CORRUPT_MANIFEST_HASH");
            return m;
        }
        private string ManifestHash(Manifest m) => CommittedWorldCodec.Hash(Game.Foundation.Random.CanonicalCodec.Encode(
            new Dictionary<string, object?> { { "version", 1L }, { "sequence", m.Sequence }, { "count", m.Count }, { "sha", m.Sha }, { "world", m.World }, { "seed", m.Seed }, { "codec", "mdc1" }, { "rule", RuleVersion }, { "content", contentHash }, { "rng", "sha256-counter-v1" } }));
        private void SignManifest(SqliteConnection c, SqliteTransaction t)
        {
            Manifest m;
            using (var cmd = c.CreateCommand())
            {
                cmd.Transaction = t; cmd.CommandText = "SELECT head_sequence,head_sha,accepted_mutation_count,world_id,world_seed FROM save_manifest WHERE singleton=1";
                using var r = cmd.ExecuteReader(); if (!r.Read()) throw new InvalidDataException("SAVE_MANIFEST_MISSING");
                m = new Manifest { Sequence = r.GetInt64(0), Sha = r.GetString(1), Count = r.GetInt64(2), World = r.GetString(3), Seed = r.IsDBNull(4) ? null : r.GetString(4) };
            }
            Exec(c, t, "UPDATE save_manifest SET manifest_sha=$hash WHERE singleton=1", ("$hash", ManifestHash(m)));
        }
        private static void PutSnapshot(SqliteConnection c, SqliteTransaction t, long sequence, byte[] bytes, string kind)
        {
            Exec(c, t, "INSERT OR REPLACE INTO save_points VALUES($seq,$image,$sha,$kind)", ("$seq", sequence), ("$image", bytes), ("$sha", CommittedWorldCodec.Hash(bytes)), ("$kind", kind));
        }
        private void AppendRecovery(SqliteConnection c, SqliteTransaction t, CommittedWorld expected, CommittedWorld candidate, byte[] bytes)
        {
            if (!HasRecovery(c, t)) return;
            var m = ReadManifest(c, t);
            if (m.Sha != CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(expected))) throw new InvalidDataException("SAVE_HEAD_CONFLICT");
            var sequence = checked(m.Sequence + 1);
            Exec(c, t, "INSERT INTO recovery_images VALUES($seq,$image,$sha,$previous)", ("$seq", sequence), ("$image", bytes), ("$sha", CommittedWorldCodec.Hash(bytes)), ("$previous", m.Sha));
            probe?.Invoke("after_recovery_frame");
            var mutations = candidate.AcceptedReceipts.Count - expected.AcceptedReceipts.Count;
            if (mutations < 0 || mutations > 1) throw new InvalidDataException("INVALID_PUBLICATION_RECEIPTS");
            var count = checked(m.Count + mutations);
            if (count > m.Count && count % 100 == 0) { PutSnapshot(c, t, sequence, bytes, "periodic"); probe?.Invoke("after_snapshot"); }
            Exec(c, t, "UPDATE save_manifest SET head_sequence=$seq,head_sha=$sha,accepted_mutation_count=$count WHERE singleton=1", ("$seq", sequence), ("$sha", CommittedWorldCodec.Hash(bytes)), ("$count", count));
            SignManifest(c, t); probe?.Invoke("after_save_manifest");
        }
        public SaveInfo GetSaveInfo()
        {
            using var c = ConnectReadOnly(); using var t = c.BeginTransaction(deferred: true);
            if(Convert.ToInt64(Scalar(c,t,"SELECT schema_version FROM world_meta WHERE singleton=1"),CultureInfo.InvariantCulture)!=2)throw new InvalidDataException("SAVE_UPGRADE_REQUIRED");
            var m = ReadManifest(c, t);
            return new SaveInfo(m.Sequence, m.Count, Convert.ToInt64(Scalar(c, t, "SELECT COUNT(*) FROM save_points"), CultureInfo.InvariantCulture), m.Seed);
        }
        public void CreateSnapshot(CommittedWorld expected)
        {
            using var c = Connect(); using var t = c.BeginTransaction(); var bytes = Read(c, t);
            if (!bytes.SequenceEqual(CommittedWorldCodec.Encode(expected))) throw new InvalidOperationException("STORE_CONFLICT");
            VerifyMirrors(c, t, expected, bytes); var m = ReadManifest(c, t);
            if (m.Sha != CommittedWorldCodec.Hash(bytes)) throw new InvalidDataException("SAVE_HEAD_CONFLICT");
            PutSnapshot(c, t, m.Sequence, bytes, "manual"); probe?.Invoke("after_snapshot");
            probe?.Invoke("before_commit"); t.Commit(); probe?.Invoke("after_commit");
        }
        private SqliteConnection ConnectReadOnly()
        {
            var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = path, Mode = SqliteOpenMode.ReadOnly, Pooling = false, DefaultTimeout = timeout }.ToString());
            try { connection.Open(); return connection; } catch { connection.Dispose(); throw; }
        }
        public CommittedWorld RecoverToNewFile(string destination)
        {
            using var c = ConnectReadOnly(); using var t = c.BeginTransaction(deferred: true);
            // Compatibility is checked before considering damaged snapshots/current image.
            using (var cmd = c.CreateCommand())
            {
                cmd.Transaction = t; cmd.CommandText = "SELECT schema_version,codec_version,rule_version,content_hash,rng_version FROM world_meta WHERE singleton=1";
                using var r = cmd.ExecuteReader(); if (!r.Read() || r.GetInt64(0) != 2 || r.GetString(1) != "mdc1" || r.GetString(2) != RuleVersion || r.GetString(3) != contentHash || r.GetString(4) != "sha256-counter-v1") throw new InvalidDataException("INCOMPATIBLE_SAVE");
            }
            var m = ReadManifest(c, t); var points = new List<(long, byte[], string)>();
            using (var cmd = c.CreateCommand())
            { cmd.Transaction = t; cmd.CommandText = "SELECT sequence,image,sha256 FROM save_points ORDER BY sequence DESC"; using var r = cmd.ExecuteReader(); while (r.Read()) points.Add((r.GetInt64(0), (byte[])r[1], r.GetString(2))); }
            byte[]? recovered = null;
            foreach (var point in points)
            {
                if (point.Item1 < 0 || point.Item1 > m.Sequence || CommittedWorldCodec.Hash(point.Item2) != point.Item3) continue;
                try
                {
                    ValidateRecoveryImage(point.Item2, m, null); var bytes = point.Item2; var sha = point.Item3; var seq = point.Item1;
                    using var cmd = c.CreateCommand(); cmd.Transaction = t; cmd.CommandText = "SELECT sequence,image,sha256,previous_sha FROM recovery_images WHERE sequence>$seq ORDER BY sequence"; cmd.Parameters.AddWithValue("$seq", seq);
                    using var r = cmd.ExecuteReader(); while (r.Read())
                    {
                        if (r.GetInt64(0) != checked(seq + 1) || r.GetInt64(0) > m.Sequence || r.GetString(3) != sha) throw new InvalidDataException("RECOVERY_CHAIN_BROKEN");
                        var next = (byte[])r[1]; if (CommittedWorldCodec.Hash(next) != r.GetString(2)) throw new InvalidDataException("RECOVERY_FRAME_CORRUPT");
                        ValidateRecoveryImage(next, m, bytes); bytes = next; sha = r.GetString(2); seq++;
                    }
                    if (seq != m.Sequence || sha != m.Sha) throw new InvalidDataException("RECOVERY_HEAD_MISSING");
                    recovered = bytes; break;
                }
                catch (Exception error) when (error is InvalidDataException || error is FormatException || error is ArgumentException || error is KeyNotFoundException || error is InvalidOperationException || error is OverflowException) { }
            }
            if (recovered == null) throw new InvalidDataException("NO_COMPLETE_RECOVERY_PATH");
            var state = CommittedWorldCodec.Decode(recovered);
            // Recovery never invokes gameplay handlers. Output is explicit, create-new only.
            var output = new SqliteWorldStore(destination, contentHash); output.InitializeCore(state, true, m.Seed, m.Count);
            _ = output.Load(); return state;
        }
        private static void ValidateRecoveryImage(byte[] bytes, Manifest manifest, byte[]? prior)
        {
            var state = CommittedWorldCodec.Decode(bytes);
            if (state.World.WorldId.ToString("x16", CultureInfo.InvariantCulture) != manifest.World) throw new InvalidDataException("RECOVERY_WORLD_MISMATCH");
            if (prior != null)
            {
                var old = CommittedWorldCodec.Decode(prior);
                if (state.World.Revision.Value < old.World.Revision.Value || state.World.Tick < old.World.Tick) throw new InvalidDataException("RECOVERY_ORDER_INVALID");
            }
        }
        public CommittedWorld MigrateToNewFile(string destination, string backupPath, string? worldSeed = null)
        {
            using var c = ConnectReadOnly(); using var t = c.BeginTransaction(deferred: true);
            var bytes = Read(c, t); var state = CommittedWorldCodec.Decode(bytes); VerifyMirrors(c, t, state, bytes);
            if (HasRecovery(c, t)) throw new InvalidDataException("MIGRATION_NOT_REGISTERED");
            // Online backup captures one SQLite-consistent image. Neither original nor backup is overwritten.
            using (var f = new FileStream(backupPath, FileMode.CreateNew, FileAccess.Write, FileShare.None)) { }
            using (var backup = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = Path.GetFullPath(backupPath), Mode = SqliteOpenMode.ReadWrite, Pooling = false }.ToString()))
            { backup.Open(); c.BackupDatabase(backup); }
            var backupStore = new SqliteWorldStore(backupPath, contentHash); var saved = backupStore.Load();
            if (!CommittedWorldCodec.Encode(saved).SequenceEqual(bytes)) throw new InvalidDataException("BACKUP_VALIDATION_FAILED");
            var output = new SqliteWorldStore(destination, contentHash); probe?.Invoke("migration_after_backup"); output.InitializeCore(state, true, worldSeed, beforeInitializeCommit: () => probe?.Invoke("migration_before_commit")); probe?.Invoke("migration_after_copy"); _ = output.Load(); return state;
        }
    }
}
