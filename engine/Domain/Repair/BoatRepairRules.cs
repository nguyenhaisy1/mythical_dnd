using System;
using System.Collections.Generic;
using System.Linq;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Time;

namespace Game.Domain.Repair
{
    /// <summary>A typed, immutable proposal; only compiled repair rules can construct it.</summary>
    public sealed class RepairPlan
    {
        public RepairState Repair { get; }
        public IReadOnlyList<ResourceLot> Lots { get; }
        public RepairActivity Activity { get; }
        internal RepairPlan(RepairState repair, IEnumerable<ResourceLot> lots, RepairActivity activity)
        {
            Repair = repair; Activity = activity;
            Lots = Array.AsReadOnly(lots.ToArray());
        }
    }
    public sealed class RepairEvaluation
    {
        public string? ErrorCode { get; }
        public RepairPlan? Plan { get; }
        public IReadOnlyList<RuleCheck> Checks { get; }
        internal RepairEvaluation(string? error, RepairPlan? plan, IEnumerable<RuleCheck> checks)
        { ErrorCode = error; Plan = plan; Checks = Array.AsReadOnly(checks.ToArray()); }
    }

    /// <summary>Pure repair-v1 validation and proposals. No identity allocation, mutation, storage, Unity or RNG.</summary>
    public static class BoatRepairRules
    {
        private sealed class Inspection
        {
            public readonly List<RuleCheck> Checks = new List<RuleCheck>();
            public bool Check(string name, bool passed, string? detail = null)
            { Checks.Add(new RuleCheck(name, passed, detail)); return passed; }
            public RepairEvaluation Reject(string code) => new RepairEvaluation(code, null, Checks);
            public RepairEvaluation Propose(RepairPlan plan) => new RepairEvaluation(null, plan, Checks);
        }
        public static RepairEvaluation EvaluateStart(WorldSnapshot world, EntityId<Actor> issuer,
            EntityId<Actor> actorId, EntityId<Item> itemId, EntityId<Lot> materialLotId,
            DefinitionId recipeId, EntityId<Activity> activityId, EntityId<Job> jobId,
            string startCommandId, string correlationId)
        {
            if (world == null || issuer == null || actorId == null || itemId == null || materialLotId == null || recipeId == null || activityId == null || jobId == null)
                throw new ArgumentNullException();
            var audit = new Inspection(); var repair = world.Repair;
            if (!audit.Check("repair_profile_enabled", repair != null)) return audit.Reject("COMMAND_UNKNOWN");
            if (!audit.Check("issuer_actor_match", issuer.Equals(actorId))) return audit.Reject("NOT_AUTHORIZED");
            if (!audit.Check("recipe_match", repair!.Recipe.Id.Equals(recipeId))) return audit.Reject("REF_NOT_FOUND");
            if (!audit.Check("new_reference_world", activityId.Value.World == world.WorldId && jobId.Value.World == world.WorldId)) return audit.Reject("REF_NOT_FOUND");
            try
            {
                var actor = world.GetActor(actorId); var recipe = repair.Recipe;
                var item = repair.GetItem(itemId); var lot = world.GetLot(materialLotId);
                var capability = repair.Capabilities.FirstOrDefault(c => c.ActorId.Equals(actorId));
                if (!audit.Check("actor_alive", actor.Alive)) return audit.Reject("ACTOR_DEAD");
                if (!audit.Check("qualified", capability != null && capability.Qualified)) return audit.Reject("NOT_AUTHORIZED");
                var tool = repair.GetItem(capability!.ToolItemId);
                var reason = CheckEligibility(audit, actor, item, lot, tool, actor.Location, recipe);
                if (reason != null) return audit.Reject(reason);
                if (!audit.Check("actor_item_tool_available", !repair.Activities.Any(a => a.Status == RepairActivityStatus.Running &&
                    (a.ActorId.Equals(actorId) || a.ItemId.Equals(itemId) || a.ToolItemId.Equals(tool.Id))))) return audit.Reject("ITEM_UNAVAILABLE");
                if (!audit.Check("material_available", lot.Available >= recipe.MaterialQuantity)) return audit.Reject("INSUFFICIENT_RESOURCE");
                WorldTick due;
                try { due = world.Tick.Add(recipe.DurationTicks); }
                catch (TimeRangeException) { audit.Check("due_tick_range", false); return audit.Reject("TIME_RANGE"); }
                audit.Check("due_tick_range", true);
                var activity = new RepairActivity(activityId, actorId, itemId, materialLotId, tool.Id, actor.Location,
                    jobId, world.Tick, due, recipe, startCommandId, correlationId, RepairActivityStatus.Running, recipe.MaterialQuantity);
                var proposedLot = new ResourceLot(lot.Id, lot.Commodity, lot.Owner, lot.Location, lot.OnHand, checked(lot.Reserved + recipe.MaterialQuantity));
                var proposedRepair = repair.With(newActivities: repair.Activities.Concat(new[] { activity }));
                return audit.Propose(new RepairPlan(proposedRepair, ReplaceLot(world, proposedLot), activity));
            }
            catch (IdentityException error) { audit.Check("reference_resolution", false, error.Code); return audit.Reject(error.Code); }
            catch (OverflowException) { audit.Check("numeric_range", false); return audit.Reject("NUMERIC_OVERFLOW"); }
        }

