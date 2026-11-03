using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.Application.CommandPipeline;
using Game.Application.Events;
using Game.Application.SimulationScheduler;
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
    public sealed class BoatRepairAdversarialTests
    {
        private static EntityId<Actor> OtherActor => EntityId<Actor>.Parse("w000000000000000e/actor/0000000000000002");
        private static EntityId<Settlement> OtherSite => EntityId<Settlement>.Parse("w000000000000000e/settlement/0000000000000002");
        private static EntityId<Household> OtherOwner => EntityId<Household>.Parse("w000000000000000e/household/0000000000000001");
        private static EntityId<Item> OtherTool => EntityId<Item>.Parse("w000000000000000e/item/0000000000000003");
        private static EntityId<Item> OtherBoat => EntityId<Item>.Parse("w000000000000000e/item/0000000000000004");
        private static EntityId<Lot> Spare => EntityId<Lot>.Parse("w000000000000000e/lot/0000000000000002");
        private static CommandProcessor Processor(WorldSnapshot? world = null, Action? before = null)
        {
            var registry = F.Registry();
            return new CommandProcessor(world ?? RichWorld(), F.Rules(), beforeCommit: before, repairRecipe: registry.Recipe);
        }
        private static WorldSnapshot RichWorld(bool sharedControl = false)
        {
            var registry = F.Registry(); var source = F.Initial(registry).World;
            var ids = source.Identity.BeginStage(source.Revision.Value + 1);
            ids.Allocate<Actor>(); ids.Allocate<Settlement>(); ids.Allocate<Household>(); ids.Allocate<Item>(); ids.Allocate<Item>();
            var site = source.GetActor(F.Actor).Location;
            var actors = source.Actors.Concat(new[] { new ActorState(OtherActor, true, site, sharedControl ? new[] { F.Actor.Value } : null) });
            var items = source.Repair!.Items.Concat(new[] {
                new RepairItemState(OtherTool, RepairItemKind.RepairTool, F.Actor.Value, site, 1000),
                new RepairItemState(OtherBoat, RepairItemKind.Boat, F.Actor.Value, site, 400) });
            var capabilities = source.Repair.Capabilities.Concat(new[] { new RepairCapability(OtherActor, true, F.Tool) });
            var repair = new RepairState(registry.Recipe, items, capabilities);
            var rng = new RngStreamState("repair.fixture", new RngState(new RngContext(new string('0', 64), "repair", "fixture", "initial", F.Actor.Value, "resolution"), 17));
            return new WorldSnapshot(ids.PrepareSnapshot(), source.Tick, actors, source.Lots, new[] { rng }, repair);
        }
        private static CommandEnvelope Envelope(CommandProcessor p, string id, string type, Dictionary<string, object?> payload,
            EntityId<Actor>? actor = null, string source = "Player")
        {
            var world = p.InspectCommitted().World;
            return new CommandEnvelope(id, type, (actor ?? F.Actor).ToString(), source, world.Tick.Value, world.Revision.Value, 1, payload, "repair.adversarial");
        }
        private static CommandReceipt Start(CommandProcessor p, string id = "start", EntityId<Actor>? actor = null, EntityId<Item>? boat = null)
        {
            var owner = actor ?? F.Actor;
            return p.Execute(Envelope(p, id, "StartRepair", new Dictionary<string, object?> {
                ["actor_id"] = owner.ToString(), ["item_id"] = (boat ?? F.Boat).ToString(),
                ["material_lot_id"] = F.Timber.ToString(), ["recipe_id"] = F.Registry().Recipe.Id.ToString() }, owner),
                new ExecutionPrincipal(owner, CommandSource.Player));
        }
        private static void Advance(CommandProcessor p, int budget = CommandProcessor.MaximumJobsPerInvocation)
        {
            p.Execute(Envelope(p, "advance", "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = 60L }), F.Principal);
            p.DrainAdvance(F.Principal, budget);
        }
        // Trusted recomposition preserves original receipts/events. It tests invalidation admission,
        // not implementation of Health/Travel/rights mutation commands.
        private static CommittedWorld Recompose(CommittedWorld source, WorldSnapshot world, SchedulerState? scheduler = null)
            => (CommittedWorld)Activator.CreateInstance(typeof(CommittedWorld), BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null, args: new object?[] { world, source.AcceptedReceipts, source.MetadataReceipts, source.Events,
                    scheduler ?? source.Scheduler, source.Journal.Events }, culture: null)!;
        private static CommandProcessor Restore(CommittedWorld source, WorldSnapshot world, SchedulerState? scheduler = null)
            => new CommandProcessor(Recompose(source, world, scheduler), F.Rules(), repairRecipe: world.Repair!.Recipe);

        [Test]
        public void VS_03A_ConfiguredRecipeMustMatchExactProfileFingerprint()
        {
            var initial = F.Initial(); var r = initial.World.Repair!.Recipe;
            Assert.Throws<ArgumentException>(() => new CommandProcessor(initial, F.Rules()));
            var changed = new RepairRecipe(r.Id, r.MaterialCommodity, r.MaterialQuantity, r.DurationTicks + 1, r.ConditionGain, r.ConditionMax);
            Assert.Throws<ArgumentException>(() => new CommandProcessor(initial, F.Rules(), repairRecipe: changed));
            var identical = new RepairRecipe(r.Id, r.MaterialCommodity, r.MaterialQuantity, r.DurationTicks, r.ConditionGain, r.ConditionMax);
            Assert.DoesNotThrow(() => new CommandProcessor(initial, F.Rules(), repairRecipe: identical));
        }
        [TestCase("recipe", "REF_NOT_FOUND")]
        [TestCase("actor", "NOT_AUTHORIZED")]
        [TestCase("source", "NOT_AUTHORIZED")]
        public void VS_03A_RejectedStartCannotSpendIdsMaterialOrRng(string mode, string expected)
        {
            var p = Processor(); var before = p.InspectCommitted();
            var payload = F.Start().Payload.ToDictionary(x => x.Key, x => x.Value);
            if (mode == "recipe") payload["recipe_id"] = "recipe.unapproved@1";
            if (mode == "actor") payload["actor_id"] = OtherActor.ToString();
            var command = Envelope(p, "bad.start", "StartRepair", payload, source: mode == "source" ? "Scheduler" : "Player");
            Assert.That(p.Execute(command, F.Principal).ErrorCode, Is.EqualTo(expected));
            Assert.That(p.InspectCommitted().World, Is.SameAs(before.World));
            Assert.That(p.InspectCommitted().World.Identity.Cursor("activity").NextCounter, Is.EqualTo(1));
            Assert.That(p.InspectCommitted().World.Identity.Cursor("job").NextCounter, Is.EqualTo(1));
            Assert.That(p.InspectCommitted().World.RngStates.Single().State.DrawIndex, Is.EqualTo(17));
            Assert.That(p.InspectCommitted().World.GetLot(F.Timber).Reserved, Is.Zero);
        }
        [Test]
        public void VS_03A_ActorAndSharedToolCannotBeDoubleBookedByNewCommandIds()
        {
            var p = Processor(RichWorld(sharedControl: true)); Start(p);
            var before = p.InspectCommitted().World;
            Assert.That(Start(p, "actor.busy", boat: OtherBoat).ErrorCode, Is.EqualTo("ITEM_UNAVAILABLE"));
            Assert.That(Start(p, "shared.tool", OtherActor, OtherBoat).ErrorCode, Is.EqualTo("ITEM_UNAVAILABLE"));
            Assert.That(p.InspectCommitted().World, Is.SameAs(before));
            Assert.That(p.InspectCommitted().World.Repair!.Activities.Count, Is.EqualTo(1));
            Assert.That(p.InspectCommitted().Scheduler.Jobs.Count, Is.EqualTo(1));
            Assert.That(before.GetLot(F.Timber).Reserved, Is.EqualTo(2));
        }
        [Test]
        public void VS_03A_OtherPrincipalCannotCancelActivityOrProbeRepairJobAsOwner()
        {
            var p = Processor(); Start(p); var before = p.InspectCommitted().World;
            var principal = new ExecutionPrincipal(OtherActor, CommandSource.Player);
            var activity = Envelope(p, "other.cancel", "CancelActivity", new Dictionary<string, object?> {
                ["activity_id"] = F.Activity.ToString(), ["reason"] = "no_permission" }, OtherActor);
            var job = Envelope(p, "other.job", "CancelJob", new Dictionary<string, object?> {
                ["job_id"] = p.InspectCommitted().Scheduler.Jobs.Single().Id.ToString() }, OtherActor);
            Assert.That(p.Execute(activity, principal).ErrorCode, Is.EqualTo("NOT_AUTHORIZED"));
            Assert.That(p.Execute(job, principal).ErrorCode, Is.EqualTo("NOT_AUTHORIZED"));
            Assert.That(p.InspectCommitted().World, Is.SameAs(before));
        }
        [Test]
        public void VS_03A_CancelMayUseFreshCommandCorrelationWhileKeepingStartPins()
        {
            var p = Processor(); Start(p); var started = p.InspectCommitted(); var activity = started.World.Repair!.GetActivity(F.Activity);
            var command = new CommandEnvelope("cancel.fresh-correlation", "CancelActivity", F.Actor.ToString(), "Player",
                started.World.Tick.Value, started.World.Revision.Value, 1,
                new Dictionary<string, object?> { ["activity_id"] = F.Activity.ToString(), ["reason"] = "new_decision" }, "independent.cancel.flow");
            Assert.That(p.Execute(command, F.Principal).Outcome, Is.EqualTo(CommandOutcome.Accepted));
            var after = p.InspectCommitted();
            Assert.That(after.World.Repair!.GetActivity(F.Activity).CorrelationId, Is.EqualTo(activity.CorrelationId));
            Assert.That(after.Journal.Events.Where(e => e.CausationId == command.CommandId).All(e => e.CorrelationId == command.CorrelationId), Is.True);
            Assert.That(after.World.GetLot(F.Timber).OnHand, Is.EqualTo(10)); Assert.That(after.World.GetLot(F.Timber).Reserved, Is.Zero);
        }
        [Test]
        public void VS_03A_PausedMixedJobAdvanceCanCancelRepairWithoutRestoringConsumedWood()
        {
            var p = Processor();
            var transfer = Envelope(p, "schedule.transfer", "ScheduleTransfer", new Dictionary<string, object?> {
                ["due_tick"] = 60L, ["phase"] = 2L, ["priority"] = 0L,
                ["source_lot_id"] = F.Timber.ToString(), ["destination_lot_id"] = Spare.ToString(), ["quantity"] = 8L });
            Assert.That(p.Execute(transfer, F.Principal).Outcome, Is.EqualTo(CommandOutcome.Accepted)); Start(p);
            Advance(p, 1); var paused = p.InspectCommitted();
            Assert.That(paused.Scheduler.Workflow!.Status, Is.EqualTo(AdvanceStatus.PausedBudget));
            Assert.That(paused.World.GetLot(F.Timber).OnHand, Is.EqualTo(2));
            Assert.That(paused.World.GetLot(F.Timber).Reserved, Is.EqualTo(2));
            var cancel = Envelope(p, "cancel.paused", "CancelActivity", new Dictionary<string, object?> {
                ["activity_id"] = F.Activity.ToString(), ["reason"] = "changed_plan" });
            Assert.That(p.Execute(cancel, F.Principal).Outcome, Is.EqualTo(CommandOutcome.Accepted));
            p.DrainAdvance(F.Principal); var after = p.InspectCommitted();
            Assert.That(after.World.GetLot(F.Timber).OnHand, Is.EqualTo(2));
            Assert.That(after.World.GetLot(F.Timber).Reserved, Is.Zero);
            Assert.That(after.World.GetLot(Spare).OnHand, Is.EqualTo(8));
            Assert.That(after.World.Repair!.GetItem(F.Boat).Condition, Is.EqualTo(500));
            Assert.That(after.Journal.Events.Any(e => e.EventType == "RepairCompleted"), Is.False);
        }
        [TestCase("dead", "ACTOR_DEAD")]
        [TestCase("qualification", "NOT_AUTHORIZED")]
        [TestCase("boat_owner", "NOT_AUTHORIZED")]
        [TestCase("tool_destroyed", "ITEM_UNAVAILABLE")]
        [TestCase("boat_destroyed", "ITEM_UNAVAILABLE")]
        [TestCase("tool_changed", "ITEM_UNAVAILABLE")]
        [TestCase("actor_location", "LOCATION_MISMATCH")]
        [TestCase("tool_location", "LOCATION_MISMATCH")]
        [TestCase("boat_full", "ITEM_UNAVAILABLE")]
        public void VS_03A_DueRevalidationCancelsWithoutRepairOrCreatingMaterial(string mode, string reason)
        {
            var p = Processor(); Start(p);
            // Admit the workflow while its issuer is alive, then invalidate the running activity.
            // A dead issuer cannot initiate AdvanceTime; this case tests due-time revalidation.
            Assert.That(p.Execute(Envelope(p, "advance", "AdvanceTime", new Dictionary<string, object?> {
                ["target_tick"] = 60L }), F.Principal).Outcome, Is.EqualTo(CommandOutcome.AcceptedPending));
            var started = p.InspectCommitted(); var w = started.World; var repair = w.Repair!;
            var actors = w.Actors.Select(a => a.Id.Equals(F.Actor)
                ? new ActorState(a.Id, mode != "dead", mode == "actor_location" ? OtherSite : a.Location, a.ControlledOwnerIds) : a);
            var items = repair.Items.Select(i => new RepairItemState(i.Id, i.Kind,
                i.Id.Equals(F.Boat) && mode == "boat_owner" ? OtherOwner.Value : i.Owner,
                i.Id.Equals(F.Tool) && mode == "tool_location" ? OtherSite : i.Location,
                i.Id.Equals(F.Boat) && mode == "boat_full" ? 1000 : i.Condition,
                i.Destroyed || (i.Id.Equals(F.Tool) && mode == "tool_destroyed") || (i.Id.Equals(F.Boat) && mode == "boat_destroyed")));
            var capabilities = repair.Capabilities.Select(c => c.ActorId.Equals(F.Actor)
                ? new RepairCapability(c.ActorId, mode != "qualification", mode == "tool_changed" ? OtherTool : c.ToolItemId) : c);
            var changed = new WorldSnapshot(w.Identity, w.Tick, actors, w.Lots, w.RngStates,
                new RepairState(repair.Recipe, items, capabilities, repair.Activities));
            var restored = Restore(started, changed); restored.DrainAdvance(F.Principal); var after = restored.InspectCommitted();
            Assert.That(after.World.Repair!.GetActivity(F.Activity).Status, Is.EqualTo(RepairActivityStatus.Cancelled));
            Assert.That(after.World.Repair.GetActivity(F.Activity).TerminalReason, Is.EqualTo(reason));
            Assert.That(after.World.GetLot(F.Timber).OnHand, Is.EqualTo(10)); Assert.That(after.World.GetLot(F.Timber).Reserved, Is.Zero);
            Assert.That(after.World.Repair.GetItem(F.Boat).Condition, Is.EqualTo(mode == "boat_full" ? 1000 : 500));
            Assert.That(after.Journal.Events.Any(e => e.EventType == "RepairCompleted"), Is.False);
            Assert.That(after.Journal.Events.Count(e => e.EventType == "ActivityCancelled"), Is.EqualTo(1));
            Assert.That(after.World.RngStates.Single().State.DrawIndex, Is.EqualTo(17));
        }
        [Test]
        public void VS_03A_CompletionFailureDoesNotPublishDueTickItemOrHoldAndCanRetry()
        {
            var fail = false; var p = Processor(before: () => { if (fail) throw new InvalidOperationException("precommit"); });
            Start(p); p.Execute(Envelope(p, "advance", "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = 60L }), F.Principal);
            var before = p.InspectCommitted(); fail = true;
            Assert.Throws<InvalidOperationException>(() => p.DrainAdvance(F.Principal));
            Assert.That(p.InspectCommitted(), Is.SameAs(before)); Assert.That(before.World.Tick.Value, Is.Zero);
            Assert.That(before.World.GetLot(F.Timber).Reserved, Is.EqualTo(2)); fail = false;
            p.DrainAdvance(F.Principal); var after = p.InspectCommitted();
            Assert.That(after.World.Repair!.GetItem(F.Boat).Condition, Is.EqualTo(700));
            Assert.That(after.World.GetLot(F.Timber).OnHand, Is.EqualTo(8));
            Assert.That(after.World.RngStates.Single().State.DrawIndex, Is.EqualTo(17));
        }
        [Test]
        public void VS_03A_SequenceExhaustionCannotAllocateOrReserve()
        {
            var r = F.Registry(); var w = RichWorld();
            var p = new CommandProcessor(w, F.Rules(), initialScheduler: SchedulerState.Restore(Array.Empty<ScheduledJob>(), ulong.MaxValue, true), repairRecipe: r.Recipe);
            Assert.That(Start(p).ErrorCode, Is.EqualTo("SEQUENCE_EXHAUSTED"));
            Assert.That(p.InspectCommitted().World, Is.SameAs(w));
        }
        [Test]
        public void VS_03A_RestoredJobCannotChangePinnedStartCausation()
        {
            var p = Processor(); Start(p); var state = p.InspectCommitted(); var job = state.Scheduler.Jobs.Single();
            var changed = ScheduledJob.ForRepair(job.Id, job.ActorId, job.DueTick, job.CreationSequence, job.RepairActivityId!, "different.start", job.CorrelationId);
            var scheduler = SchedulerState.Restore(new[] { changed }, state.Scheduler.NextCreationSequence);
            Assert.Throws<ArgumentException>(() => Restore(state, state.World, scheduler));
        }

        private static RepairPlan ForgePlan(RepairPlan source, RepairState repair, RepairActivity activity)
            => (RepairPlan)Activator.CreateInstance(typeof(RepairPlan), BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null, args: new object?[] { repair, source.Lots, activity }, culture: null)!;
        private static void ValidateProposal(WorldSnapshot world, RepairPlan plan)
            => typeof(CommandProcessor).GetMethod("ValidateRepairPlan", BindingFlags.Static | BindingFlags.NonPublic)!
                .Invoke(null, new object?[] { world, plan, F.Activity, RepairActivityStatus.Completed });
        private static RepairActivity ChangeActivity(RepairActivity a, string mode)
            => new RepairActivity(a.Id, mode == "actor" ? OtherActor : a.ActorId, mode == "boat" ? OtherBoat : a.ItemId,
                mode == "material" ? Spare : a.MaterialLotId, mode == "tool" ? OtherTool : a.ToolItemId,
                mode == "worksite" ? OtherSite : a.Worksite, a.JobId,
                mode == "time" ? new WorldTick(1) : a.StartedTick, mode == "time" ? new WorldTick(61) : a.DueTick,
                a.Recipe, mode == "cause" ? "different.start" : a.StartCommandId,
                mode == "correlation" ? "different.correlation" : a.CorrelationId, a.Status, a.ReservedQuantity, a.TerminalReason);
        [TestCase("gain")]
        [TestCase("tool_condition")]
        [TestCase("owner")]
        [TestCase("location")]
        [TestCase("destroyed")]
        [TestCase("actor")]
        [TestCase("boat")]
        [TestCase("material")]
        [TestCase("tool")]
        [TestCase("worksite")]
        [TestCase("time")]
        [TestCase("cause")]
        [TestCase("correlation")]
        public void VS_03A_IndependentOwnerGuardRejectsUnrelatedItemChangesAndRetargetedActivity(string mode)
        {
            var p = Processor(); Start(p); var state = p.InspectCommitted(); var w = state.World;
            var atDue = new WorldSnapshot(w.Identity, new WorldTick(60), w.Actors, w.Lots, w.RngStates, w.Repair);
            var plan = BoatRepairRules.EvaluateComplete(atDue, F.Activity).Plan!;
            Assert.DoesNotThrow(() => ValidateProposal(atDue, plan));
            var changedItems = plan.Repair.Items.Select(i => {
                if (i.Id.Equals(F.Boat)) return new RepairItemState(i.Id, i.Kind,
                    mode == "owner" ? OtherOwner.Value : i.Owner, mode == "location" ? OtherSite : i.Location,
                    mode == "gain" ? i.Condition + 1 : i.Condition, i.Destroyed || mode == "destroyed");
                if (i.Id.Equals(F.Tool) && mode == "tool_condition") return new RepairItemState(i.Id, i.Kind, i.Owner, i.Location, i.Condition - 1, i.Destroyed);
                return i;
            });
            var activity = ChangeActivity(plan.Activity, mode);
            var repair = new RepairState(plan.Repair.Recipe, changedItems, plan.Repair.Capabilities,
                plan.Repair.Activities.Select(a => a.Id.Equals(activity.Id) ? activity : a));
            var forged = ForgePlan(plan, repair, activity);
            var error = Assert.Throws<TargetInvocationException>(() => ValidateProposal(atDue, forged));
            Assert.That(error!.InnerException, Is.TypeOf<PipelineFailureException>());
            Assert.That(((PipelineFailureException)error.InnerException!).Code, Does.StartWith("PROPOSAL_REPAIR_"));
        }
    }
}
