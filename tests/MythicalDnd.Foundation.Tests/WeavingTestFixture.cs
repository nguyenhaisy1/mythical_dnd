using System;
using System.Collections.Generic;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Domain.Origins;
using Game.Domain.Weaving;
using Game.Foundation.Identity;
using Game.Foundation.Time;

namespace MythicalDnd.Foundation.Tests
{
    internal static class WeavingTestFixture
    {
        internal static WeavingRecipe Recipe => new WeavingRecipe(2, 30, 1, 40, new string('a', 64));
        internal static WorldSnapshot World(int skill = 43, bool alive = true, long tick = 0, long input = 8, long output = 0)
        {
            var selection = OriginSelector.Select(new string('0', 64), new string('c', 64), new string('b', 64),
                new[] { new OriginCandidate("actor.vac-lam@1", "household.vac-tong@1", OriginSelector.HomeDefinitionId, true, 22) });
            var stage = IdentitySnapshot.Empty(selection.Origin.Principal.Value.World).BeginStage(1);
            var actor = stage.Allocate<Actor>(); _ = stage.Allocate<Household>(); var home = stage.Allocate<Settlement>();
            var fiber = stage.Allocate<Lot>(); var cord = stage.Allocate<Lot>();
            var work = new WeavingState(Recipe, new string('d', 64), actor, home, fiber, cord, skill);
            return new WorldSnapshot(stage.PrepareSnapshot(), new WorldTick(tick), new[] { new ActorState(actor, alive, home) },
                new[] { new ResourceLot(fiber, Recipe.MaterialCommodity, actor.Value, home, input, 0),
                    new ResourceLot(cord, Recipe.OutputCommodity, actor.Value, home, output, 0) }, new[] { selection.Rng }, origin: selection.Origin, weaving: work);
        }
        internal static RuleRegistry Rules() => new RuleRegistry(new IRuleHandler[] { new TransferResourceRule() });
        internal static CommandProcessor Processor(WorldSnapshot? world = null) => new CommandProcessor(world ?? World(), Rules(), weavingRecipe: Recipe);
        internal static ExecutionPrincipal Principal(WorldSnapshot world) => new ExecutionPrincipal(world.Weaving!.ActorId, CommandSource.Player);
        internal static CommandEnvelope Command(CommandProcessor p, string id, string type, IReadOnlyDictionary<string, object?> payload)
        {
            var w = p.InspectCommitted().World;
            return new CommandEnvelope(id, type, w.Weaving!.ActorId.ToString(), "Player", w.Tick.Value, w.Revision.Value, 1, payload, "weaving.test");
        }
        internal static CommandEnvelope Start(CommandProcessor p, string id = "weave.start")
        {
            var s = p.InspectCommitted().World.Weaving!;
            return Command(p, id, "StartWeaving", new Dictionary<string, object?> { ["actor_id"] = s.ActorId.ToString(),
                ["input_lot_id"] = s.InputLotId.ToString(), ["output_lot_id"] = s.OutputLotId.ToString() });
        }
        internal static CommandEnvelope Cancel(CommandProcessor p, string id = "weave.cancel")
            => Command(p, id, "CancelWeaving", new Dictionary<string, object?> { ["activity_id"] = p.InspectCommitted().World.Weaving!.Activities.Last().Id.ToString(), ["reason"] = "player_cancel" });
        internal static void Advance(CommandProcessor p, long target, string id = "weave.advance")
        {
            var command = Command(p, id, "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = target });
            var principal = Principal(p.InspectCommitted().World); p.Execute(command, principal); p.DrainAdvance(principal);
        }
    }
}
