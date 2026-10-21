using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Application.CommandPipeline;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Foundation.Random;
using Game.Infrastructure.Persistence;

namespace Game.Infrastructure.Replay
{
    public sealed class ReplayManifest
    {
        public int ArtifactVersion { get; }
        public string CodecVersion { get; }
        public string RngVersion { get; }
        public string RuleVersion { get; }
        public string ContentHash { get; }
        public string? WorldSeed { get; }
        public ReplayManifest(string contentHash, string ruleVersion = "transfer-resource-v1+scheduler-v1", int artifactVersion = 1,
            string codecVersion = "mdc1", string rngVersion = "sha256-counter-v1", string? worldSeed = null)
        {
            if (contentHash == null || !Regex.IsMatch(contentHash, "\\A[0-9a-f]{64}\\z") || string.IsNullOrWhiteSpace(ruleVersion) ||
                string.IsNullOrWhiteSpace(codecVersion) || string.IsNullOrWhiteSpace(rngVersion) || artifactVersion < 1)
                throw new ArgumentException("INVALID_REPLAY_MANIFEST");
            WorldSeed=worldSeed; ContentHash=contentHash; RuleVersion=ruleVersion; ArtifactVersion=artifactVersion; CodecVersion=codecVersion; RngVersion=rngVersion;
        }
    }
    public sealed class ReplayAction
    {
        public string Kind { get; }
        public CommandEnvelope? Envelope { get; }
        public ExecutionPrincipal Principal { get; }
        public int Budget { get; }
        private ReplayAction(string kind, CommandEnvelope? envelope, ExecutionPrincipal principal, int budget)
        { Kind=kind; Envelope=envelope; Principal=principal ?? throw new ArgumentNullException(nameof(principal)); Budget=budget; }
        public static ReplayAction Execute(CommandEnvelope envelope, ExecutionPrincipal principal) => new ReplayAction("execute",envelope ?? throw new ArgumentNullException(nameof(envelope)),principal,0);
        public static ReplayAction Drain(ExecutionPrincipal principal, int budget=CommandProcessor.MaximumJobsPerInvocation)
        {
            if(budget<1||budget>CommandProcessor.MaximumJobsPerInvocation)throw new ArgumentOutOfRangeException(nameof(budget));
            return new ReplayAction("drain",null,principal,budget);
        }
    }
    public sealed class ReplayCheckpoint
    {
        public ulong Revision { get; }
        public string ResultHash { get; }
        public string GameplayHash { get; }
        public string IntegrityHash { get; }
        public string JournalHash { get; }
        public string JobsHash { get; }
        public ReplayCheckpoint(ulong revision,string resultHash,string gameplayHash,string integrityHash,string journalHash,string jobsHash)
        {
            foreach(var hash in new[]{resultHash,gameplayHash,integrityHash,journalHash,jobsHash})
                if(hash==null||!Regex.IsMatch(hash,"\\A[0-9a-f]{64}\\z"))throw new ArgumentException("INVALID_REPLAY_HASH");
            Revision=revision;ResultHash=resultHash;GameplayHash=gameplayHash;IntegrityHash=integrityHash;JournalHash=journalHash;JobsHash=jobsHash;
        }
    }
    public sealed class ReplayArtifact
    {
        private readonly byte[] initialImage;
        public ReplayManifest Manifest { get; }
        public byte[] InitialImage => (byte[])initialImage.Clone();
        public IReadOnlyList<ReplayAction> Actions { get; }
        public IReadOnlyList<ReplayCheckpoint> Checkpoints { get; }
        public ReplayArtifact(ReplayManifest manifest,byte[] initialImage,IEnumerable<ReplayAction> actions,IEnumerable<ReplayCheckpoint> checkpoints)
        {
            Manifest=manifest??throw new ArgumentNullException(nameof(manifest));
            this.initialImage=(byte[])(initialImage??throw new ArgumentNullException(nameof(initialImage))).Clone();
            var a=actions.ToArray();var c=checkpoints.ToArray();
            if(a.Length>10000||a.Length!=c.Length||a.Any(x=>x==null)||c.Any(x=>x==null))throw new ArgumentException("REPLAY_ACTION_COUNT");
            Actions=Array.AsReadOnly(a);Checkpoints=Array.AsReadOnly(c);
        }
    }
    public sealed class ReplayCompatibilityException : InvalidOperationException
    { public ReplayCompatibilityException():base("REPLAY_MANIFEST_INCOMPATIBLE"){} }
    public sealed class ReplayDivergence
    {
        public int Index { get; }
        public string Category { get; }
        public string ExpectedHash { get; }
        public string ActualHash { get; }
        public ulong ExpectedRevision { get; }
        public ulong ActualRevision { get; }
        internal ReplayDivergence(int index,string category,string expected,string actual,ulong before,ulong after)
        {Index=index;Category=category;ExpectedHash=expected;ActualHash=actual;ExpectedRevision=before;ActualRevision=after;}
    }
    public sealed class ReplayReport
    {
        public bool Success => Divergence==null;
        public ReplayDivergence? Divergence { get; }
        public CommittedWorld FinalState { get; }
        internal ReplayReport(CommittedWorld state,ReplayDivergence? divergence=null){FinalState=state;Divergence=divergence;}
    }
    /// <summary>Omniscient QA harness. Artifact principals are test inputs, never live authorization.</summary>
    public static class ReplayHarness
    {
        private static readonly HashSet<string> StableFailures=new HashSet<string>(StringComparer.Ordinal){
            "NO_ADVANCE_WORKFLOW","NOT_AUTHORIZED","REVISION_EXHAUSTED","JOB_IN_PAST","MISSING_PROPOSAL","MISSING_JOB_PROPOSAL",
            "JOB_FANOUT_LIMIT","CAUSAL_DEPTH_LIMIT","PROPOSAL_AUTHORITY","CAUSAL_CYCLE","TIME_RANGE","SEQUENCE_EXHAUSTED"};
        private static void CheckManifest(ReplayManifest actual,ReplayManifest expected)
        {
            if(actual.ArtifactVersion!=1||actual.CodecVersion!="mdc1"||actual.RngVersion!=RngContext.Algorithm||
                CanonicalCodec.Hash(ReplayArtifactCodec.ManifestObject(actual))!=CanonicalCodec.Hash(ReplayArtifactCodec.ManifestObject(expected)))
                throw new ReplayCompatibilityException();
        }
        public static ReplayArtifact Record(CommittedWorld initial,IEnumerable<ReplayAction> actions,ReplayManifest manifest,
            RuleRegistry rules,JobHandlerRegistry? handlers=null)
        {
            CheckManifest(manifest,manifest);
            var image=CommittedWorldCodec.Encode(initial);var clean=CommittedWorldCodec.Decode(image);
            var list=actions.Take(10001).ToArray();if(list.Length>10000)throw new ArgumentException("REPLAY_ACTION_COUNT");
            var processor=new CommandProcessor(clean,rules,jobHandlers:handlers);
            var checkpoints=list.Select(a=>Step(processor,a,manifest)).ToArray();
            return new ReplayArtifact(manifest,image,list,checkpoints);
        }
        public static ReplayReport Replay(ReplayArtifact artifact,ReplayManifest expected,RuleRegistry rules,JobHandlerRegistry? handlers=null)
        {
            CheckManifest(artifact.Manifest,expected);
            var processor=new CommandProcessor(CommittedWorldCodec.Decode(artifact.InitialImage),rules,jobHandlers:handlers);
            for(int i=0;i<artifact.Actions.Count;i++)
            {
                var actual=Step(processor,artifact.Actions[i],expected);var wanted=artifact.Checkpoints[i];
                var pairs=new[]{("result",wanted.ResultHash,actual.ResultHash),("journal",wanted.JournalHash,actual.JournalHash),
                    ("jobs",wanted.JobsHash,actual.JobsHash),("gameplay",wanted.GameplayHash,actual.GameplayHash),("integrity",wanted.IntegrityHash,actual.IntegrityHash)};
                foreach(var pair in pairs)if(pair.Item2!=pair.Item3)return new ReplayReport(processor.InspectCommitted(),new ReplayDivergence(i,pair.Item1,pair.Item2,pair.Item3,wanted.Revision,actual.Revision));
                if(wanted.Revision!=actual.Revision)return new ReplayReport(processor.InspectCommitted(),new ReplayDivergence(i,"revision",wanted.IntegrityHash,actual.IntegrityHash,wanted.Revision,actual.Revision));
            }
            return new ReplayReport(processor.InspectCommitted());
        }
        private static ReplayCheckpoint Step(CommandProcessor processor,ReplayAction action,ReplayManifest manifest)
        {
            object result;
            try
            {
                if(action.Kind=="execute")
                {
                    var r=processor.Execute(action.Envelope!,action.Principal);
                    result=ReplayArtifactCodec.Obj("kind","receipt","id",r.CommandId,"fingerprint",r.Fingerprint,"outcome",(int)r.Outcome,"error",r.ErrorCode,
                        "before",r.BeforeRevision,"after",r.AfterRevision,"tick",r.CommittedTick.Value,"events",r.OrderedEventIds.Select(x=>(object?)x.ToString()).ToArray());
                }
                else
                {
                    var w=processor.DrainAdvance(action.Principal,action.Budget);
                    result=ReplayArtifactCodec.Obj("kind","workflow","id",w.Id,"owner",w.OwnerId.ToString(),"source",(int)w.OwnerSource,"target",w.TargetTick.Value,
                        "status",(int)w.Status,"cursor",w.Cursor?.ToString(),"committed",w.CommittedJobs,"correlation",w.CorrelationId);
                }
            }
            catch(PipelineFailureException ex) when(StableFailures.Contains(ex.Code)) {result=ReplayArtifactCodec.Obj("kind","failure","code",ex.Code);}
            var state=processor.InspectCommitted();var full=ReplayArtifactCodec.Map(MdcReader.Decode(CommittedWorldCodec.Encode(state)));
            var gameplay=new Dictionary<string,object?>(full,StringComparer.Ordinal);gameplay.Remove("metadata");
            string Bound(object value)=>CanonicalCodec.Hash(ReplayArtifactCodec.Obj("manifest",ReplayArtifactCodec.ManifestObject(manifest),"value",value));
            return new ReplayCheckpoint(state.World.Revision.Value,Bound(result),Bound(gameplay),Bound(full),Bound(full["events"]!),
                Bound(ReplayArtifactCodec.Obj("jobs",full["jobs"],"next",full["job_next"],"exhausted",full["job_exhausted"],"workflows",full["workflows"],"executions",full["executions"])));
        }
    }
}
