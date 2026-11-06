using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Game.Application.CommandPipeline;
using Game.Application.Projection;
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
    public sealed class RepairClientSessionTests
    {
        // These are session/port tests. Real SQLite callbacks are verified by the Infrastructure/Player gates.
        private sealed class Harness
        {
            internal CommandProcessor Processor { get; }
            internal RepairClientSession Session { get; }
            internal CommittedWorld Saved { get; private set; }
            internal int Snapshots { get; private set; }
            internal bool FailSave, FailLoad, FailRestore;
            internal Harness(RepairObservationFields fields = RepairObservationFields.All, CommittedWorld? initial = null,
                Action? beforeCommit = null, Action? afterCommit = null)
            {
                var registry = F.Registry(); Saved = initial ?? F.Initial(registry);
                Processor = new CommandProcessor(Saved, F.Rules(), beforeCommit: beforeCommit, afterCommit: afterCommit, repairRecipe: registry.Recipe);
                var scope = new RepairObservationScope(F.Principal, F.Boat, F.Timber, F.Tool,
                    Saved.World.GetActor(F.Actor).Location, "Nhân vật thử nghiệm", "Bến Vạc", fields);
                Session = new RepairClientSession(Processor, scope,
                    state => { if (FailRestore) throw new InvalidOperationException("secret restore path/private-actor"); return new CommandProcessor(state, F.Rules(), repairRecipe: registry.Recipe); },
                    () => { if (FailLoad) throw new InvalidOperationException("secret database/password"); return Saved; },
                    state => { if (FailSave) throw new InvalidOperationException("secret SQL/password"); Saved = state; Snapshots++; });
            }
            internal RepairClientResult Start(string id = "repair")
            { var preview = Session.PreviewRepair(); Assert.That(preview.CanConfirm, Is.True); return Session.ConfirmRepair(preview.Token!, id); }
        }
        private static string Json(object value) => JsonSerializer.Serialize(value);

        private static CommandProcessor RunningRepairOnOtherBoat()
        {
            var source = F.Initial(); var world = source.World; var repair = world.Repair!;
            var ids = world.Identity.BeginStage(world.Revision.Value + 1); var otherBoat = ids.Allocate<Item>();
            var site = world.GetActor(F.Actor).Location;
            var changed = new WorldSnapshot(ids.PrepareSnapshot(), world.Tick, world.Actors, world.Lots,
                world.RngStates, new RepairState(repair.Recipe, repair.Items.Concat(new[] {
                    new RepairItemState(otherBoat, RepairItemKind.Boat, F.Actor.Value, site, 400)
                }), repair.Capabilities));
            var processor = new CommandProcessor(changed, F.Rules(), repairRecipe: repair.Recipe);
            var command = new CommandEnvelope("client/outside", "StartRepair", F.Actor.ToString(), "Player",
                world.Tick.Value, changed.Revision.Value, 1, new Dictionary<string, object?> {
                    ["actor_id"] = F.Actor.ToString(), ["item_id"] = otherBoat.ToString(),
                    ["material_lot_id"] = F.Timber.ToString(), ["recipe_id"] = repair.Recipe.Id.ToString()
                }, "repair-client");
            Assert.That(processor.Execute(command, F.Principal).Outcome, Is.EqualTo(CommandOutcome.Accepted));
            return processor;
        }

        [Test]
        public void S15_01_AllGrantsExposeOnlyBoundedKnownWorksiteFields()
        {
            var h = new Harness(); var view = h.Session.Read(); var preview = h.Session.PreviewRepair();
            Assert.That(view.ProfileId, Is.EqualTo("repair-client-2d-v1")); Assert.That(view.ArtProfileId, Is.EqualTo("earth-monument-2d-v1"));
            Assert.That(view.BoatVisible, Is.True); Assert.That(view.BoatCondition, Is.EqualTo(500));
            Assert.That(view.TimberOnHand, Is.EqualTo(10)); Assert.That(view.TimberReserved, Is.Zero); Assert.That(view.TimberAvailable, Is.EqualTo(10));
            Assert.That(view.CanRepair, Is.True); Assert.That(view.ActivityStatus, Is.EqualTo("none")); Assert.That(view.CanCancel, Is.False);
            Assert.That(view.Observation.Source, Is.EqualTo("direct-worksite-fixture")); Assert.That(view.Observation.ObservedTick, Is.Zero);
            Assert.That(preview.CostTimber, Is.EqualTo(2)); Assert.That(preview.DurationTicks, Is.EqualTo(60));
            Assert.That(preview.ConditionGain, Is.EqualTo(200)); Assert.That(preview.ConditionCap, Is.EqualTo(1000));
            Assert.That(Json(view), Does.Not.Contain("OriginalEnvelope").And.Not.Contain("Revision").And.Not.Contain("scheduler/").And.Not.Contain(F.Actor.ToString()));
        }
        [Test]
        public void S15_01_NoGrantDoesNotBecomeKnowledgeThroughOwnership()
        {
            var h = new Harness(RepairObservationFields.None); var view = h.Session.Read(); var preview = h.Session.PreviewRepair();
            Assert.That(view.BoatVisible, Is.False); Assert.That(view.BoatCondition, Is.Null);
            Assert.That(view.TimberOnHand, Is.Null); Assert.That(view.TimberReserved, Is.Null); Assert.That(view.TimberAvailable, Is.Null);
            Assert.That(view.Qualified, Is.Null); Assert.That(view.ToolUsable, Is.Null); Assert.That(view.ActorAlive, Is.Null);
            Assert.That(view.ActivityStatus, Is.Null); Assert.That(view.ActivityDueTick, Is.Null); Assert.That(view.CanRepair, Is.Null);
            Assert.That(view.Observation.Source, Is.EqualTo("unknown")); Assert.That(preview.Token, Is.Null);
            Assert.That(preview.Code, Is.EqualTo("not_observed")); Assert.That(preview.RequirementsKnown, Is.False);
            Assert.That(preview.CostTimber, Is.Null); Assert.That(preview.ConditionGain, Is.Null);
        }
        [TestCase(RepairObservationFields.MaterialOnHand)]
        [TestCase(RepairObservationFields.MaterialReserved)]
        public void S15_01_DerivedAvailableRequiresBothQuantityGrants(RepairObservationFields only)
        {
            var view = new Harness(only).Session.Read(); Assert.That(view.TimberAvailable, Is.Null);
            Assert.That(only == RepairObservationFields.MaterialOnHand ? view.TimberOnHand : view.TimberReserved, Is.Not.Null);
        }
        [Test]
        public void S15_01_BoatConditionDoesNotMakeAnUngrantedBoatPickable()
        {
            var h = new Harness(RepairObservationFields.BoatCondition); var view = h.Session.Read();
            Assert.That(view.BoatVisible, Is.False); Assert.That(view.BoatCondition, Is.Null); Assert.That(h.Session.PreviewRepair().CanConfirm, Is.False);
        }
        [Test]
        public void S15_01_UngrantedConditionCannotInfluencePreviewOrRead()
        {
            var fields = RepairObservationFields.All & ~RepairObservationFields.BoatCondition;
            var a = new Harness(fields, F.Initial(condition: 100)); var b = new Harness(fields, F.Initial(condition: 1000));
            Assert.That(Json(a.Session.Read()), Is.EqualTo(Json(b.Session.Read())));
            Assert.That(Json(a.Session.PreviewRepair()), Is.EqualTo(Json(b.Session.PreviewRepair())));
        }
        [Test]
        public void S15_01_UngrantedMaterialCannotInfluencePreviewOrRead()
        {
            var fields = RepairObservationFields.All & ~(RepairObservationFields.MaterialOnHand | RepairObservationFields.MaterialReserved);
            var a = new Harness(fields, F.Initial(timber: 1)); var b = new Harness(fields, F.Initial(timber: 999));
            Assert.That(Json(a.Session.Read()), Is.EqualTo(Json(b.Session.Read())));
            Assert.That(Json(a.Session.PreviewRepair()), Is.EqualTo(Json(b.Session.PreviewRepair())));
        }
        [Test]
        public void S15_01_PrivateActorsInventoriesBoatsAndRngDoNotLeakIntoProjection()
        {
            Harness WithSecrets(int hiddenCondition, long hiddenTimber, bool hiddenAlive)
            {
                var source = F.Initial(); var w = source.World; var ids = w.Identity.BeginStage(2);
                var other = ids.Allocate<Actor>(); var lot = ids.Allocate<Lot>(); var boat = ids.Allocate<Item>();
                var site = w.GetActor(F.Actor).Location; var recipe = w.Repair!.Recipe;
                var actors = w.Actors.Concat(new[] { new ActorState(other, hiddenAlive, site) });
                var lots = w.Lots.Concat(new[] { new ResourceLot(lot, recipe.MaterialCommodity, other.Value, site, hiddenTimber, 0) });
                var items = w.Repair.Items.Concat(new[] { new RepairItemState(boat, RepairItemKind.Boat, other.Value, site, hiddenCondition) });
                var rng = new RngStreamState("hidden.private", new RngState(new RngContext(new string('0',64), "repair", "private", "initial", other.Value, "resolution"), (ulong)hiddenTimber));
                var changed = new WorldSnapshot(ids.PrepareSnapshot(), w.Tick, actors, lots, new[] { rng },
                    new RepairState(recipe, items, w.Repair.Capabilities));
                return new Harness(initial: new CommandProcessor(changed, F.Rules(), repairRecipe: recipe).InspectCommitted());
            }
            var a = WithSecrets(0, 1, true); var b = WithSecrets(1000, 9999, false);
            Assert.That(Json(a.Session.Read()), Is.EqualTo(Json(b.Session.Read())));
            Assert.That(Json(a.Session.PreviewRepair()), Is.EqualTo(Json(b.Session.PreviewRepair())));
            Assert.That(Json(a.Session.Read()), Does.Not.Contain("private").And.Not.Contain("hidden").And.Not.Contain("9999"));
        }
        [Test]
        public void S15_01_ReadAndPreviewArePureIncludingReceiptsIdentityAndRng()
        {
            var initial = F.Initial(); var w = initial.World;
            var rng = new RngStreamState("own.fixture", new RngState(new RngContext(new string('0',64), "repair", "own", "initial", F.Actor.Value, "resolution"), 17));
            var p = new CommandProcessor(new WorldSnapshot(w.Identity, w.Tick, w.Actors, w.Lots, new[] { rng }, w.Repair), F.Rules(), repairRecipe: w.Repair!.Recipe);
            var h = new Harness(initial: p.InspectCommitted()); var before = h.Processor.InspectCommitted(); var hash = F.Hash(before);
            for (var i = 0; i < 10; i++) { h.Session.Read(); h.Session.PreviewRepair(); }
            var after = h.Processor.InspectCommitted(); Assert.That(after, Is.SameAs(before)); Assert.That(F.Hash(after), Is.EqualTo(hash));
            Assert.That(after.AcceptedReceipts, Is.Empty); Assert.That(after.MetadataReceipts, Is.Empty);
            Assert.That(after.World.RngStates.Single().State.DrawIndex, Is.EqualTo(17));
        }
        [Test]
        public void S15_01_StalePreviewRequiresNewPreviewAndNewRequestIdentity()
        {
            var h = new Harness(); var preview = h.Session.PreviewRepair();
            Assert.That(h.Session.AdvanceBy(1, "wait" ).Success, Is.True); var before = h.Processor.InspectCommitted();
            Assert.That(h.Session.ConfirmRepair(preview.Token!, "stale").Code, Is.EqualTo("stale_preview"));
            Assert.That(h.Processor.InspectCommitted(), Is.SameAs(before)); Assert.That(h.Session.Read().TimberReserved, Is.Zero);
            var fresh = h.Session.PreviewRepair(); Assert.That(h.Session.ConfirmRepair(fresh.Token!, "stale").Code, Is.EqualTo("request_mismatch"));
            Assert.That(h.Session.ConfirmRepair(fresh.Token!, "fresh").Success, Is.True);
            Assert.That(h.Session.Retry("stale").Code, Is.EqualTo("stale_preview")); Assert.That(h.Session.Read().TimberReserved, Is.EqualTo(2));
        }
        [Test]
        public void S15_01_DuplicateConfirmAndRetryReserveOnlyOnceAndRejectMismatchedIntent()
        {
            var h = new Harness(); var preview = h.Session.PreviewRepair(); var first = h.Session.ConfirmRepair(preview.Token!, "same");
            var before = h.Processor.InspectCommitted();
            Assert.That(first.Success, Is.True); Assert.That(h.Session.ConfirmRepair(preview.Token!, "same").Success, Is.True);
            Assert.That(h.Session.Retry("same").Success, Is.True); Assert.That(h.Processor.InspectCommitted(), Is.SameAs(before));
            Assert.That(h.Session.AdvanceBy(60, "same").Code, Is.EqualTo("request_mismatch"));
            Assert.That(h.Session.CancelOwnRepair("same").Code, Is.EqualTo("request_mismatch"));
            Assert.That(h.Session.Read().TimberReserved, Is.EqualTo(2));
        }
        [Test]
        public void S15_01_Start59Then60CompletesOnceAndCancelledBranchDoesNotCreateWood()
        {
            var h = new Harness(); Assert.That(h.Start().Success, Is.True);
            Assert.That(h.Session.AdvanceBy(59, "before").Success, Is.True); Assert.That(h.Session.Read().Tick, Is.EqualTo(59));
            Assert.That(h.Session.Read().BoatCondition, Is.EqualTo(500)); Assert.That(h.Session.Read().TimberReserved, Is.EqualTo(2));
            Assert.That(h.Session.AdvanceBy(1, "due").Success, Is.True); var done = h.Session.Read();
            Assert.That(done.BoatCondition, Is.EqualTo(700)); Assert.That(done.TimberOnHand, Is.EqualTo(8)); Assert.That(done.TimberReserved, Is.Zero);
            Assert.That(done.ActivityStatus, Is.EqualTo("completed")); Assert.That(done.CanCancel, Is.False);
            Assert.That(h.Session.AdvanceBy(1, "due").Success, Is.True); Assert.That(h.Session.Retry("due").Success, Is.True);
            Assert.That(h.Session.Read().Tick, Is.EqualTo(60)); Assert.That(h.Session.Read().TimberOnHand, Is.EqualTo(8));
            var cancelled = new Harness(); cancelled.Start(); Assert.That(cancelled.Session.CancelOwnRepair("cancel").Success, Is.True);
            Assert.That(cancelled.Session.AdvanceBy(60, "wait-cancelled").Success, Is.True); var view = cancelled.Session.Read();
            Assert.That(view.ActivityStatus, Is.EqualTo("cancelled")); Assert.That(view.TimberOnHand, Is.EqualTo(10));
            Assert.That(view.TimberReserved, Is.Zero); Assert.That(view.BoatCondition, Is.EqualTo(500));
        }
        [Test]
        public void S15_01_WaitWithoutRepairHasNoAutomaticMaterialOrConditionEffects()
        {
            var h = new Harness(); Assert.That(h.Session.AdvanceBy(60, "wait").Success, Is.True); var view = h.Session.Read();
            Assert.That(view.Tick, Is.EqualTo(60)); Assert.That(view.TimberOnHand, Is.EqualTo(10));
            Assert.That(view.TimberReserved, Is.Zero); Assert.That(view.BoatCondition, Is.EqualTo(500)); Assert.That(view.ActivityStatus, Is.EqualTo("none"));
        }
        [Test]
        public void S15_01_SaveAndReloadCallbacksPreserveRunningActivityAndRetryOriginalEnvelope()
        {
            var h = new Harness(); h.Start(); Assert.That(h.Session.SaveSnapshot().Code, Is.EqualTo("saved"));
            Assert.That(h.Session.SaveSnapshot().Durable, Is.True); Assert.That(h.Snapshots, Is.EqualTo(2));
            Assert.That(h.Session.Reload().Code, Is.EqualTo("loaded")); Assert.That(h.Session.Retry("repair").Success, Is.True);
            Assert.That(h.Session.Read().TimberReserved, Is.EqualTo(2)); Assert.That(h.Session.Read().ActivityStatus, Is.EqualTo("running"));
            Assert.That(h.Session.AdvanceBy(60, "finish").Success, Is.True); Assert.That(h.Session.Read().BoatCondition, Is.EqualTo(700));
        }
        [Test]
        public void S15_01_UnknownRequestAndInvalidClientIdsDoNotExposeExistingReceipts()
        {
            var h = new Harness(); var before = h.Processor.InspectCommitted();
            Assert.That(h.Session.Retry("missing").Code, Is.EqualTo("not_found"));
            Assert.That(h.Session.Retry("scheduler/job/secret").Code, Is.EqualTo("invalid_request"));
            Assert.That(h.Session.AdvanceBy(60, "bad/path").Code, Is.EqualTo("invalid_request"));
            Assert.That(h.Session.AdvanceBy(0, "zero").Code, Is.EqualTo("invalid_request"));
            Assert.That(h.Session.AdvanceBy(1441, "too-long").Code, Is.EqualTo("invalid_request"));
            Assert.That(h.Processor.InspectCommitted(), Is.SameAs(before));
        }
        [Test]
        public void S15_01_ExceptionsAreSanitizedAndFailedCallbacksKeepCurrentSession()
        {
            var h = new Harness(); h.Start(); var before = Json(h.Session.Read());
            h.FailSave = true; var save = h.Session.SaveSnapshot(); Assert.That(save.Success, Is.False); Assert.That(save.Durable, Is.False); Assert.That(save.Code, Is.EqualTo("storage_error"));
            h.FailLoad = true; var load = h.Session.Reload(); Assert.That(load.Code, Is.EqualTo("storage_error")); Assert.That(load.Durable, Is.False);
            Assert.That(Json(h.Session.Read()), Is.EqualTo(before)); h.FailLoad = false; h.FailRestore = true;
            Assert.That(h.Session.Reload().Code, Is.EqualTo("storage_error")); Assert.That(Json(h.Session.Read()), Is.EqualTo(before));
            Assert.That(Json(save) + Json(load), Does.Not.Contain("secret").And.Not.Contain("password").And.Not.Contain("SQL"));
        }
        [Test]
        public void S15_01_PreCommitFailureKeepsOriginalIntentForRetryWithoutEffects()
        {
            var fail = true; var h = new Harness(beforeCommit: () => { if (fail) throw new InvalidOperationException("private failure"); });
            var before = F.Hash(h.Processor.InspectCommitted()); var preview = h.Session.PreviewRepair();
            var first = h.Session.ConfirmRepair(preview.Token!, "retry-after-failure"); Assert.That(first.Code, Is.EqualTo("session_error"));
            Assert.That(first.Durable, Is.False); Assert.That(F.Hash(h.Processor.InspectCommitted()), Is.EqualTo(before));
            fail = false; Assert.That(h.Session.Retry("retry-after-failure").Success, Is.True); Assert.That(h.Session.Read().TimberReserved, Is.EqualTo(2));
        }
        [Test]
        public void S15_01_ResponseLossRetryUsesCommittedReceiptRatherThanNewMutation()
        {
            var fail = true; var h = new Harness(afterCommit: () => { if (fail) throw new InvalidOperationException("private response failure"); });
            Assert.That(h.Start().Code, Is.EqualTo("session_error")); var committed = h.Processor.InspectCommitted(); fail = false;
            Assert.That(h.Session.Retry("repair").Success, Is.True); Assert.That(h.Processor.InspectCommitted(), Is.SameAs(committed));
            Assert.That(h.Session.Read().TimberReserved, Is.EqualTo(2));
        }
        [Test]
        public void S15_01_ReloadInvalidatesPreviewTokensBeforeConfirm()
        {
            var h = new Harness(); var token = h.Session.PreviewRepair().Token!; Assert.That(h.Session.Reload().Success, Is.True);
            Assert.That(h.Session.ConfirmRepair(token, "obsolete").Code, Is.EqualTo("stale_preview")); Assert.That(h.Session.Read().TimberReserved, Is.Zero);
        }
        [Test]
        public void S15_01_FreshSessionCanRetryOnlyItsOwnRecordedCommand()
        {
            var h = new Harness(); h.Start(); var committed = h.Processor.InspectCommitted(); var next = new Harness(initial: committed);
            Assert.That(next.Session.Retry("repair").Success, Is.True); Assert.That(next.Processor.InspectCommitted(), Is.SameAs(committed));
            Assert.That(next.Session.Read().TimberReserved, Is.EqualTo(2));
        }
        [Test]
        public void S15_01_BoundedActivityProjectionDoesNotAdoptAnotherBoatActivity()
        {
            var h = new Harness(initial: RunningRepairOnOtherBoat().InspectCommitted()); var view = h.Session.Read();
            Assert.That(view.ActivityStatus, Is.EqualTo("none")); Assert.That(view.ActivityDueTick, Is.Null);
            Assert.That(view.CanCancel, Is.False); Assert.That(view.BoatCondition, Is.EqualTo(500));
        }
        [Test]
        public void S15_01_FreshRetryCannotAdoptSameActorCommandForUnboundBoat()
        {
            var h = new Harness(initial: RunningRepairOnOtherBoat().InspectCommitted()); var before = h.Processor.InspectCommitted();
            Assert.That(h.Session.Retry("outside").Code, Is.EqualTo("not_found"));
            Assert.That(h.Processor.InspectCommitted(), Is.SameAs(before));
        }
        [Test]
        public void S15_01_KnownInsufficientMaterialProducesOnlySafeUnavailablePreview()
        {
            var h = new Harness(initial: F.Initial(timber: 1)); var before = F.Hash(h.Processor.InspectCommitted()); var preview = h.Session.PreviewRepair();
            Assert.That(preview.RequirementsKnown, Is.True); Assert.That(preview.Code, Is.EqualTo("unavailable"));
            Assert.That(preview.CanConfirm, Is.False); Assert.That(preview.Token, Is.Null); Assert.That(preview.CostTimber, Is.EqualTo(2));
            Assert.That(F.Hash(h.Processor.InspectCommitted()), Is.EqualTo(before));
        }
    }
}