        public static RepairEvaluation EvaluateCancel(WorldSnapshot world, EntityId<Actor> issuer,
            EntityId<Activity> activityId, string reason)
        {
            if (world == null || issuer == null || activityId == null) throw new ArgumentNullException();
            var audit = new Inspection(); var repair = world.Repair;
            if (!audit.Check("repair_profile_enabled", repair != null)) return audit.Reject("COMMAND_UNKNOWN");
            if (!audit.Check("cancel_reason_valid", !string.IsNullOrWhiteSpace(reason) && reason.Length <= 128 && !reason.Any(char.IsControl))) return audit.Reject("INVALID_PAYLOAD");
            try
            {
                var actor = world.GetActor(issuer); var activity = repair!.GetActivity(activityId);
                if (!audit.Check("issuer_alive", actor.Alive)) return audit.Reject("ACTOR_DEAD");
                if (!audit.Check("controls_activity_actor", actor.Controls(activity.ActorId.Value))) return audit.Reject("NOT_AUTHORIZED");
                if (!audit.Check("activity_running", activity.Status == RepairActivityStatus.Running)) return audit.Reject("ACTIVITY_STATE");
                return audit.Propose(CancelPlan(world, activity, reason));
            }
            catch (IdentityException error) { audit.Check("reference_resolution", false, error.Code); return audit.Reject(error.Code); }
        }

