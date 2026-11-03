using System;
using System.Collections.Generic;
using System.Linq;
using Game.Domain.Kernel;
using Game.Domain.Repair;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Foundation.Time;
using NUnit.Framework;
using F = MythicalDnd.Foundation.Tests.BoatRepairTestFixture;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class BoatRepairRuleTests
    {
        private static WorldSnapshot Rebuild(WorldSnapshot world, RepairState? repair = null,
            IEnumerable<ActorState>? actors = null, IEnumerable<ResourceLot>? lots = null,
            IdentitySnapshot? identity = null, WorldTick? tick = null, IEnumerable<RngStreamState>? rng = null)
            => new WorldSnapshot(identity ?? world.Identity, tick ?? world.Tick, actors ?? world.Actors,
                lots ?? world.Lots, rng ?? world.RngStates, repair ?? world.Repair);
        private static RepairState ReplaceItem(WorldSnapshot world, RepairItemState replacement)
            => new RepairState(world.Repair!.Recipe, world.Repair.Items.Select(i => i.Id.Equals(replacement.Id) ? replacement : i),
                world.Repair.Capabilities, world.Repair.Activities);
        private static RepairEvaluation Start(WorldSnapshot world, out IdentitySnapshot identity,
            EntityId<Actor>? actor = null, EntityId<Item>? boat = null, EntityId<Lot>? timber = null)
        {
            actor ??= F.Actor; boat ??= F.Boat; timber ??= F.Timber;
            var stage = world.Identity.BeginStage(checked(world.Revision.Value + 1));
            var activity = stage.Allocate<Activity>(); var job = stage.Allocate<Job>();
            var evaluation = BoatRepairRules.EvaluateStart(world, actor, actor, boat, timber, world.Repair!.Recipe.Id,
                activity, job, "domain.start", "domain.correlation");
            identity = stage.PrepareSnapshot(); return evaluation;
        }
        private static WorldSnapshot Running(WorldSnapshot? initial = null)
        {
            var world = initial ?? F.Initial().World;
            var evaluation = Start(world, out var identity);
            Assert.That(evaluation.ErrorCode, Is.Null);
            return Rebuild(world, evaluation.Plan!.Repair, lots: evaluation.Plan.Lots, identity: identity);
        }

        [TestCase("unqualified", "NOT_AUTHORIZED")]
        [TestCase("no_capability", "NOT_AUTHORIZED")]
        [TestCase("destroyed_tool", "ITEM_UNAVAILABLE")]
        [TestCase("broken_tool", "ITEM_UNAVAILABLE")]
        [TestCase("destroyed_boat", "ITEM_UNAVAILABLE")]
        public void VS_03A_PureStartChecksTrustedQualificationAndUsableItems(string variant, string error)
        {
            var world = F.Initial().World; var state = world.Repair!;
            if (variant == "unqualified" || variant == "no_capability")
                world = Rebuild(world, new RepairState(state.Recipe, state.Items,
                    variant == "no_capability" ? Array.Empty<RepairCapability>() : new[] { new RepairCapability(F.Actor, false, F.Tool) }));
            else
            {
                var item = state.GetItem(variant == "destroyed_boat" ? F.Boat : F.Tool);
                world = Rebuild(world, ReplaceItem(world, new RepairItemState(item.Id, item.Kind, item.Owner, item.Location,
                    variant == "broken_tool" ? 0 : item.Condition, variant != "broken_tool")));
            }
            var beforeLots = world.Lots.ToArray(); var beforeIdentity = world.Identity;
            var result = Start(world, out _);
            Assert.That(result.ErrorCode, Is.EqualTo(error)); Assert.That(result.Plan, Is.Null);
            Assert.That(result.Checks.Any(c => !c.Passed), Is.True);
            Assert.That(world.Identity, Is.SameAs(beforeIdentity)); Assert.That(world.Lots, Is.EqualTo(beforeLots));
            Assert.That(world.GetLot(F.Timber).Reserved, Is.Zero);
        }

        [TestCase("boat")]
        [TestCase("tool")]
        [TestCase("material")]
        public void VS_03A_PureStartRequiresControlOfEveryInput(string target)
        {
            var world = F.Initial().World; var stage = world.Identity.BeginStage(2); var otherOwner = stage.Allocate<Household>();
            if (target == "material")
            {
                var lot = world.GetLot(F.Timber);
                world = Rebuild(world, lots: world.Lots.Select(l => l.Id.Equals(F.Timber)
                    ? new ResourceLot(l.Id, l.Commodity, otherOwner.Value, l.Location, l.OnHand, l.Reserved) : l), identity: stage.PrepareSnapshot());
                Assert.That(lot.OnHand, Is.EqualTo(10));
            }
            else
            {
                var item = world.Repair!.GetItem(target == "boat" ? F.Boat : F.Tool);
                world = Rebuild(world, ReplaceItem(world, new RepairItemState(item.Id, item.Kind, otherOwner.Value, item.Location, item.Condition)),
                    identity: stage.PrepareSnapshot());
            }
            Assert.That(Start(world, out _).ErrorCode, Is.EqualTo("NOT_AUTHORIZED"));
            Assert.That(world.GetLot(F.Timber).Reserved, Is.Zero);
        }

        [TestCase("actor")]
        [TestCase("boat")]
        [TestCase("tool")]
        [TestCase("material")]
        public void VS_03A_PureStartRequiresAllInputsAtOneWorksite(string target)
        {
            var world = F.Initial().World; var stage = world.Identity.BeginStage(2); var remote = stage.Allocate<Settlement>();
            if (target == "actor")
                world = Rebuild(world, actors: world.Actors.Select(a => new ActorState(a.Id, a.Alive, remote, a.ControlledOwnerIds)), identity: stage.PrepareSnapshot());
            else if (target == "material")
                world = Rebuild(world, lots: world.Lots.Select(l => l.Id.Equals(F.Timber)
                    ? new ResourceLot(l.Id, l.Commodity, l.Owner, remote, l.OnHand, l.Reserved) : l), identity: stage.PrepareSnapshot());
            else
            {
                var item = world.Repair!.GetItem(target == "boat" ? F.Boat : F.Tool);
                world = Rebuild(world, ReplaceItem(world, new RepairItemState(item.Id, item.Kind, item.Owner, remote, item.Condition)), identity: stage.PrepareSnapshot());
            }
            Assert.That(Start(world, out _).ErrorCode, Is.EqualTo("LOCATION_MISMATCH"));
        }

        [Test]
        public void VS_03A_PureStartCannotShareAReservedToolWithAnotherActorAndBoat()
        {
            var world = F.Initial().World; var stage = world.Identity.BeginStage(2);
            var otherActor = stage.Allocate<Actor>(); var otherBoat = stage.Allocate<Item>(); var otherMaterial = stage.Allocate<Lot>();
            var worksite = world.GetActor(F.Actor).Location; var repair = world.Repair!;
            var extended = new RepairState(repair.Recipe,
                repair.Items.Concat(new[] { new RepairItemState(otherBoat, RepairItemKind.Boat, otherActor.Value, worksite, 500) }),
                repair.Capabilities.Concat(new[] { new RepairCapability(otherActor, true, F.Tool) }));
            world = Rebuild(world, extended,
                world.Actors.Concat(new[] { new ActorState(otherActor, true, worksite, new[] { F.Actor.Value }) }),
                world.Lots.Concat(new[] { new ResourceLot(otherMaterial, repair.Recipe.MaterialCommodity, otherActor.Value, worksite, 10, 0) }), stage.PrepareSnapshot());
            var running = Running(world);
            Assert.That(Start(running, out _, otherActor, otherBoat, otherMaterial).ErrorCode, Is.EqualTo("ITEM_UNAVAILABLE"));
            Assert.That(running.GetLot(otherMaterial).Reserved, Is.Zero);
            Assert.That(running.Repair!.Activities.Count, Is.EqualTo(1));
        }

        [TestCase("actor_dead", "ACTOR_DEAD")]
        [TestCase("unqualified", "NOT_AUTHORIZED")]
        [TestCase("destroyed_tool", "ITEM_UNAVAILABLE")]
        [TestCase("broken_tool", "ITEM_UNAVAILABLE")]
        [TestCase("destroyed_boat", "ITEM_UNAVAILABLE")]
        [TestCase("full_boat", "ITEM_UNAVAILABLE")]
        [TestCase("lost_rights", "NOT_AUTHORIZED")]
        [TestCase("actor_moved", "LOCATION_MISMATCH")]
        public void VS_03A_PureCompletionRevalidatesAndProposesCancellationWithoutRefundingWood(string variant, string reason)
        {
            var world = Running(); var state = world.Repair!; var identity = world.Identity;
            if (variant == "actor_dead")
                world = Rebuild(world, actors: world.Actors.Select(a => new ActorState(a.Id, false, a.Location, a.ControlledOwnerIds)));
            else if (variant == "unqualified")
                world = Rebuild(world, new RepairState(state.Recipe, state.Items, new[] { new RepairCapability(F.Actor, false, F.Tool) }, state.Activities));
            else if (variant == "actor_moved")
            {
                var stage = identity.BeginStage(3); var remote = stage.Allocate<Settlement>();
                world = Rebuild(world, actors: world.Actors.Select(a => new ActorState(a.Id, true, remote, a.ControlledOwnerIds)), identity: stage.PrepareSnapshot());
            }
            else
            {
                var item = state.GetItem(variant.Contains("tool") ? F.Tool : F.Boat); var owner = item.Owner;
                if (variant == "lost_rights")
                {
                    var stage = identity.BeginStage(3); owner = stage.Allocate<Household>().Value; identity = stage.PrepareSnapshot();
                }
                var condition = variant == "broken_tool" ? 0 : variant == "full_boat" ? 1000 : item.Condition;
                var destroyed = variant == "destroyed_tool" || variant == "destroyed_boat";
                world = Rebuild(world, ReplaceItem(world, new RepairItemState(item.Id, item.Kind, owner, item.Location, condition, destroyed)), identity: identity);
            }
            world = Rebuild(world, tick: new WorldTick(60));
            var evaluation = BoatRepairRules.EvaluateComplete(world, F.Activity);
            Assert.That(evaluation.ErrorCode, Is.Null);
            var plan = evaluation.Plan!;
            Assert.That(plan.Activity.Status, Is.EqualTo(RepairActivityStatus.Cancelled)); Assert.That(plan.Activity.TerminalReason, Is.EqualTo(reason));
            Assert.That(plan.Activity.ReservedQuantity, Is.Zero);
            Assert.That(plan.Lots.Single(l => l.Id.Equals(F.Timber)).Reserved, Is.Zero);
            Assert.That(plan.Lots.Single(l => l.Id.Equals(F.Timber)).OnHand, Is.EqualTo(10));
            Assert.That(plan.Repair.GetItem(F.Boat).Condition, Is.EqualTo(world.Repair!.GetItem(F.Boat).Condition));
            Assert.That(world.GetLot(F.Timber).Reserved, Is.EqualTo(2));
        }

        [Test]
        public void VS_03A_PureCancelPreservesReservationsFromOtherDomains()
        {
            var initial = F.Initial().World;
            initial = Rebuild(initial, lots: initial.Lots.Select(l => l.Id.Equals(F.Timber)
                ? new ResourceLot(l.Id, l.Commodity, l.Owner, l.Location, 10, 3) : l));
            var world = Running(initial); Assert.That(world.GetLot(F.Timber).Reserved, Is.EqualTo(5));
            var evaluation = BoatRepairRules.EvaluateCancel(world, F.Actor, F.Activity, "changed_mind");
            Assert.That(evaluation.ErrorCode, Is.Null);
            Assert.That(evaluation.Plan!.Lots.Single(l => l.Id.Equals(F.Timber)).Reserved, Is.EqualTo(3));
            Assert.That(evaluation.Plan.Lots.Single(l => l.Id.Equals(F.Timber)).OnHand, Is.EqualTo(10));
            Assert.That(world.GetLot(F.Timber).Reserved, Is.EqualTo(5));
        }

        [Test]
        public void VS_03A_RepairAggregateRejectsLostHoldOrOrphanReferenceBeforePublication()
        {
            var world = Running();
            Assert.Throws<ArgumentException>(() => Rebuild(world, lots: world.Lots.Select(l => l.Id.Equals(F.Timber)
                ? new ResourceLot(l.Id, l.Commodity, l.Owner, l.Location, l.OnHand, 0) : l)));
            var a = world.Repair!.GetActivity(F.Activity);
            var orphan = new RepairActivity(a.Id, a.ActorId, a.ItemId,
                new EntityId<Lot>(new EntityId(14, "lot", 999)), a.ToolItemId, a.Worksite, a.JobId,
                a.StartedTick, a.DueTick, a.Recipe, a.StartCommandId, a.CorrelationId, a.Status, a.ReservedQuantity);
            Assert.Throws<IdentityException>(() => Rebuild(world, new RepairState(world.Repair.Recipe, world.Repair.Items, world.Repair.Capabilities, new[] { orphan })));
            Assert.That(world.GetLot(F.Timber).OnHand, Is.EqualTo(10));
            Assert.That(world.GetLot(F.Timber).Reserved, Is.EqualTo(2));
        }

        [Test]
        public void VS_03A_RecipeCannotBeSilentlyChangedUnderARunningActivity()
        {
            var world = Running(); var old = world.Repair!.Recipe;
            var changed = new RepairRecipe(old.Id, old.MaterialCommodity, old.MaterialQuantity, old.DurationTicks, old.ConditionGain + 1, old.ConditionMax);
            Assert.That(changed.Fingerprint, Is.Not.EqualTo(old.Fingerprint));
            Assert.Throws<ArgumentException>(() => new RepairState(changed, world.Repair.Items, world.Repair.Capabilities, world.Repair.Activities));
            Assert.That(world.Repair.GetActivity(F.Activity).Recipe.Fingerprint, Is.EqualTo(old.Fingerprint));
        }

        [Test]
        public void VS_03A_PureEvaluationDoesNotAdvanceIdentityOrRngAndReturnsImmutableLists()
        {
            var initial = F.Initial().World;
            var stream = new RngStreamState("repair-domain-test", new RngState(new RngContext(new string('0', 64), "repair", "test", "first", F.Actor.Value, "main"), 7));
            var world = Rebuild(initial, rng: new[] { stream }); var beforeIdentity = world.Identity;
            var evaluation = Start(world, out _); Assert.That(evaluation.ErrorCode, Is.Null);
            Assert.That(world.Identity, Is.SameAs(beforeIdentity)); Assert.That(world.Identity.Cursor("activity").NextCounter, Is.EqualTo(1UL));
            Assert.That(world.RngStates.Single().State.DrawIndex, Is.EqualTo(7UL)); Assert.That(world.GetLot(F.Timber).Reserved, Is.Zero);
            Assert.Throws<NotSupportedException>(() => ((IList<ResourceLot>)evaluation.Plan!.Lots).Clear());
            Assert.Throws<NotSupportedException>(() => ((IList<RepairActivity>)evaluation.Plan!.Repair.Activities).Clear());
        }

        [Test]
        public void VS_03A_PureCompletionRejectsBeforeDueAndStartRejectsLegacyWorld()
        {
            var running = Running(); Assert.That(BoatRepairRules.EvaluateComplete(running, F.Activity).ErrorCode, Is.EqualTo("ACTIVITY_STATE"));
            var legacy = new WorldSnapshot(running.Identity, running.Tick, running.Actors, running.Lots, running.RngStates);
            var stage = legacy.Identity.BeginStage(3);
            var result = BoatRepairRules.EvaluateStart(legacy, F.Actor, F.Actor, F.Boat, F.Timber, running.Repair!.Recipe.Id,
                stage.Allocate<Activity>(), stage.Allocate<Job>(), "legacy.repair", "legacy.repair");
            Assert.That(result.ErrorCode, Is.EqualTo("COMMAND_UNKNOWN"));
            Assert.That(legacy.Repair, Is.Null);
        }
    }
}
