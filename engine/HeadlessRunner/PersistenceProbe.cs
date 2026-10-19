using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using Game.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;

namespace Game.Headless
{
    internal static class PersistenceProbe
    {
        internal static int Run(string path,string mode,string? point)
        {
            var store=new SqliteWorldStore(path,new string('a',64),step=>{
                if(mode=="crash"&&step==point){Console.WriteLine("READY:"+step);Console.Out.Flush();Thread.Sleep(Timeout.Infinite);}
            });
            var rules=new RuleRegistry(new[]{new TransferResourceRule()});
            if(mode=="init")
            {
                var ids=IdentitySnapshot.Empty(1).BeginStage(1);var a=ids.Allocate<Actor>();var loc=ids.Allocate<Settlement>();var src=ids.Allocate<Lot>();var dst=ids.Allocate<Lot>();
                var world=new WorldSnapshot(ids.PrepareSnapshot(),new WorldTick(0),new[]{new ActorState(a,true,loc)},new[]{new ResourceLot(src,DefinitionId.Parse("commodity.timber@1"),a.Value,loc,10,0),new ResourceLot(dst,DefinitionId.Parse("commodity.timber@1"),a.Value,loc,0,0)});
                store.Initialize(new CommandProcessor(world,rules).InspectCommitted());Console.WriteLine("INITIALIZED");return 0;
            }
            var before=store.Load();var p=new CommandProcessor(before,rules,store);var actor=before.World.Actors[0].Id;
            var command=new CommandEnvelope("crash-transfer","TransferResource",actor.ToString(),"Player",0,1,1,new Dictionary<string,object?>{{"source_lot_id",before.World.Lots[0].Id.ToString()},{"destination_lot_id",before.World.Lots[1].Id.ToString()},{"quantity",3L}},"crash-probe");
            var receipt=p.Execute(command,new ExecutionPrincipal(actor,CommandSource.Player));var after=store.Load();
            using var c=new SqliteConnection(new SqliteConnectionStringBuilder{DataSource=path,Pooling=false}.ToString());c.Open();using var sql=c.CreateCommand();sql.CommandText="SELECT sqlite_version()";
            bool passed=after.World.Lots[0].OnHand==7&&after.World.Lots[1].OnHand==3&&after.AcceptedReceipts.Count==1&&after.World.Revision.Value==2&&receipt.Outcome==CommandOutcome.Accepted;
            Console.WriteLine(JsonSerializer.Serialize(new{passed,beforeRevision=before.World.Revision.Value,beforeSource=before.World.Lots[0].OnHand,afterRevision=after.World.Revision.Value,afterSource=after.World.Lots[0].OnHand,receipts=after.AcceptedReceipts.Count,sqliteVersion=sql.ExecuteScalar(),providerVersion=typeof(SqliteConnection).Assembly.GetName().Version?.ToString()}));return passed?0:1;
        }
    }
}