        /// <summary>Scheduler must separately authenticate the job/activity/causation binding before using this proposal.</summary>
        public static RepairEvaluation EvaluateComplete(WorldSnapshot world, EntityId<Activity> activityId)
        {
            if (world == null || activityId == null) throw new ArgumentNullException();
            var audit = new Inspection(); var repair = world.Repair;
            if (!audit.Check("repair_profile_enabled", repair != null)) return audit.Reject("COMMAND_UNKNOWN");
            RepairActivity activity;
            try { activity = repair!.GetActivity(activityId); }
            catch (IdentityException error) { audit.Check("reference_resolution", false, error.Code); return audit.Reject(error.Code); }
            if (!audit.Check("activity_running", activity.Status == RepairActivityStatus.Running)) return audit.Reject("ACTIVITY_STATE");
            if (!audit.Check("due_tick_reached", world.Tick >= activity.DueTick)) return audit.Reject("ACTIVITY_STATE");
            try
            {
                var actor = world.GetActor(activity.ActorId); var item = repair.GetItem(activity.ItemId);
                var lot = world.GetLot(activity.MaterialLotId); var tool = repair.GetItem(activity.ToolItemId);
                var capability = repair.Capabilities.FirstOrDefault(c => c.ActorId.Equals(activity.ActorId));
                string? reason;
                if (!audit.Check("actor_alive", actor.Alive)) reason = "ACTOR_DEAD";
                else if (!audit.Check("qualified", capability != null && capability.Qualified)) reason = "NOT_AUTHORIZED";
                else if (!audit.Check("same_tool", capability!.ToolItemId.Equals(activity.ToolItemId))) reason = "ITEM_UNAVAILABLE";
                else reason = CheckEligibility(audit, actor, item, lot, tool, activity.Worksite, activity.Recipe);
                if (reason != null) return audit.Propose(CancelPlan(world, activity, reason));
                if (!audit.Check("reservation_intact", lot.Reserved >= activity.ReservedQuantity && lot.OnHand >= activity.ReservedQuantity))
                    return audit.Reject("RESERVATION_LOST"); // Invalid aggregate must not fabricate a release or resource replacement.
                var finished = activity.Finish(RepairActivityStatus.Completed);
                var updated = new RepairItemState(item.Id, item.Kind, item.Owner, item.Location,
                    Math.Min(activity.Recipe.ConditionMax, checked(item.Condition + activity.Recipe.ConditionGain)), item.Destroyed);
                var proposedLot = new ResourceLot(lot.Id, lot.Commodity, lot.Owner, lot.Location,
                    checked(lot.OnHand - activity.ReservedQuantity), checked(lot.Reserved - activity.ReservedQuantity));
                return audit.Propose(new RepairPlan(repair.With(repair.Items.Select(i => i.Id.Equals(item.Id) ? updated : i),
                    repair.Activities.Select(a => a.Id.Equals(activity.Id) ? finished : a)), ReplaceLot(world, proposedLot), finished));
            }
            catch (IdentityException error) { audit.Check("reference_resolution", false, error.Code); return audit.Reject(error.Code); }
            catch (OverflowException) { audit.Check("numeric_range", false); return audit.Reject("NUMERIC_OVERFLOW"); }
        }
        private static string? CheckEligibility(Inspection audit, ActorState actor, RepairItemState item,
            ResourceLot lot, RepairItemState tool, EntityId<Settlement> worksite, RepairRecipe recipe)
        {
            if (!audit.Check("boat_repairable", item.Kind == RepairItemKind.Boat && !item.Destroyed && item.Condition < recipe.ConditionMax)) return "ITEM_UNAVAILABLE";
            if (!audit.Check("tool_usable", tool.Kind == RepairItemKind.RepairTool && !tool.Destroyed && tool.Condition > 0)) return "ITEM_UNAVAILABLE";
            if (!audit.Check("controls_item_material_tool", actor.Controls(item.Owner) && actor.Controls(lot.Owner) && actor.Controls(tool.Owner))) return "NOT_AUTHORIZED";
            if (!audit.Check("worksite_match", actor.Location.Equals(worksite) && item.Location.Equals(worksite) && lot.Location.Equals(worksite) && tool.Location.Equals(worksite))) return "LOCATION_MISMATCH";
            if (!audit.Check("material_commodity", lot.Commodity.Equals(recipe.MaterialCommodity))) return "COMMODITY_MISMATCH";
            return null;
        }
        private static ResourceLot[] ReplaceLot(WorldSnapshot world, ResourceLot replacement)
            => world.Lots.Select(l => l.Id.Equals(replacement.Id) ? replacement : l).ToArray();
        private static RepairPlan CancelPlan(WorldSnapshot world, RepairActivity activity, string reason)
        {
            var lot = world.GetLot(activity.MaterialLotId);
            if (lot.Reserved < activity.ReservedQuantity) throw new ArgumentException("Repair reservation lost");
            var cancelled = activity.Finish(RepairActivityStatus.Cancelled, reason);
            var proposedLot = new ResourceLot(lot.Id, lot.Commodity, lot.Owner, lot.Location, lot.OnHand,
                checked(lot.Reserved - activity.ReservedQuantity));
            return new RepairPlan(world.Repair!.With(newActivities: world.Repair.Activities.Select(a => a.Id.Equals(activity.Id) ? cancelled : a)),
                ReplaceLot(world, proposedLot), cancelled);
        }
    }
}
