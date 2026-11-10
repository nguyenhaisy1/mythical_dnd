using System;
using System.Collections.Generic;
using System.Linq;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Time;

namespace Game.Domain.Weaving
{
    public sealed class WeavingPlan
    {
        public WeavingState Weaving { get; }
        public IReadOnlyList<ResourceLot> Lots { get; }
        public WeavingActivity Activity { get; }
        internal WeavingPlan(WeavingState weaving, IEnumerable<ResourceLot> lots, WeavingActivity activity)
        { Weaving = weaving; Lots = Array.AsReadOnly(lots.ToArray()); Activity = activity; }
    }
    public sealed class WeavingEvaluation
    {
        public string? ErrorCode { get; }
        public WeavingPlan? Plan { get; }
        internal WeavingEvaluation(string? error, WeavingPlan? plan = null) { ErrorCode = error; Plan = plan; }
    }
    public static class WeavingRules
    {
        public static string? Eligibility(WorldSnapshot world)
        {
            var state = world.Weaving; if (state == null) return "COMMAND_UNKNOWN";
            var actor = world.GetActor(state.ActorId); var input = world.GetLot(state.InputLotId); var output = world.GetLot(state.OutputLotId);
            if (!actor.Alive) return "ACTOR_DEAD";
            if (state.SkillValue < state.Recipe.MinimumSkill || !actor.Controls(input.Owner) || !actor.Controls(output.Owner)) return "NOT_AUTHORIZED";
            if (!actor.Location.Equals(state.Worksite) || !input.Location.Equals(state.Worksite) || !output.Location.Equals(state.Worksite)) return "LOCATION_MISMATCH";
            if (!input.Commodity.Equals(state.Recipe.MaterialCommodity) || !output.Commodity.Equals(state.Recipe.OutputCommodity)) return "COMMODITY_MISMATCH";
            try { _ = checked(output.OnHand + state.Recipe.CordYield); }
            catch (OverflowException) { return "NUMERIC_OVERFLOW"; }
            return null;
        }
        public static WeavingEvaluation EvaluateStart(WorldSnapshot world, EntityId<Actor> issuer, EntityId<Actor> actorId,
            EntityId<Lot> inputLotId, EntityId<Lot> outputLotId, EntityId<Activity> activityId, EntityId<Job> jobId,
            string commandId, string correlationId)
        {
            if (world == null || issuer == null || actorId == null || inputLotId == null || outputLotId == null || activityId == null || jobId == null) throw new ArgumentNullException();
            var state = world.Weaving; if (state == null) return new WeavingEvaluation("COMMAND_UNKNOWN");
            if (!issuer.Equals(actorId) || !actorId.Equals(state.ActorId)) return new WeavingEvaluation("NOT_AUTHORIZED");
            if (!inputLotId.Equals(state.InputLotId) || !outputLotId.Equals(state.OutputLotId) || activityId.Value.World != world.WorldId || jobId.Value.World != world.WorldId)
                return new WeavingEvaluation("REF_NOT_FOUND");
            var error = Eligibility(world); if (error != null) return new WeavingEvaluation(error);
            if (state.Activities.Any(a => a.Status == WeavingActivityStatus.Running)) return new WeavingEvaluation("ITEM_UNAVAILABLE");
            var input = world.GetLot(inputLotId); if (input.Available < state.Recipe.FiberCost) return new WeavingEvaluation("INSUFFICIENT_RESOURCE");
            try
            {
                var due = world.Tick.Add(state.Recipe.DurationTicks);
                var activity = new WeavingActivity(activityId, actorId, jobId, world.Tick, due, commandId, correlationId, WeavingActivityStatus.Running, state.Recipe.FiberCost);
                var lot = new ResourceLot(input.Id, input.Commodity, input.Owner, input.Location, input.OnHand, checked(input.Reserved + state.Recipe.FiberCost));
                return new WeavingEvaluation(null, new WeavingPlan(state.With(state.Activities.Concat(new[] { activity })),
                    world.Lots.Select(l => l.Id.Equals(lot.Id) ? lot : l), activity));
            }
            catch (TimeRangeException) { return new WeavingEvaluation("TIME_RANGE"); }
            catch (OverflowException) { return new WeavingEvaluation("NUMERIC_OVERFLOW"); }
        }
        public static WeavingEvaluation EvaluateCancel(WorldSnapshot world, EntityId<Actor> issuer, EntityId<Activity> activityId, string reason)
        {
            if (world == null || issuer == null || activityId == null) throw new ArgumentNullException();
            var state = world.Weaving; if (state == null) return new WeavingEvaluation("COMMAND_UNKNOWN");
            if (!issuer.Equals(state.ActorId)) return new WeavingEvaluation("NOT_AUTHORIZED");
            if (string.IsNullOrWhiteSpace(reason) || reason.Length > 128 || reason.Any(char.IsControl)) return new WeavingEvaluation("INVALID_PAYLOAD");
            try
            {
                var actor = world.GetActor(issuer); var activity = state.GetActivity(activityId);
                if (!actor.Alive) return new WeavingEvaluation("ACTOR_DEAD");
                if (!actor.Controls(activity.ActorId.Value)) return new WeavingEvaluation("NOT_AUTHORIZED");
                if (activity.Status != WeavingActivityStatus.Running) return new WeavingEvaluation("ACTIVITY_STATE");
                return new WeavingEvaluation(null, TerminalPlan(world, activity, WeavingActivityStatus.Cancelled, reason));
            }
            catch (IdentityException e) { return new WeavingEvaluation(e.Code); }
        }
        public static WeavingEvaluation EvaluateComplete(WorldSnapshot world, EntityId<Activity> activityId)
        {
            if (world == null || activityId == null) throw new ArgumentNullException();
            var state = world.Weaving; if (state == null) return new WeavingEvaluation("COMMAND_UNKNOWN");
            WeavingActivity activity;
            try { activity = state.GetActivity(activityId); } catch (IdentityException e) { return new WeavingEvaluation(e.Code); }
            if (activity.Status != WeavingActivityStatus.Running || world.Tick < activity.DueTick) return new WeavingEvaluation("ACTIVITY_STATE");
            var input = world.GetLot(state.InputLotId);
            if (input.Reserved < activity.ReservedQuantity || input.OnHand < activity.ReservedQuantity) return new WeavingEvaluation("RESERVATION_LOST");
            var error = Eligibility(world);
            return new WeavingEvaluation(null, TerminalPlan(world, activity, error == null ? WeavingActivityStatus.Completed : WeavingActivityStatus.Cancelled, error));
        }
        private static WeavingPlan TerminalPlan(WorldSnapshot world, WeavingActivity activity, WeavingActivityStatus status, string? reason)
        {
            var state = world.Weaving!; var input = world.GetLot(state.InputLotId); var output = world.GetLot(state.OutputLotId);
            if (input.Reserved < activity.ReservedQuantity) throw new ArgumentException("Weaving reservation lost");
            var finished = activity.Finish(status, reason); var complete = status == WeavingActivityStatus.Completed;
            var nextInput = new ResourceLot(input.Id, input.Commodity, input.Owner, input.Location,
                complete ? checked(input.OnHand - activity.ReservedQuantity) : input.OnHand, checked(input.Reserved - activity.ReservedQuantity));
            var nextOutput = new ResourceLot(output.Id, output.Commodity, output.Owner, output.Location,
                complete ? checked(output.OnHand + state.Recipe.CordYield) : output.OnHand, output.Reserved);
            return new WeavingPlan(state.With(state.Activities.Select(a => a.Id.Equals(finished.Id) ? finished : a)),
                world.Lots.Select(l => l.Id.Equals(nextInput.Id) ? nextInput : l.Id.Equals(nextOutput.Id) ? nextOutput : l), finished);
        }
    }
}
