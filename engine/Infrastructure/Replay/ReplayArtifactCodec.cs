using System;
using System.Collections.Generic;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Infrastructure.Persistence;

namespace Game.Infrastructure.Replay
{
    public static class ReplayArtifactCodec
    {
        internal static Dictionary<string,object?> Obj(params object?[] pairs)
        {var d=new Dictionary<string,object?>(StringComparer.Ordinal);for(int i=0;i<pairs.Length;i+=2)d.Add((string)pairs[i]!,pairs[i+1]);return d;}
        internal static Dictionary<string,object?> Map(object? value)=>value as Dictionary<string,object?>??throw new FormatException("REPLAY_OBJECT");
        private static string Str(object? value)=>value as string??throw new FormatException("REPLAY_STRING");
        private static string? NullableStr(object? value)=>value==null?null:Str(value);
        private static long Int(object? value)=>value is long n?n:throw new FormatException("REPLAY_INT64");
        private static ulong UInt(object? value)=>value is ulong n?n:value is long l&&l>=0?(ulong)l:throw new FormatException("REPLAY_UINT64");
        private static List<object?> Arr(object? value)=>value as List<object?>??throw new FormatException("REPLAY_ARRAY");
        internal static object ManifestObject(ReplayManifest m)=>Obj("artifact_version",m.ArtifactVersion,"codec_version",m.CodecVersion,"rng_version",m.RngVersion,"rule_version",m.RuleVersion,"content_hash",m.ContentHash,"world_seed",m.WorldSeed);
        private static object EnvelopeObject(CommandEnvelope e)=>Obj("id",e.CommandId,"type",e.CommandType,"issuer",e.IssuerId,"source",e.SourceType,"tick",e.WorldTick,"revision",e.ExpectedRevision,"version",e.PayloadVersion,"payload",e.Payload,"correlation",e.CorrelationId);
        private static object ActionObject(ReplayAction a)=>Obj("kind",a.Kind,"actor",a.Principal.ActorId.ToString(),"source",(int)a.Principal.Source,"budget",a.Budget,"envelope",a.Envelope==null?null:EnvelopeObject(a.Envelope));
        private static object CheckpointObject(ReplayCheckpoint c)=>Obj("revision",c.Revision,"result",c.ResultHash,"gameplay",c.GameplayHash,"integrity",c.IntegrityHash,"journal",c.JournalHash,"jobs",c.JobsHash);
        public static byte[] Encode(ReplayArtifact artifact)
        {
            var bytes=CanonicalCodec.Encode(Obj("manifest",ManifestObject(artifact.Manifest),"initial",MdcReader.Decode(artifact.InitialImage),
                "actions",artifact.Actions.Select(ActionObject).ToArray(),"checkpoints",artifact.Checkpoints.Select(CheckpointObject).ToArray()));
            if(bytes.Length>64*1024*1024)throw new FormatException("REPLAY_SIZE");return bytes;
        }
        public static ReplayArtifact Decode(byte[] bytes)
        {
            var root=Map(MdcReader.Decode(bytes));var m=Map(root["manifest"]);
            var manifest=new ReplayManifest(Str(m["content_hash"]),Str(m["rule_version"]),checked((int)Int(m["artifact_version"])),Str(m["codec_version"]),Str(m["rng_version"]),NullableStr(m["world_seed"]));
            if(manifest.ArtifactVersion!=1||manifest.CodecVersion!="mdc1"||manifest.RngVersion!=RngContext.Algorithm)throw new ReplayCompatibilityException();
            var actionRows=Arr(root["actions"]);var checkpointRows=Arr(root["checkpoints"]);
            if(actionRows.Count>10000||actionRows.Count!=checkpointRows.Count)throw new FormatException("REPLAY_ACTION_COUNT");
            var actions=actionRows.Select(value=>
            {
                var a=Map(value);var source=checked((int)Int(a["source"]));if(!Enum.IsDefined(typeof(CommandSource),source))throw new FormatException("REPLAY_SOURCE");
                var principal=new ExecutionPrincipal(EntityId<Actor>.Parse(Str(a["actor"])),(CommandSource)source);var kind=Str(a["kind"]);var budget=checked((int)Int(a["budget"]));
                if(kind=="drain") {if(a["envelope"]!=null)throw new FormatException("REPLAY_DRAIN_ENVELOPE");return ReplayAction.Drain(principal,budget);}
                if(kind!="execute"||budget!=0)throw new FormatException("REPLAY_ACTION_KIND");
                var e=Map(a["envelope"]);return ReplayAction.Execute(new CommandEnvelope(NullableStr(e["id"])!,NullableStr(e["type"])!,NullableStr(e["issuer"])!,NullableStr(e["source"])!,Int(e["tick"]),UInt(e["revision"]),checked((int)Int(e["version"])),Map(e["payload"]),NullableStr(e["correlation"])!),principal);
            }).ToArray();
            var checkpoints=checkpointRows.Select(value=>{var c=Map(value);return new ReplayCheckpoint(UInt(c["revision"]),Str(c["result"]),Str(c["gameplay"]),Str(c["integrity"]),Str(c["journal"]),Str(c["jobs"]));}).ToArray();
            var initial=CanonicalCodec.Encode(root["initial"]);CommittedWorldCodec.Decode(initial);
            var artifact=new ReplayArtifact(manifest,initial,actions,checkpoints);
            if(!Encode(artifact).SequenceEqual(bytes))throw new FormatException("REPLAY_UNKNOWN_FIELDS_OR_NONCANONICAL");return artifact;
        }
    }
}
