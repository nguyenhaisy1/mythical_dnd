using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Game.Application.CommandPipeline;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Domain.Repair;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Repair;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using F=MythicalDnd.Foundation.Tests.BoatRepairTestFixture;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class BoatRepairPersistenceTests
    {
        private string folder=null!;private string file=null!;private RepairContentRegistry registry=null!;
        [SetUp] public void Setup(){folder=Path.Combine(Path.GetTempPath(),"mythical-repair-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(folder);file=Path.Combine(folder,"world.db");registry=F.Registry();}
        [TearDown] public void Cleanup(){Directory.Delete(folder,true);}
        private SqliteWorldStore Store(Action<string>? probe=null)=>F.Store(file,registry,probe);
        private CommandProcessor Started()
        {var s=Store();s.InitializeSave(F.Initial(registry));var p=F.Open(s,registry);Assert.That(p.Execute(F.Start(),F.Principal).Outcome,Is.EqualTo(CommandOutcome.Accepted));return p;}
        private static void Complete(CommandProcessor p)
        {
            var w=p.InspectCommitted().World;
            Assert.That(p.Execute(F.Advance(w.Revision.Value,w.Tick.Value),F.Principal).Outcome,Is.EqualTo(CommandOutcome.AcceptedPending));
            Assert.That(p.DrainAdvance(F.Principal).Status,Is.EqualTo(AdvanceStatus.Completed));
        }
        private static void AssertRunning(CommittedWorld state)
        {Assert.That(state.World.GetLot(F.Timber).OnHand,Is.EqualTo(10));Assert.That(state.World.GetLot(F.Timber).Reserved,Is.EqualTo(2));Assert.That(state.World.Repair!.GetItem(F.Boat).Condition,Is.EqualTo(500));Assert.That(state.World.Repair.GetActivity(F.Activity).Status,Is.EqualTo(RepairActivityStatus.Running));}
        private static void AssertCompleted(CommittedWorld state)
        {Assert.That(state.World.GetLot(F.Timber).OnHand,Is.EqualTo(8));Assert.That(state.World.GetLot(F.Timber).Reserved,Is.Zero);Assert.That(state.World.Repair!.GetItem(F.Boat).Condition,Is.EqualTo(700));Assert.That(state.World.Repair.GetActivity(F.Activity).Status,Is.EqualTo(RepairActivityStatus.Completed));Assert.That(state.Journal.Events.Count(e=>e.EventType=="RepairCompleted"),Is.EqualTo(1));}
        [Test] public void VS_03A_Image2IncludesExplicitEmptyProfileAndRetainsLegacyImage1()
        {
            var empty=new WorldSnapshot(IdentitySnapshot.Empty(14),new WorldTick(0),Array.Empty<ActorState>(),Array.Empty<ResourceLot>(),repair:new RepairState(registry.Recipe,Array.Empty<RepairItemState>(),Array.Empty<RepairCapability>()));
            var state=new CommandProcessor(empty,F.Rules(),repairRecipe:registry.Recipe).InspectCommitted();var image=CommittedWorldCodec.Encode(state);var copy=CommittedWorldCodec.Decode(image);
            Assert.That(copy.World.Repair,Is.Not.Null);Assert.That(copy.World.Repair!.Items,Is.Empty);Assert.That(CommittedWorldCodec.Encode(copy),Is.EqualTo(image));
            Assert.That(Encoding.UTF8.GetString(image),Does.Contain("image_versioni2;"));
            var legacy=new CommandProcessor(new WorldSnapshot(IdentitySnapshot.Empty(14),new WorldTick(0),Array.Empty<ActorState>(),Array.Empty<ResourceLot>()),F.Rules()).InspectCommitted();
            var old=CommittedWorldCodec.Encode(legacy);Assert.That(Encoding.UTF8.GetString(old),Does.Contain("image_versioni1;"));Assert.That(CommittedWorldCodec.Decode(old).World.Repair,Is.Null);
            var store=Store();store.InitializeSave(state);Assert.That(store.GetSaveInfo().SchemaVersion,Is.EqualTo(2));Assert.That(F.Hash(store.Load()),Is.EqualTo(F.Hash(state)));
        }
        [Test] public void VS_03A_RunningRepairReopensCompletesAndRestoredRetryCannotDoubleConsume()
        {
            var first=Started();AssertRunning(first.InspectCommitted());var reopened=F.Open(Store(),registry);AssertRunning(reopened.InspectCommitted());
            Complete(reopened);AssertCompleted(Store().Load());var before=F.Hash(Store().Load());
            var again=F.Open(Store(),registry);Assert.That(again.Execute(F.Start(),F.Principal).Outcome,Is.EqualTo(CommandOutcome.Accepted));Assert.That(F.Hash(Store().Load()),Is.EqualTo(before));
            var restored=CommittedWorldCodec.Decode(CommittedWorldCodec.Encode(Store().Load()));Assert.That(restored.World.Identity.Resolve<Activity>(F.Activity.Value).Lifecycle,Is.EqualTo(EntityLifecycle.Retired));
        }
        [Test] public void VS_03A_CancelDuringPendingAdvancePersistsAndReleasesWithoutCreatingMaterial()
        {
            var first=Started();Assert.That(first.Execute(F.Advance(),F.Principal).Outcome,Is.EqualTo(CommandOutcome.AcceptedPending));
            var reopened=F.Open(Store(),registry);var cancel=F.Cancel(reopened.InspectCommitted().World.Revision.Value);
            Assert.That(reopened.Execute(cancel,F.Principal).Outcome,Is.EqualTo(CommandOutcome.Accepted));var before=F.Hash(Store().Load());
            var retry=F.Open(Store(),registry);Assert.That(retry.Execute(cancel,F.Principal).Outcome,Is.EqualTo(CommandOutcome.Accepted));Assert.That(F.Hash(Store().Load()),Is.EqualTo(before));
            Assert.That(retry.DrainAdvance(F.Principal).Status,Is.EqualTo(AdvanceStatus.Completed));var state=Store().Load();
            Assert.That(state.World.GetLot(F.Timber).OnHand,Is.EqualTo(10));Assert.That(state.World.GetLot(F.Timber).Reserved,Is.Zero);Assert.That(state.World.Repair!.GetItem(F.Boat).Condition,Is.EqualTo(500));Assert.That(state.World.Repair.GetActivity(F.Activity).Status,Is.EqualTo(RepairActivityStatus.Cancelled));
            Assert.That(state.Journal.Events.Count(e=>e.EventType=="RepairCompleted"),Is.Zero);
            Assert.That(retry.Execute(F.Cancel(state.World.Revision.Value,state.World.Tick.Value,"cancel.again"),F.Principal).ErrorCode,Is.EqualTo("ACTIVITY_STATE"));
        }
        [TestCase("before_commit")][TestCase("after_commit")]
        public void VS_03A_StoreFailureAndResponseLossRetryStartOnlyOnce(string boundary)
        {
            bool armed=false;var s=Store(point=>{if(armed&&point==boundary){armed=false;throw new IOException("injected");}});s.InitializeSave(F.Initial(registry));var p=F.Open(s,registry);var before=p.InspectCommitted();armed=true;
            Assert.Throws<IOException>(()=>p.Execute(F.Start(),F.Principal));Assert.That(p.InspectCommitted(),Is.SameAs(before));Assert.That(p.NeedsRecovery,Is.True);
            var disk=Store().Load();Assert.That(disk.World.Repair!.Activities.Count,Is.EqualTo(boundary=="before_commit"?0:1));
            var retry=F.Open(Store(),registry);Assert.That(retry.Execute(F.Start(),F.Principal).Outcome,Is.EqualTo(CommandOutcome.Accepted));AssertRunning(Store().Load());Complete(retry);AssertCompleted(Store().Load());
        }
        [Test] public void VS_03A_RunningSnapshotRecoveryKeepsProfileAndResumesRealCompletion()
        {
            var p=Started();Store().CreateSnapshot(p.InspectCommitted());var running=F.Hash(Store().Load());Sql("UPDATE world_meta SET image=x'00'");var source=File.ReadAllBytes(file);var target=Path.Combine(folder,"recovered.db");
            Assert.That(F.Hash(Store().RecoverToNewFile(target)),Is.EqualTo(running));Assert.That(File.ReadAllBytes(file),Is.EqualTo(source));
            var output=F.Store(target,registry);Assert.That(F.Hash(output.Load()),Is.EqualTo(running));var resumed=F.Open(output,registry);Complete(resumed);AssertCompleted(output.Load());
        }
        [Test] public void VS_03A_WrongRuleOrContentRefusesWithoutMutatingSource()
        {
            Started();var source=File.ReadAllBytes(file);
            Assert.Throws<InvalidDataException>(()=>new SqliteWorldStore(file,registry.ContentHash).Load());
            Assert.Throws<InvalidDataException>(()=>new SqliteWorldStore(file,new string('b',64),expectedRuleVersion:SqliteWorldStore.RepairRuleVersion).Load());
            Assert.Throws<ArgumentException>(()=>new SqliteWorldStore(file,registry.ContentHash,expectedRuleVersion:"unknown"));
            Assert.That(File.ReadAllBytes(file),Is.EqualTo(source));
            var wrong=Path.Combine(folder,"wrong.db");Assert.Throws<InvalidDataException>(()=>new SqliteWorldStore(wrong,registry.ContentHash).InitializeSave(F.Initial(registry)));Assert.That(File.Exists(wrong),Is.False);
        }
        [Test] public void VS_03A_RepairPhysicalSchemaMigrationKeepsProfileAndSourceBytes()
        {
            var source=Store();source.Initialize(F.Initial(registry));var p=F.Open(source,registry);p.Execute(F.Start(),F.Principal);var state=source.Load();var bytes=File.ReadAllBytes(file);
            var target=Path.Combine(folder,"migrated.db");var backup=Path.Combine(folder,"backup.db");
            Assert.That(F.Hash(source.MigrateToNewFile(target,backup)),Is.EqualTo(F.Hash(state)));Assert.That(File.ReadAllBytes(file),Is.EqualTo(bytes));
            Assert.That(F.Hash(F.Store(backup,registry).Load()),Is.EqualTo(F.Hash(state)));var output=F.Store(target,registry);
            Assert.That(output.GetSaveInfo().SchemaVersion,Is.EqualTo(2));AssertRunning(output.Load());var resumed=F.Open(output,registry);Complete(resumed);AssertCompleted(output.Load());
        }
        [Test] public void VS_03A_ItemAndActivityRelationalMirrorsAreVerified()
        {
            Started();Sql("UPDATE entities SET payload=x'00' WHERE id='"+F.Boat+"'");var source=File.ReadAllBytes(file);
            Assert.Throws<InvalidDataException>(()=>Store().Load());Assert.That(File.ReadAllBytes(file),Is.EqualTo(source));
        }
        [TestCase("version")][TestCase("profile")][TestCase("fingerprint")][TestCase("hold")][TestCase("jobref")][TestCase("unknown")]
        public void VS_03A_Image2RejectsMalformedExtensionAndUnknownFields(string flaw)
        {
            var p=Started();var image=CommittedWorldCodec.Encode(p.InspectCommitted());var text=Encoding.UTF8.GetString(image);
            if(flaw=="version")text=text.Replace("image_versioni2;","image_versioni3;");
            if(flaw=="profile")text=text.Replace("boat-repair-v1","boat-repair-v9");
            if(flaw=="fingerprint")text=text.Replace(registry.Recipe.Fingerprint,new string('0',64));
            if(flaw=="hold")text=text.Replace("reservedi2;","reservedi1;");
            if(flaw=="jobref")
            {
                var id=F.Activity.ToString();var needle="s8:activitys"+Encoding.UTF8.GetByteCount(id)+":"+id;
                Assert.That(text,Does.Contain(needle));text=text.Replace(needle,needle.Substring(0,needle.Length-id.Length)+id.Substring(0,id.Length-1)+"9");
            }
            if(flaw=="unknown")
            {
                Assert.That(text.StartsWith("MDC1o",StringComparison.Ordinal),Is.True);var colon=text.IndexOf(':',5);var count=int.Parse(text.Substring(5,colon-5),System.Globalization.CultureInfo.InvariantCulture);
                text="MDC1o"+(count+1).ToString(System.Globalization.CultureInfo.InvariantCulture)+":"+text.Substring(colon+1)+"s3:zzzn";
            }
            var corrupt=Encoding.UTF8.GetBytes(text);Assert.That(corrupt,Is.Not.EqualTo(image));Assert.Catch(()=>CommittedWorldCodec.Decode(corrupt));
        }
        [TestCase("root_unknown")][TestCase("recipe_unknown")][TestCase("decimal")][TestCase("handler")][TestCase("missing")]
        public void VS_03A_RepairCompilerIsStrictAndIndependentOfInputMutation(string flaw)
        {
            var input=F.Authoring();var recipe=(Dictionary<string,object?>)input["recipe"]!;
            if(flaw=="root_unknown")input["script"]="x";
            if(flaw=="recipe_unknown")recipe["sql"]="x";
            if(flaw=="decimal")recipe["material_quantity"]=2.0;
            if(flaw=="handler")recipe["handler"]="arbitrary";
            if(flaw=="missing")recipe.Remove("material");
            Assert.Catch(()=>RepairContentRegistry.Compile(input));
            var good=F.Authoring();var compiled=RepairContentRegistry.Compile(good);var hash=compiled.ContentHash;((Dictionary<string,object?>)good["recipe"]!)["condition_gain"]=1L;good["status"]="changed";
            Assert.That(compiled.ContentHash,Is.EqualTo(hash));Assert.That(compiled.Recipe.ConditionGain,Is.EqualTo(200));
        }
        private void Sql(string text){using var c=new SqliteConnection("Data Source="+file+";Pooling=False");c.Open();using var command=c.CreateCommand();command.CommandText=text;command.ExecuteNonQuery();}
    }
}
