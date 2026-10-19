using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Game.Application.CommandPipeline;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Foundation.Time;
using Game.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class SqlitePersistenceTests
    {
        private string folder=null!;private string file=null!;
        private static readonly string Content=new string('a',64);
        private static EntityId<Actor> ActorId=>EntityId<Actor>.Parse("w0000000000000001/actor/0000000000000001");
        private static ExecutionPrincipal Principal=>new ExecutionPrincipal(ActorId,CommandSource.Player);
        private static RuleRegistry Rules()=>new RuleRegistry(new[]{new TransferResourceRule()});
        [SetUp]public void Setup(){folder=Path.Combine(Path.GetTempPath(),"mythical-store-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(folder);file=Path.Combine(folder,"world.db");}
        [TearDown]public void Cleanup(){Directory.Delete(folder,true);}
        private static CommittedWorld Initial(ulong revision=1)
        {
            var ids=IdentitySnapshot.Empty(1).BeginStage(revision);var actor=ids.Allocate<Actor>();var otherActor=ids.Allocate<Actor>();var location=ids.Allocate<Settlement>();var a=ids.Allocate<Lot>();var b=ids.Allocate<Lot>();
            var rng=new RngStreamState("test",new RngState(new RngContext(new string('0',64),"test","transfer","one",actor.Value,"main"),7));
            return new CommandProcessor(new WorldSnapshot(ids.PrepareSnapshot(),new WorldTick(0),new[]{new ActorState(actor,true,location),new ActorState(otherActor,true,location)},new[]{new ResourceLot(a,DefinitionId.Parse("commodity.timber@1"),actor.Value,location,10,0),new ResourceLot(b,DefinitionId.Parse("commodity.timber@1"),actor.Value,location,0,0)},new[]{rng}),Rules()).InspectCommitted();
        }
        private static Dictionary<string,object?> Transfer()=>new Dictionary<string,object?>{{"source_lot_id","w0000000000000001/lot/0000000000000001"},{"destination_lot_id","w0000000000000001/lot/0000000000000002"},{"quantity",3L}};
        private static CommandEnvelope Envelope(CommandProcessor p,string id="transfer",string type="TransferResource",Dictionary<string,object?>? payload=null)
        {var w=p.InspectCommitted().World;return new CommandEnvelope(id,type,ActorId.ToString(),"Player",w.Tick.Value,w.Revision.Value,1,payload??Transfer(),"persist.test");}
        private SqliteWorldStore Store(Action<string>? probe=null,int? pages=null)=>new SqliteWorldStore(file,Content,probe,maxPageCount:pages);
        private CommandProcessor Open(SqliteWorldStore store)=>new CommandProcessor(store.Load(),Rules(),store);
        private static string Hash(CommittedWorld state)=>CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(state));
        private void Sql(string text){using var c=new SqliteConnection("Data Source="+file+";Pooling=False");c.Open();using var cmd=c.CreateCommand();cmd.CommandText=text;cmd.ExecuteNonQuery();}
        [Test]public void FND_11_RoundtripRetainsRngUnsignedIdentityAndCanonicalBytes()
        {var state=Initial(ulong.MaxValue);var bytes=CommittedWorldCodec.Encode(state);var copy=CommittedWorldCodec.Decode(bytes);Assert.That(CommittedWorldCodec.Encode(copy),Is.EqualTo(bytes));Assert.That(copy.World.RngStates[0].State.DrawIndex,Is.EqualTo(7));Store().Initialize(state);Assert.That(Hash(Store().Load()),Is.EqualTo(Hash(state)));}
        [TestCase("trailing")][TestCase("truncate")][TestCase("unknown")][TestCase("noncanonical")][TestCase("depth")]
        public void FND_11_CodecRejectsMalformedInput(string flaw)
        {var bytes=CommittedWorldCodec.Encode(Initial());if(flaw=="trailing")bytes=bytes.Concat(new byte[]{0}).ToArray();if(flaw=="truncate")bytes=bytes.Take(bytes.Length-1).ToArray();if(flaw=="unknown")bytes=CanonicalCodec.Encode(new Dictionary<string,object?>{{"image_version",1L}});if(flaw=="noncanonical")bytes=Encoding.ASCII.GetBytes("MDC1i01;");if(flaw=="depth")bytes=Encoding.ASCII.GetBytes("MDC1"+string.Concat(Enumerable.Repeat("a1:",50))+"n");Assert.Catch(()=>CommittedWorldCodec.Decode(bytes));}
        [Test]public void FND_11_DurableRetryMismatchMetadataAndStaleSurviveReopen()
        {
            var store=Store();store.Initialize(Initial());var p=Open(store);var command=Envelope(p);p.Execute(command,Principal);var reject=Envelope(p,"reject","Unknown");p.Execute(reject,Principal);
            var reopened=Open(Store());var before=Hash(reopened.InspectCommitted());Assert.That(reopened.Execute(command,Principal).Outcome,Is.EqualTo(CommandOutcome.Accepted));Assert.That(reopened.Execute(reject,Principal).Outcome,Is.EqualTo(CommandOutcome.Rejected));Assert.That(Hash(reopened.InspectCommitted()),Is.EqualTo(before));
            var mismatch=new CommandEnvelope(command.CommandId,command.CommandType,command.IssuerId,command.SourceType,command.WorldTick,command.ExpectedRevision,1,Transfer(),"other");Assert.That(reopened.Execute(mismatch,Principal).ErrorCode,Is.EqualTo("CMD_PAYLOAD_MISMATCH"));
            Assert.That(reopened.InspectCommitted().World.Lots[0].OnHand,Is.EqualTo(7));Assert.That(reopened.InspectCommitted().World.RngStates[0].State.DrawIndex,Is.EqualTo(7));
        }
        [TestCase("before_begin")][TestCase("after_entities")][TestCase("after_allocator")][TestCase("after_jobs")][TestCase("after_rng_contexts")][TestCase("after_events")][TestCase("after_commands")][TestCase("after_world_meta")][TestCase("before_commit")]
        public void FND_11_RealTransactionFailureRollsBackEveryMemberAndRequiresReopen(string point)
        {
            Store().Initialize(Initial());var store=Store(s=>{if(s==point)throw new IOException("injected");});var p=Open(store);var before=p.InspectCommitted();var command=Envelope(p);
            Assert.Throws<IOException>(()=>p.Execute(command,Principal));Assert.That(p.InspectCommitted(),Is.SameAs(before));Assert.That(p.NeedsRecovery,Is.True);Assert.That(Hash(Store().Load()),Is.EqualTo(Hash(before)));Assert.Throws<PipelineFailureException>(()=>p.Execute(command,Principal));
            var retry=Open(Store());Assert.That(retry.Execute(command,Principal).Outcome,Is.EqualTo(CommandOutcome.Accepted));Assert.That(retry.InspectCommitted().World.Lots[0].OnHand,Is.EqualTo(7));
        }
        [Test]public void FND_11_AfterCommitUnknownOutcomeReopensOriginalReceiptWithoutDoubleEffect()
        {
            Store().Initialize(Initial());var p=Open(Store(s=>{if(s=="after_commit")throw new IOException("lost ack");}));var before=p.InspectCommitted();var command=Envelope(p);Assert.Throws<IOException>(()=>p.Execute(command,Principal));Assert.That(p.InspectCommitted(),Is.SameAs(before));
            var retry=Open(Store());var receipt=retry.Execute(command,Principal);Assert.That(receipt.AfterRevision,Is.EqualTo(2));Assert.That(retry.InspectCommitted().AcceptedReceipts.Count,Is.EqualTo(1));Assert.That(retry.InspectCommitted().World.Lots[0].OnHand,Is.EqualTo(7));
        }
        [Test]public void FND_11_FullImageCasProtectsMetadataAtSameWorldRevision()
        {Store().Initialize(Initial());var a=Open(Store());var b=Open(Store());a.Execute(Envelope(a,"first","Unknown"),Principal);Assert.Throws<InvalidOperationException>(()=>b.Execute(Envelope(b,"second","Unknown"),Principal));var loaded=Store().Load();Assert.That(loaded.World.Revision.Value,Is.EqualTo(1));Assert.That(loaded.MetadataReceipts.Single().CommandId,Is.EqualTo("first"));Assert.That(b.NeedsRecovery,Is.True);}
        [TestCase("schema")][TestCase("content")][TestCase("image")][TestCase("mirror")][TestCase("version")]
        public void FND_11_IncompatibleOrCorruptDatabaseIsRefusedWithoutOverwrite(string flaw)
        {
            Store().Initialize(Initial());if(flaw=="schema")Sql("UPDATE world_meta SET schema_version=2");if(flaw=="image")Sql("UPDATE world_meta SET image=x'00'");if(flaw=="mirror")Sql("DELETE FROM allocator");if(flaw=="version")Sql("UPDATE world_meta SET rng_version='other'");var before=File.ReadAllBytes(file);
            var store=flaw=="content"?new SqliteWorldStore(file,new string('b',64)):Store();Assert.Catch(()=>store.Load());Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));Assert.Throws<IOException>(()=>store.Initialize(Initial()));Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));
        }
        [Test]public void FND_11_BusyWriterFailureDoesNotPublish()
        {Store().Initialize(Initial());var p=Open(Store());var before=p.InspectCommitted();using(var c=new SqliteConnection("Data Source="+file+";Pooling=False")){c.Open();using var t=c.BeginTransaction();Assert.Throws<SqliteException>(()=>p.Execute(Envelope(p),Principal));Assert.That(p.InspectCommitted(),Is.SameAs(before));t.Rollback();}Assert.That(Hash(Store().Load()),Is.EqualTo(Hash(before)));}
        [Test]public void FND_11_SqliteFullFailureRetainsLastGoodCommit()
        {
            Store().Initialize(Initial());int pages;using(var c=new SqliteConnection("Data Source="+file+";Pooling=False")){c.Open();using var cmd=c.CreateCommand();cmd.CommandText="PRAGMA page_count";pages=Convert.ToInt32(cmd.ExecuteScalar());}
            var p=Open(Store(pages:pages));bool full=false;
            for(int i=0;i<100&&!full;i++){var before=p.InspectCommitted();try{p.Execute(Envelope(p,"metadata."+i,"Unknown"),Principal);}catch(SqliteException e){Assert.That(e.SqliteErrorCode,Is.EqualTo(13));Assert.That(p.InspectCommitted(),Is.SameAs(before));Assert.That(Hash(Store().Load()),Is.EqualTo(Hash(before)));full=true;}}
            Assert.That(full,Is.True,"Expected actual SQLITE_FULL under max_page_count");
        }
        [Test]public void FND_11_SchedulerPendingBudgetAndTerminalHistoryRoundtrip()
        {
            Store().Initialize(Initial());var p=Open(Store());for(int i=0;i<2;i++){var payload=Transfer();payload["due_tick"]=10L+i;payload["phase"]=3;payload["priority"]=100;p.Execute(Envelope(p,"job."+i,"ScheduleTransfer",payload),Principal);}
            var advance=Envelope(p,"advance","AdvanceTime",new Dictionary<string,object?>{{"target_tick",20L}});p.Execute(advance,Principal);var resumed=Open(Store());Assert.That(resumed.Execute(advance,Principal).Outcome,Is.EqualTo(CommandOutcome.AcceptedPending));Assert.That(resumed.DrainAdvance(Principal,1).Status,Is.EqualTo(AdvanceStatus.PausedBudget));
            var second=Open(Store());Assert.That(second.InspectCommitted().Scheduler.Workflow!.Cursor,Is.Not.Null);second.DrainAdvance(Principal);var final=Open(Store());Assert.That(final.InspectCommitted().Scheduler.Workflow!.Status,Is.EqualTo(AdvanceStatus.Completed));Assert.That(final.InspectCommitted().Scheduler.Workflow!.Cursor,Is.Null);Assert.That(final.InspectCommitted().World.Lots[0].OnHand,Is.EqualTo(4));Assert.That(final.InspectCommitted().Scheduler.Executions.Count,Is.EqualTo(2));
        }
        [Test]public void FND_11_NoOpAndStaleMetadataRemainDurableWithoutWorldMutation()
        {
            Store().Initialize(Initial());var p=Open(Store());var payload=Transfer();payload["destination_lot_id"]=payload["source_lot_id"];
            var noop=Envelope(p,"noop",payload:payload);Assert.That(p.Execute(noop,Principal).Outcome,Is.EqualTo(CommandOutcome.NoOp));
            var stale=new CommandEnvelope("stale","TransferResource",ActorId.ToString(),"Player",0,0,1,Transfer(),"persist.test");
            Assert.That(p.Execute(stale,Principal).ErrorCode,Is.EqualTo("STALE_REVISION"));var persisted=Hash(p.InspectCommitted());
            var reopened=Open(Store());Assert.That(reopened.Execute(noop,Principal).Outcome,Is.EqualTo(CommandOutcome.NoOp));Assert.That(reopened.Execute(stale,Principal).ErrorCode,Is.EqualTo("STALE_REVISION"));
            Assert.That(Hash(reopened.InspectCommitted()),Is.EqualTo(persisted));Assert.That(reopened.InspectCommitted().World.Revision.Value,Is.EqualTo(1));Assert.That(reopened.InspectCommitted().Journal.Events,Is.Empty);Assert.That(reopened.InspectCommitted().MetadataReceipts.Count,Is.EqualTo(2));
        }
        private sealed class DecisionHandler:IJobContinuationHandler
        {public string Key=>"persist-decision";public JobContinuation Evaluate(WorldSnapshot world,ScheduledJob job)=>new JobContinuation(requiresDecision:true);}
        private CommandProcessor PausedDecision()
        {
            Store().Initialize(Initial());var store=Store();var p=new CommandProcessor(store.Load(),Rules(),store,new JobHandlerRegistry(new[]{new DecisionHandler()}));
            var payload=Transfer();payload["due_tick"]=10L;payload["phase"]=3;payload["priority"]=100;payload["handler_key"]="persist-decision";
            p.Execute(Envelope(p,"job","ScheduleTransfer",payload),Principal);p.Execute(Envelope(p,"advance","AdvanceTime",new Dictionary<string,object?>{{"target_tick",20L}}),Principal);
            Assert.That(p.DrainAdvance(Principal).Status,Is.EqualTo(AdvanceStatus.PausedDecision));return p;
        }
        [Test]public void FND_11_DecisionPauseRestoresAndRequiresExplicitResolution()
        {
            var p=PausedDecision();var before=Hash(p.InspectCommitted());var store=Store();var reopened=new CommandProcessor(store.Load(),Rules(),store,new JobHandlerRegistry(new[]{new DecisionHandler()}));
            Assert.That(reopened.DrainAdvance(Principal).Status,Is.EqualTo(AdvanceStatus.PausedDecision));Assert.That(Hash(reopened.InspectCommitted()),Is.EqualTo(before));
            reopened.Execute(Envelope(reopened,"resolve","ResolveAdvanceDecision",new Dictionary<string,object?>{{"workflow_id","advance"}}),Principal);
            Assert.That(reopened.DrainAdvance(Principal).Status,Is.EqualTo(AdvanceStatus.Completed));var final=Store().Load();Assert.That(final.World.Tick.Value,Is.EqualTo(20));Assert.That(final.World.Lots[0].OnHand,Is.EqualTo(7));Assert.That(final.Scheduler.Executions.Count,Is.EqualTo(1));
        }
        private static Dictionary<string,object?> Image(CommittedWorld state)
        {
            var reader=typeof(CommittedWorldCodec).Assembly.GetType("Game.Infrastructure.Persistence.MdcReader",true)!;
            return (Dictionary<string,object?>)reader.GetMethod("Decode",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Static)!.Invoke(null,new object[]{CommittedWorldCodec.Encode(state)})!;
        }
        private static Dictionary<string,object?> FirstRow(Dictionary<string,object?> image,string key)=>(Dictionary<string,object?>)((IEnumerable<object?>)image[key]!).First()!;
        [TestCase("owner")][TestCase("source")][TestCase("target")][TestCase("correlation")][TestCase("execution_command")][TestCase("execution_error")][TestCase("execution_job_payload")][TestCase("execution_workflow_payload")]
        public void FND_11_CanonicalImageRejectsBrokenWorkflowAndExecutionLinks(string flaw)
        {
            var image=Image(PausedDecision().InspectCommitted());var workflow=FirstRow(image,"workflows");
            if(flaw=="owner")workflow["owner"]="w0000000000000001/actor/0000000000000002";
            if(flaw=="source")workflow["source"]=1L;
            if(flaw=="target")workflow["target"]=21L;
            if(flaw=="correlation")workflow["correlation"]="different";
            if(flaw=="execution_command")FirstRow(image,"executions")["command"]="advance";
            if(flaw=="execution_error")FirstRow(image,"executions")["error"]="fabricated";
            if(flaw=="execution_job_payload"||flaw=="execution_workflow_payload")
            {
                var command=(string)FirstRow(image,"executions")["command"]!;
                var receipt=((IEnumerable<object?>)image["accepted"]!).Cast<Dictionary<string,object?>>().Single(r=>(string)((Dictionary<string,object?>)r["envelope"]!)["id"]! == command);
                var envelope=(Dictionary<string,object?>)receipt["envelope"]!;var payload=(Dictionary<string,object?>)envelope["payload"]!;
                payload[flaw=="execution_job_payload"?"job_id":"workflow_id"]=flaw=="execution_job_payload"?"w0000000000000001/job/0000000000000002":"nonexistent-workflow";
                var rebuilt=new CommandEnvelope((string)envelope["id"]!,(string)envelope["type"]!,(string)envelope["issuer"]!,(string)envelope["source"]!,(long)envelope["tick"]!,Convert.ToUInt64(envelope["expected"]),Convert.ToInt32(envelope["version"]),payload,(string)envelope["correlation"]!);
                receipt["fingerprint"]=rebuilt.Fingerprint();
            }
            Assert.Catch(()=>CommittedWorldCodec.Decode(CanonicalCodec.Encode(image)),flaw);
        }
        [Test]public void FND_11_ValidFullImageWithUnknownFieldIsRejected()
        {var image=Image(Initial());image["unknown_future_field"]=true;Assert.Throws<FormatException>(()=>CommittedWorldCodec.Decode(CanonicalCodec.Encode(image)));}
        [TestCase("MDC1o2:s1:an s1:an")][TestCase("MDC1s2147483647:x")][TestCase("MDC1a2147483647:n")]
        public void FND_11_DecoderRejectsDuplicateKeysAndOversizedLengths(string text)
        {Assert.Throws<FormatException>(()=>CommittedWorldCodec.Decode(Encoding.ASCII.GetBytes(text.Replace(" ",""))));}
    }
}
