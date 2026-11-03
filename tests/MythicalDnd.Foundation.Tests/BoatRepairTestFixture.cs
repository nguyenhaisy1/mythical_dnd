using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Domain.Repair;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Repair;

namespace MythicalDnd.Foundation.Tests
{
    internal static class BoatRepairTestFixture
    {
        internal static EntityId<Actor> Actor => EntityId<Actor>.Parse("w000000000000000e/actor/0000000000000001");
        internal static EntityId<Item> Boat => EntityId<Item>.Parse("w000000000000000e/item/0000000000000001");
        internal static EntityId<Item> Tool => EntityId<Item>.Parse("w000000000000000e/item/0000000000000002");
        internal static EntityId<Lot> Timber => EntityId<Lot>.Parse("w000000000000000e/lot/0000000000000001");
        internal static EntityId<Activity> Activity => EntityId<Activity>.Parse("w000000000000000e/activity/0000000000000001");
        internal static ExecutionPrincipal Principal => new ExecutionPrincipal(Actor, CommandSource.Player);
        internal static RuleRegistry Rules() => new RuleRegistry(new IRuleHandler[] { new TransferResourceRule() });
        internal static Dictionary<string,object?> Authoring()
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"boat-repair-profile.json")));
            object? Read(JsonElement value) => value.ValueKind switch {
                JsonValueKind.Object => value.EnumerateObject().ToDictionary(p=>p.Name,p=>Read(p.Value),StringComparer.Ordinal),
                JsonValueKind.String => value.GetString(), JsonValueKind.Number => value.GetInt64(),
                JsonValueKind.True => true, JsonValueKind.False => false, JsonValueKind.Null => null,
                _ => throw new FormatException("TEST_PROFILE_TYPE") };
            return (Dictionary<string,object?>)Read(doc.RootElement)!;
        }
        internal static RepairContentRegistry Registry() => RepairContentRegistry.Compile(Authoring());
        internal static CommittedWorld Initial(RepairContentRegistry? registry=null,int condition=500,long timber=10)
        {
            registry ??= Registry();var ids=IdentitySnapshot.Empty(14).BeginStage(1);
            var actor=ids.Allocate<Actor>();var location=ids.Allocate<Settlement>();var lot=ids.Allocate<Lot>();var spare=ids.Allocate<Lot>();
            var boat=ids.Allocate<Item>();var tool=ids.Allocate<Item>();
            var repair=new RepairState(registry.Recipe,new[]{new RepairItemState(boat,RepairItemKind.Boat,actor.Value,location,condition),
                new RepairItemState(tool,RepairItemKind.RepairTool,actor.Value,location,1000)},new[]{new RepairCapability(actor,true,tool)});
            var world=new WorldSnapshot(ids.PrepareSnapshot(),new WorldTick(0),new[]{new ActorState(actor,true,location)},
                new[]{new ResourceLot(lot,registry.Recipe.MaterialCommodity,actor.Value,location,timber,0),new ResourceLot(spare,registry.Recipe.MaterialCommodity,actor.Value,location,0,0)},repair:repair);
            return new CommandProcessor(world,Rules(),repairRecipe:registry.Recipe).InspectCommitted();
        }
        internal static SqliteWorldStore Store(string file,RepairContentRegistry registry,Action<string>? probe=null)
            => new SqliteWorldStore(file,registry.ContentHash,probe,expectedRuleVersion:SqliteWorldStore.RepairRuleVersion);
        internal static CommandProcessor Open(SqliteWorldStore store,RepairContentRegistry registry)
            => new CommandProcessor(store.Load(),Rules(),store,repairRecipe:registry.Recipe);
        internal static CommandEnvelope Command(string id,string type,ulong revision,long tick,Dictionary<string,object?> payload)
            => new CommandEnvelope(id,type,Actor.ToString(),"Player",tick,revision,1,payload,"repair.test");
        internal static CommandEnvelope Start(ulong revision=1,long tick=0,string id="repair.start") => Command(id,"StartRepair",revision,tick,new Dictionary<string,object?> {
            ["actor_id"]=Actor.ToString(),["item_id"]=Boat.ToString(),["material_lot_id"]=Timber.ToString(),["recipe_id"]="recipe.boat-repair@1" });
        internal static CommandEnvelope Advance(ulong revision=2,long tick=0,long target=60,string id="repair.advance")
            => Command(id,"AdvanceTime",revision,tick,new Dictionary<string,object?>{["target_tick"]=target});
        internal static CommandEnvelope Cancel(ulong revision,long tick=0,string id="repair.cancel")
            => Command(id,"CancelActivity",revision,tick,new Dictionary<string,object?>{["activity_id"]=Activity.ToString(),["reason"]="player_cancel"});
        internal static string Hash(CommittedWorld value) => CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(value));
    }
}
