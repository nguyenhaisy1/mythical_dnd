using System;
using System.IO;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Application.Projection;
using Game.Domain.Kernel;
using Game.Domain.Repair;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using Game.Infrastructure.Persistence;

namespace Game.Infrastructure.Repair
{
    /// <summary>Trusted composition for a fixed engineering fixture, not a random-life generator.</summary>
    public static class RepairDemoFactory
    {
        public const string ProfileId = "repair-client-2d-v1";
        public const string FixtureSeed = "sprint15-fixed-worksite-v1";
        private static EntityId<Actor> ActorId => EntityId<Actor>.Parse("w000000000000000f/actor/0000000000000001");
        private static EntityId<Settlement> Worksite => EntityId<Settlement>.Parse("w000000000000000f/settlement/0000000000000001");
        private static EntityId<Item> BoatId => EntityId<Item>.Parse("w000000000000000f/item/0000000000000001");
        private static EntityId<Item> ToolId => EntityId<Item>.Parse("w000000000000000f/item/0000000000000002");
        private static EntityId<Lot> MaterialId => EntityId<Lot>.Parse("w000000000000000f/lot/0000000000000001");

        public static IRepairClientSession Open(string databasePath, RepairContentRegistry registry)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            var store = new SqliteWorldStore(databasePath, registry.ContentHash,
                expectedRuleVersion: SqliteWorldStore.RepairRuleVersion);
            if (!File.Exists(databasePath)) store.InitializeSave(Initial(registry), FixtureSeed);
            CommittedWorld Load()
            {
                var state = store.Load();
                if (state.World.WorldId != 15 || store.GetSaveInfo().WorldSeed != FixtureSeed || state.World.Repair == null)
                    throw new InvalidDataException("REPAIR_CLIENT_FIXTURE_INCOMPATIBLE");
                var actor = state.World.GetActor(ActorId);
                var repair = state.World.Repair;
                // Fixed fixture identity is explicit; never adopt a compatible arbitrary world as the player.
                if (!actor.Location.Equals(Worksite) || !repair.GetItem(BoatId).Location.Equals(Worksite) ||
                    !repair.GetItem(ToolId).Location.Equals(Worksite) || !state.World.GetLot(MaterialId).Location.Equals(Worksite) ||
                    !repair.Capabilities.Any(c => c.ActorId.Equals(ActorId) && c.ToolItemId.Equals(ToolId)))
                    throw new InvalidDataException("REPAIR_CLIENT_FIXTURE_BINDING");
                return state;
            }
            CommandProcessor Processor(CommittedWorld state) => new CommandProcessor(state,
                new RuleRegistry(new[] { new TransferResourceRule() }), store, repairRecipe: registry.Recipe);
            var scope = new RepairObservationScope(new ExecutionPrincipal(ActorId, CommandSource.Player),
                BoatId, MaterialId, ToolId, Worksite, "Người thợ ở bến", "Bến Vạc · Bãi sửa thuyền",
                RepairObservationFields.All);
            return new RepairClientSession(Processor(Load()), scope, Processor, Load, store.CreateSnapshot,
                durableCommands: true);
        }

        private static CommittedWorld Initial(RepairContentRegistry registry)
        {
            var stage = IdentitySnapshot.Empty(15).BeginStage(1);
            var actor = stage.Allocate<Actor>(); var other = stage.Allocate<Actor>();
            var owner = stage.Allocate<Household>(); var location = stage.Allocate<Settlement>();
            var material = stage.Allocate<Lot>(); var privateLot = stage.Allocate<Lot>();
            var boat = stage.Allocate<Item>(); var tool = stage.Allocate<Item>(); var privateBoat = stage.Allocate<Item>();
            var repair = new RepairState(registry.Recipe, new[] {
                new RepairItemState(boat, RepairItemKind.Boat, owner.Value, location, 500),
                new RepairItemState(tool, RepairItemKind.RepairTool, owner.Value, location, 1000),
                new RepairItemState(privateBoat, RepairItemKind.Boat, other.Value, location, 321)
            }, new[] { new RepairCapability(actor, true, tool) });
            var world = new WorldSnapshot(stage.PrepareSnapshot(), new WorldTick(0), new[] {
                new ActorState(actor, true, location, new[] { owner.Value }), new ActorState(other, true, location)
            }, new[] {
                new ResourceLot(material, registry.Recipe.MaterialCommodity, owner.Value, location, 10, 0),
                new ResourceLot(privateLot, registry.Recipe.MaterialCommodity, other.Value, location, 917, 0)
            }, repair: repair);
            return new CommandProcessor(world, new RuleRegistry(new[] { new TransferResourceRule() }),
                repairRecipe: registry.Recipe).InspectCommitted();
        }
    }
}
