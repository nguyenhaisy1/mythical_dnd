using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public sealed class SaveRecoveryTests
    {
        private string folder=null!;
        private string file=>Path.Combine(folder,"world.db");
        private static readonly string Content=new string('a',64);
        private static EntityId<Actor> ActorId=>EntityId<Actor>.Parse("w0000000000000001/actor/0000000000000001");
        private static ExecutionPrincipal Principal=>new ExecutionPrincipal(ActorId,CommandSource.Player);
        private static RuleRegistry Rules()=>new RuleRegistry(new[]{new TransferResourceRule()});
        [SetUp] public void Setup(){folder=Path.Combine(Path.GetTempPath(),"mythical-save-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(folder);}
        [TearDown] public void Cleanup(){Directory.Delete(folder,true);}
        private static CommittedWorld Initial()
        {
            var ids=IdentitySnapshot.Empty(1).BeginStage(1);var actor=ids.Allocate<Actor>();var location=ids.Allocate<Settlement>();var a=ids.Allocate<Lot>();var b=ids.Allocate<Lot>();
            return new CommandProcessor(new WorldSnapshot(ids.PrepareSnapshot(),new WorldTick(0),new[]{new ActorState(actor,true,location)},new[]{new ResourceLot(a,DefinitionId.Parse("commodity.timber@1"),actor.Value,location,10,0),new ResourceLot(b,DefinitionId.Parse("commodity.timber@1"),actor.Value,location,0,0)},new[]{new RngStreamState("save-test",new RngState(new RngContext(new string('0',64),"test","transfer","one",actor.Value,"main"),7))}),Rules()).InspectCommitted();
        }
        private SqliteWorldStore Store(Action<string>? probe=null,string? path=null)=>new SqliteWorldStore(path??file,Content,probe);
        private CommandProcessor Open(SqliteWorldStore? store=null){store??=Store();return new CommandProcessor(store.Load(),Rules(),store);}
        private static byte[] Bytes(CommittedWorld state)=>CommittedWorldCodec.Encode(state);
        private static Dictionary<string,object?> Transfer(bool reverse=false)=>new Dictionary<string,object?>{{"source_lot_id","w0000000000000001/lot/000000000000000"+(reverse?"2":"1")},{"destination_lot_id","w0000000000000001/lot/000000000000000"+(reverse?"1":"2")},{"quantity",1L}};
        private static CommandEnvelope Envelope(CommandProcessor p,string id,string type="TransferResource",Dictionary<string,object?>? payload=null)
        {var w=p.InspectCommitted().World;return new CommandEnvelope(id,type,ActorId.ToString(),"Player",w.Tick.Value,w.Revision.Value,1,payload??Transfer(),"save.test");}
        private static void Mutate(CommandProcessor p,int count,int start=0){for(int i=start;i<start+count;i++)Assert.That(p.Execute(Envelope(p,"transfer."+i,payload:Transfer(i%2==1)),Principal).Outcome,Is.EqualTo(CommandOutcome.Accepted));}
        private void Sql(string sql){using var c=new SqliteConnection("Data Source="+file+";Pooling=False");c.Open();using var command=c.CreateCommand();command.CommandText=sql;command.ExecuteNonQuery();}
        [Test] public void FND_12_InitialSnapshotCarriesSeedAndManualSnapshotPreservesWorld()
        {
            var initial=Initial();Store().InitializeSave(initial,"research-seed");var info=Store().GetSaveInfo();Assert.That(info.SchemaVersion,Is.EqualTo(2));Assert.That(info.Sequence,Is.EqualTo(0));Assert.That(info.AcceptedMutationCount,Is.EqualTo(0));Assert.That(info.SnapshotCount,Is.EqualTo(1));Assert.That(info.WorldSeed,Is.EqualTo("research-seed"));
            var p=Open();Mutate(p,1);var expected=Bytes(p.InspectCommitted());Store().CreateSnapshot(p.InspectCommitted());Assert.That(Bytes(Store().Load()),Is.EqualTo(expected));Assert.That(Store().GetSaveInfo().SnapshotCount,Is.EqualTo(2));
        }
        [Test] public void FND_12_AutomaticSnapshotAt100CountsMutationsNotManualOrMetadata()
        {
            Store().InitializeSave(Initial());var p=Open();Mutate(p,99);Assert.That(Store().GetSaveInfo().SnapshotCount,Is.EqualTo(1));
            Store().CreateSnapshot(p.InspectCommitted());Assert.That(Store().GetSaveInfo().SnapshotCount,Is.EqualTo(2));
            var rejected=Envelope(p,"reject","Unknown");p.Execute(rejected,Principal);p.Execute(rejected,Principal);
            var payload=Transfer();payload["destination_lot_id"]=payload["source_lot_id"];var noop=Envelope(p,"noop",payload:payload);Assert.That(p.Execute(noop,Principal).Outcome,Is.EqualTo(CommandOutcome.NoOp));p.Execute(noop,Principal);
            Assert.That(Store().GetSaveInfo().AcceptedMutationCount,Is.EqualTo(99));Assert.That(Store().GetSaveInfo().SnapshotCount,Is.EqualTo(2));
            var before100=Bytes(p.InspectCommitted());
            var faulted=Open(Store(step=>{if(step=="after_snapshot")throw new IOException("periodic fault");}));
            Assert.Throws<IOException>(()=>faulted.Execute(Envelope(faulted,"transfer.99",payload:Transfer(true)),Principal));
            Assert.That(faulted.NeedsRecovery,Is.True);Assert.That(Bytes(Store().Load()),Is.EqualTo(before100));
            Assert.That(Store().GetSaveInfo().AcceptedMutationCount,Is.EqualTo(99));Assert.That(Store().GetSaveInfo().SnapshotCount,Is.EqualTo(2));
            p=Open();
            Mutate(p,1,99);Assert.That(Store().GetSaveInfo().AcceptedMutationCount,Is.EqualTo(100));Assert.That(Store().GetSaveInfo().SnapshotCount,Is.EqualTo(3));
        }
        [Test] public void FND_12_ManualSnapshotRejectsStaleMetadataImage()
        {
            Store().InitializeSave(Initial());var stale=Store().Load();var p=Open();p.Execute(Envelope(p,"reject","Unknown"),Principal);var before=File.ReadAllBytes(file);
            Assert.Throws<InvalidOperationException>(()=>Store().CreateSnapshot(stale));Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));
        }
        [TestCase("after_recovery_frame")][TestCase("after_save_manifest")]
        public void FND_12_SaveTransactionFaultRollsBackAllMembers(string point)
        {
            Store().InitializeSave(Initial());var before=Bytes(Store().Load());var p=Open(Store(s=>{if(s==point)throw new IOException("fault");}));
            Assert.Throws<IOException>(()=>p.Execute(Envelope(p,"fault"),Principal));Assert.That(p.NeedsRecovery,Is.True);Assert.That(Bytes(Store().Load()),Is.EqualTo(before));Assert.That(Store().GetSaveInfo().Sequence,Is.EqualTo(0));Assert.That(Store().GetSaveInfo().SnapshotCount,Is.EqualTo(1));
        }
        [Test] public void FND_12_ManualSnapshotFaultRollsBackWithoutWorldChange()
        {
            Store().InitializeSave(Initial());var p=Open();Mutate(p,1);var before=File.ReadAllBytes(file);
            Assert.Throws<IOException>(()=>Store(s=>{if(s=="after_snapshot")throw new IOException("fault");}).CreateSnapshot(p.InspectCommitted()));Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));Assert.That(Store().GetSaveInfo().SnapshotCount,Is.EqualTo(1));
        }
        [Test] public void FND_12_MigrationCreatesBackupAndPreservesOriginalAndExactHead()
        {
            Store().Initialize(Initial());var p=Open();Mutate(p,2);p.Execute(Envelope(p,"metadata","Unknown"),Principal);var before=File.ReadAllBytes(file);var expected=Bytes(p.InspectCommitted());
            var destination=Path.Combine(folder,"v2.db");var backup=Path.Combine(folder,"backup.db");Store().MigrateToNewFile(destination,backup,"migrated-seed");
            Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));Assert.That(Bytes(Store(path:backup).Load()),Is.EqualTo(expected));Assert.That(Bytes(Store(path:destination).Load()),Is.EqualTo(expected));Assert.That(Store(path:destination).GetSaveInfo().WorldSeed,Is.EqualTo("migrated-seed"));Assert.That(Store(path:destination).GetSaveInfo().AcceptedMutationCount,Is.EqualTo(0));
        }
        [TestCase("migration_after_backup")][TestCase("migration_before_commit")][TestCase("migration_after_copy")]
        public void FND_12_MigrationFaultKeepsSourceAndRecoverableBackup(string point)
        {
            Store().Initialize(Initial());var before=File.ReadAllBytes(file);var destination=Path.Combine(folder,"failed.db");var backup=Path.Combine(folder,"backup.db");
            Assert.Throws<IOException>(()=>Store(s=>{if(s==point)throw new IOException("fault");}).MigrateToNewFile(destination,backup));Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));Assert.That(Bytes(Store(path:backup).Load()),Is.EqualTo(Bytes(Initial())));
        }
        [TestCase("destination")][TestCase("backup")]
        public void FND_12_MigrationNeverOverwritesExistingPaths(string occupied)
        {
            Store().Initialize(Initial());var destination=Path.Combine(folder,"v2.db");var backup=Path.Combine(folder,"backup.db");var target=occupied=="backup"?backup:destination;File.WriteAllText(target,"keep me");var before=File.ReadAllBytes(file);
            Assert.Catch(()=>Store().MigrateToNewFile(destination,backup));Assert.That(File.ReadAllText(target),Is.EqualTo("keep me"));Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));
        }
        [Test] public void FND_12_MultipleMigrationBackupsAreRetained()
        {
            Store().Initialize(Initial());var backup1=Path.Combine(folder,"backup1.db");Store().MigrateToNewFile(Path.Combine(folder,"copy1.db"),backup1);var old=File.ReadAllBytes(backup1);
            var p=Open();Mutate(p,1);var backup2=Path.Combine(folder,"backup2.db");Store().MigrateToNewFile(Path.Combine(folder,"copy2.db"),backup2);
            Assert.That(File.ReadAllBytes(backup1),Is.EqualTo(old));Assert.That(Store(path:backup1).Load().World.Revision.Value,Is.EqualTo(1));Assert.That(Store(path:backup2).Load().World.Revision.Value,Is.EqualTo(2));
        }
        [Test] public void FND_12_UnknownSchemaMigrationRefusesWithoutSourceMutation()
        {
            Store().Initialize(Initial());Sql("UPDATE world_meta SET schema_version=99");var before=File.ReadAllBytes(file);
            Assert.Catch(()=>Store().MigrateToNewFile(Path.Combine(folder,"v2.db"),Path.Combine(folder,"backup.db")));Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));
        }
        [Test] public void FND_12_CorruptNewestSnapshotFallsBackToExactHeadIncludingMetadata()
        {
            Store().InitializeSave(Initial());var p=Open();Mutate(p,2);Store().CreateSnapshot(p.InspectCommitted());Mutate(p,1,2);var rejected=Envelope(p,"last-reject","Unknown");p.Execute(rejected,Principal);var expected=Bytes(p.InspectCommitted());
            Sql("UPDATE save_points SET image=x'00' WHERE sequence=(SELECT MAX(sequence) FROM save_points); UPDATE world_meta SET image=x'00'");var before=File.ReadAllBytes(file);var destination=Path.Combine(folder,"recovered.db");
            Assert.That(Bytes(Store().RecoverToNewFile(destination)),Is.EqualTo(expected));Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));var recovered=Open(Store(path:destination));Assert.That(recovered.Execute(rejected,Principal).Outcome,Is.EqualTo(CommandOutcome.Rejected));Assert.That(Bytes(recovered.InspectCommitted()),Is.EqualTo(expected));
        }
        [TestCase("middle")][TestCase("tail")][TestCase("head_hash")][TestCase("frame_hash")]
        public void FND_12_BrokenRecoveryChainRefusesRatherThanSilentlyLosingCommits(string flaw)
        {
            Store().InitializeSave(Initial());var p=Open();Mutate(p,3);p.Execute(Envelope(p,"last-metadata","Unknown"),Principal);
            if(flaw=="middle")Sql("DELETE FROM recovery_images WHERE sequence=2");if(flaw=="tail")Sql("DELETE FROM recovery_images WHERE sequence=(SELECT MAX(sequence) FROM recovery_images)");if(flaw=="head_hash")Sql("UPDATE save_manifest SET head_sha='invalid'");if(flaw=="frame_hash")Sql("UPDATE recovery_images SET sha256='invalid' WHERE sequence=2");
            Sql("UPDATE world_meta SET image=x'00'");var before=File.ReadAllBytes(file);Assert.Catch(()=>Store().RecoverToNewFile(Path.Combine(folder,"recovered.db")));Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));
        }
        [TestCase("schema")][TestCase("content")][TestCase("rule")][TestCase("codec")][TestCase("rng")]
        public void FND_12_RecoveryRejectsIncompatibleVersionsWithoutSourceMutation(string flaw)
        {
            Store().InitializeSave(Initial());if(flaw=="schema")Sql("UPDATE world_meta SET schema_version=99");if(flaw=="rule")Sql("UPDATE save_manifest SET rule_version='future'");if(flaw=="codec")Sql("UPDATE save_manifest SET codec_version='future'");if(flaw=="rng")Sql("UPDATE save_manifest SET rng_version='future'");
            var before=File.ReadAllBytes(file);var store=flaw=="content"?new SqliteWorldStore(file,new string('b',64)):Store();Assert.Catch(()=>store.RecoverToNewFile(Path.Combine(folder,"recovered.db")));Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));
        }
        [Test] public void FND_12_RecoveryNeverOverwritesDestination()
        {
            Store().InitializeSave(Initial());var destination=Path.Combine(folder,"existing.db");File.WriteAllText(destination,"keep me");var before=File.ReadAllBytes(file);Assert.Catch(()=>Store().RecoverToNewFile(destination));Assert.That(File.ReadAllText(destination),Is.EqualTo("keep me"));Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));
        }
        [Test] public void FND_12_PausedSchedulerRecoversAndResumesWithoutDuplicateExecution()
        {
            Store().InitializeSave(Initial());var p=Open();for(int i=0;i<2;i++){var payload=Transfer();payload["due_tick"]=10L+i;payload["phase"]=3;payload["priority"]=100;p.Execute(Envelope(p,"job."+i,"ScheduleTransfer",payload),Principal);}
            p.Execute(Envelope(p,"advance","AdvanceTime",new Dictionary<string,object?>{{"target_tick",20L}}),Principal);Assert.That(p.DrainAdvance(Principal,1).Status,Is.EqualTo(AdvanceStatus.PausedBudget));var expected=Bytes(p.InspectCommitted());Sql("UPDATE world_meta SET image=x'00'");var destination=Path.Combine(folder,"recovered.db");Assert.That(Bytes(Store().RecoverToNewFile(destination)),Is.EqualTo(expected));
            var resumed=Open(Store(path:destination));Assert.That(resumed.DrainAdvance(Principal).Status,Is.EqualTo(AdvanceStatus.Completed));var result=Store(path:destination).Load();Assert.That(result.Scheduler.Executions.Count,Is.EqualTo(2));Assert.That(result.World.Lots[0].OnHand,Is.EqualTo(8));Assert.That(result.World.Tick.Value,Is.EqualTo(20));
        }
        [Test] public void FND_12_ChecksummedMalformedNewestSnapshotFallsBackToOlderValidSnapshot()
        {
            Store().InitializeSave(Initial());var p=Open();Mutate(p,2);Store().CreateSnapshot(p.InspectCommitted());Mutate(p,1,2);var expected=Bytes(p.InspectCommitted());
            var malformed=new byte[]{0};Sql("UPDATE save_points SET image=x'00',sha256='"+CommittedWorldCodec.Hash(malformed)+"' WHERE sequence=(SELECT MAX(sequence) FROM save_points); UPDATE world_meta SET image=x'00'");
            Assert.That(Bytes(Store().RecoverToNewFile(Path.Combine(folder,"recovered.db"))),Is.EqualTo(expected));
        }
        [TestCase("world_seed='forged'")][TestCase("accepted_mutation_count=99")][TestCase("world_id='0000000000000002'")]
        public void FND_12_ManifestTamperingIsRefused(string mutation)
        {
            Store().InitializeSave(Initial(),"original");Sql("UPDATE save_manifest SET "+mutation);var before=File.ReadAllBytes(file);Assert.Catch(()=>Store().Load());Assert.Catch(()=>Store().RecoverToNewFile(Path.Combine(folder,"recovered.db")));Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));
        }
        [Test] public void FND_12_MissingSchema2ManifestRefusesLoadAndCommit()
        {
            Store().InitializeSave(Initial());var p=Open();Sql("DELETE FROM save_manifest");var before=File.ReadAllBytes(file);Assert.Catch(()=>Store().Load());Assert.Catch(()=>p.Execute(Envelope(p,"new"),Principal));Assert.That(p.NeedsRecovery,Is.True);Assert.That(File.ReadAllBytes(file),Is.EqualTo(before));
        }
        [Test] public void FND_12_RecoveryPreserves99MutationCadenceForNextAutomaticSnapshot()
        {
            Store().InitializeSave(Initial(),"retained");var p=Open();Mutate(p,99);Sql("UPDATE world_meta SET image=x'00'");var destination=Path.Combine(folder,"recovered.db");Store().RecoverToNewFile(destination);var recoveredStore=Store(path:destination);var before=recoveredStore.GetSaveInfo();Assert.That(before.AcceptedMutationCount,Is.EqualTo(99));Assert.That(before.WorldSeed,Is.EqualTo("retained"));
            var resumed=Open(recoveredStore);Mutate(resumed,1,99);var after=recoveredStore.GetSaveInfo();Assert.That(after.AcceptedMutationCount,Is.EqualTo(100));Assert.That(after.SnapshotCount,Is.EqualTo(before.SnapshotCount+1));
        }
    }
}
