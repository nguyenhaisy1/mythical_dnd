using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Foundation.Random;
using Microsoft.Data.Sqlite;

namespace Game.Infrastructure.Persistence
{
    /// <summary>File-backed single-world kernel store with explicit schema 2 snapshot/recovery support.</summary>
    public sealed partial class SqliteWorldStore : ICommittedWorldStore
    {
        private readonly string path;
        private readonly string contentHash;
        private readonly Action<string>? probe;
        private readonly int timeout;
        private readonly int? maxPages;
        private const string RuleVersion="transfer-resource-v1+scheduler-v1";
        private static readonly string[] Tables={"entities","allocator","jobs","rng_contexts","events","commands"};
        public SqliteWorldStore(string path,string contentHash,Action<string>? probe=null,int timeoutSeconds=1,int? maxPageCount=null)
        {
            if(string.IsNullOrWhiteSpace(path)||path==":memory:"||contentHash==null||contentHash.Length!=64||contentHash.Any(c=>!"0123456789abcdef".Contains(c)))throw new ArgumentException("Invalid store path/content hash");
            this.path=Path.GetFullPath(path);this.contentHash=contentHash;this.probe=probe;timeout=timeoutSeconds;maxPages=maxPageCount;
            if(timeoutSeconds<1||maxPageCount<1)throw new ArgumentOutOfRangeException();
        }
        private SqliteConnection Connect(bool create=false)
        {
            var c=new SqliteConnection(new SqliteConnectionStringBuilder {DataSource=path,Mode=create?SqliteOpenMode.ReadWriteCreate:SqliteOpenMode.ReadWrite,Pooling=false,DefaultTimeout=timeout}.ToString());
            try {c.Open();Exec(c,null,"PRAGMA synchronous=FULL;");if(maxPages.HasValue)Exec(c,null,"PRAGMA max_page_count="+maxPages.Value.ToString(CultureInfo.InvariantCulture));return c;}
            catch{c.Dispose();throw;}
        }
        private static void Exec(SqliteConnection c,SqliteTransaction? t,string sql,params (string,object)[] values)
        {using var cmd=c.CreateCommand();cmd.Transaction=t;cmd.CommandText=sql;foreach(var v in values)cmd.Parameters.AddWithValue(v.Item1,v.Item2);cmd.ExecuteNonQuery();}
        private static object Scalar(SqliteConnection c,SqliteTransaction? t,string sql)
        {using var cmd=c.CreateCommand();cmd.Transaction=t;cmd.CommandText=sql;return cmd.ExecuteScalar()??throw new InvalidDataException("STORE_ROW_MISSING");}
        public void Initialize(CommittedWorld initial) => InitializeCore(initial,false,null);
        public void InitializeSave(CommittedWorld initial,string? worldSeed=null) => InitializeCore(initial,true,worldSeed);
        private void InitializeCore(CommittedWorld initial,bool save,string? worldSeed,long acceptedCount=0,Action? beforeInitializeCommit=null)
        {
            // Explicit create only. Never initialize an existing file, even if empty or incompatible.
            using(var file=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.None)){}
            using var c=Connect(true);
            Exec(c,null,"PRAGMA journal_mode=DELETE;");
            using var t=c.BeginTransaction();
            Exec(c,t,@"CREATE TABLE world_meta(singleton INTEGER PRIMARY KEY CHECK(singleton=1),world_id TEXT NOT NULL,revision TEXT NOT NULL CHECK(length(revision)=16),tick INTEGER NOT NULL CHECK(tick>=0),schema_version INTEGER NOT NULL,codec_version TEXT NOT NULL,rule_version TEXT NOT NULL,content_hash TEXT NOT NULL,rng_version TEXT NOT NULL,image BLOB NOT NULL,sha256 TEXT NOT NULL);
CREATE TABLE entities(id TEXT PRIMARY KEY NOT NULL,payload BLOB NOT NULL);
CREATE TABLE allocator(id TEXT PRIMARY KEY NOT NULL,payload BLOB NOT NULL);
CREATE TABLE jobs(id TEXT PRIMARY KEY NOT NULL,payload BLOB NOT NULL);
CREATE TABLE rng_contexts(id TEXT PRIMARY KEY NOT NULL,payload BLOB NOT NULL);
CREATE TABLE events(id TEXT PRIMARY KEY NOT NULL,payload BLOB NOT NULL,revision TEXT NOT NULL CHECK(length(revision)=16),ordinal INTEGER NOT NULL CHECK(ordinal>=0),UNIQUE(revision,ordinal));
CREATE TABLE commands(id TEXT PRIMARY KEY NOT NULL,payload BLOB NOT NULL,fingerprint TEXT NOT NULL,metadata INTEGER NOT NULL CHECK(metadata IN(0,1)));
CREATE TABLE snapshots(revision TEXT PRIMARY KEY NOT NULL,payload BLOB NOT NULL,sha256 TEXT NOT NULL);");
            var bytes=CommittedWorldCodec.Encode(initial);_ = CommittedWorldCodec.Decode(bytes);
            Write(c,t,initial,bytes,false);if(save){Exec(c,t,"UPDATE world_meta SET schema_version=2");InitializeRecovery(c,t,initial,bytes,worldSeed,acceptedCount);}beforeInitializeCommit?.Invoke();t.Commit();
        }
        public CommittedWorld Load()
        {
            using var c=Connect();using var t=c.BeginTransaction(deferred:true);
            var bytes=Read(c,t);var state=CommittedWorldCodec.Decode(bytes);VerifyMirrors(c,t,state,bytes);t.Commit();return state;
        }
        private byte[] Read(SqliteConnection c,SqliteTransaction t)
        {
            using var cmd=c.CreateCommand();cmd.Transaction=t;cmd.CommandText="SELECT world_id,revision,tick,schema_version,codec_version,rule_version,content_hash,rng_version,image,sha256 FROM world_meta WHERE singleton=1";
            using var r=cmd.ExecuteReader();if(!r.Read())throw new InvalidDataException("STORE_ROW_MISSING");
            if(r.GetInt64(3)!=1&&r.GetInt64(3)!=2)throw new InvalidDataException("UNSUPPORTED_SCHEMA");
            if(r.GetString(4)!="mdc1"||r.GetString(5)!=RuleVersion||r.GetString(6)!=contentHash||r.GetString(7)!="sha256-counter-v1")throw new InvalidDataException("INCOMPATIBLE_SAVE");
            var bytes=(byte[])r[8];if(CommittedWorldCodec.Hash(bytes)!=r.GetString(9))throw new InvalidDataException("CORRUPT_IMAGE");
            var state=CommittedWorldCodec.Decode(bytes);
            if(r.GetString(0)!=state.World.WorldId.ToString("x16",CultureInfo.InvariantCulture)||r.GetString(1)!=state.World.Revision.Value.ToString("x16",CultureInfo.InvariantCulture)||r.GetInt64(2)!=state.World.Tick.Value)throw new InvalidDataException("CORRUPT_META");
            var schema=r.GetInt64(3);if(r.Read())throw new InvalidDataException("DUPLICATE_WORLD");r.Close();
            if(schema==1&&Convert.ToInt64(Scalar(c,t,"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name IN ('save_manifest','save_points','recovery_images')"),CultureInfo.InvariantCulture)>0)throw new InvalidDataException("SCHEMA_LAYOUT_MISMATCH");
            if(schema==2){var manifest=ReadManifest(c,t);if(manifest.Sha!=CommittedWorldCodec.Hash(bytes)||manifest.World!=state.World.WorldId.ToString("x16",CultureInfo.InvariantCulture))throw new InvalidDataException("SAVE_HEAD_CONFLICT");}
            return bytes;
        }
        public void Commit(CommittedWorld expected,CommittedWorld candidate)
        {
            var before=CommittedWorldCodec.Encode(expected);var after=CommittedWorldCodec.Encode(candidate);
            _=CommittedWorldCodec.Decode(after);
            if(expected.World.WorldId!=candidate.World.WorldId||candidate.World.Revision.Value<expected.World.Revision.Value||candidate.World.Tick<expected.World.Tick)throw new InvalidDataException("INVALID_COMMIT_WORLD");
            probe?.Invoke("before_begin");using var c=Connect();using var t=c.BeginTransaction();
            var actual=Read(c,t);
            if(!actual.SequenceEqual(before))throw new InvalidOperationException("STORE_CONFLICT");
            VerifyMirrors(c,t,expected,actual);
            Write(c,t,candidate,after,true);AppendRecovery(c,t,expected,candidate,after);probe?.Invoke("before_commit");t.Commit();probe?.Invoke("after_commit");
        }
        private sealed class Row
        {
            public string Id {get;} public byte[] Payload {get;} public string Revision {get;} public long Ordinal {get;} public string Fingerprint {get;} public long Metadata {get;}
            public Row(string id,object? payload,string revision="",long ordinal=0,string fingerprint="",long metadata=0)
            {Id=id;Payload=CanonicalCodec.Encode(payload);Revision=revision;Ordinal=ordinal;Fingerprint=fingerprint;Metadata=metadata;}
        }
        private static Dictionary<string,List<Row>> Rows(CommittedWorld state,byte[] bytes)
        {
            var d=(Dictionary<string,object?>)MdcReader.Decode(bytes)!;
            Dictionary<string,object?> Map(object? x)=>(Dictionary<string,object?>)x!;
            IEnumerable<Dictionary<string,object?>> List(string key)=>((IEnumerable<object?>)d[key]!).Select(Map);
            string S(Dictionary<string,object?> x,string key)=>(string)x[key]!;
            var actors=List("actors").ToDictionary(x=>S(x,"id"));var lots=List("lots").ToDictionary(x=>S(x,"id"));
            var result=Tables.ToDictionary(x=>x,x=>new List<Row>(),StringComparer.Ordinal);
            foreach(var e in List("entities")){var id=S(e,"id");result["entities"].Add(new Row(id,new Dictionary<string,object?>{{"identity",e},{"actor",actors.TryGetValue(id,out var a)?a:null},{"lot",lots.TryGetValue(id,out var l)?l:null}}));}
            foreach(var c in List("cursors"))result["allocator"].Add(new Row(S(c,"kind"),c));
            foreach(var j in List("jobs"))result["jobs"].Add(new Row(S(j,"id"),j));
            foreach(var r in List("rng"))result["rng_contexts"].Add(new Row(S(r,"key"),r));
            var eventRows=List("events").ToArray();for(int i=0;i<eventRows.Length;i++){var e=state.Journal.Events[i];result["events"].Add(new Row(e.Id.ToString(),eventRows[i],e.CommitSequence.ToString("x16",CultureInfo.InvariantCulture),e.EventIndex));}
            foreach(var key in new[]{"accepted","metadata"})foreach(var r in List(key))result["commands"].Add(new Row(S(Map(r["envelope"]),"id"),r,fingerprint:S(r,"fingerprint"),metadata:key=="metadata"?1:0));
            return result;
        }
        private void Write(SqliteConnection c,SqliteTransaction t,CommittedWorld state,byte[] bytes,bool fault)
        {
            var rows=Rows(state,bytes);
            foreach(var table in Tables)
            {
                Exec(c,t,"DELETE FROM "+table);
                foreach(var row in rows[table])
                {
                    if(table=="events")Exec(c,t,"INSERT INTO events VALUES($id,$payload,$revision,$ordinal)",("$id",row.Id),("$payload",row.Payload),("$revision",row.Revision),("$ordinal",row.Ordinal));
                    else if(table=="commands")Exec(c,t,"INSERT INTO commands VALUES($id,$payload,$fingerprint,$metadata)",("$id",row.Id),("$payload",row.Payload),("$fingerprint",row.Fingerprint),("$metadata",row.Metadata));
                    else Exec(c,t,"INSERT INTO "+table+" VALUES($id,$payload)",("$id",row.Id),("$payload",row.Payload));
                }
                if(fault)probe?.Invoke("after_"+table);
            }
            var schemaVersion=Convert.ToInt64(Scalar(c,t,"SELECT COALESCE(MAX(schema_version),1) FROM world_meta"),CultureInfo.InvariantCulture);
            Exec(c,t,"DELETE FROM world_meta");
            Exec(c,t,"INSERT INTO world_meta VALUES(1,$world,$revision,$tick,$schema,'mdc1',$rule,$content,'sha256-counter-v1',$image,$sha)",("$schema",schemaVersion),("$world",state.World.WorldId.ToString("x16",CultureInfo.InvariantCulture)),("$revision",state.World.Revision.Value.ToString("x16",CultureInfo.InvariantCulture)),("$tick",state.World.Tick.Value),("$rule",RuleVersion),("$content",contentHash),("$image",bytes),("$sha",CommittedWorldCodec.Hash(bytes)));
            if(fault)probe?.Invoke("after_world_meta");
        }
        private static void VerifyMirrors(SqliteConnection c,SqliteTransaction t,CommittedWorld state,byte[] bytes)
        {
            foreach(var pair in Rows(state,bytes))
            {
                var expected=pair.Value.ToDictionary(x=>x.Id,StringComparer.Ordinal);
                using var cmd=c.CreateCommand();cmd.Transaction=t;cmd.CommandText="SELECT * FROM "+pair.Key;using var r=cmd.ExecuteReader();
                int count=0;while(r.Read())
                {
                    if(!expected.TryGetValue(r.GetString(0),out var row)||!row.Payload.SequenceEqual((byte[])r[1])||
                       (pair.Key=="events"&&(r.GetString(2)!=row.Revision||r.GetInt64(3)!=row.Ordinal))||
                       (pair.Key=="commands"&&(r.GetString(2)!=row.Fingerprint||r.GetInt64(3)!=row.Metadata)))throw new InvalidDataException("CORRUPT_RELATIONAL_STATE");count++;
                }
                if(count!=expected.Count)throw new InvalidDataException("MISSING_RELATIONAL_STATE");
            }
        }
    }
}
