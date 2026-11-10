using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.Application.CommandPipeline;
using Game.Application.Events;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Domain.Weaving;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using NUnit.Framework;
using F = MythicalDnd.Foundation.Tests.WeavingTestFixture;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class WeavingHistoryTests
    {
        private static CommittedWorld Recompose(CommittedWorld source, WorldSnapshot? world = null, SchedulerState? scheduler = null,
            IEnumerable<CommittedEvent>? facts = null, IEnumerable<CommandReceipt>? receipts = null)
        {
            var ctor = typeof(CommittedWorld).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single();
            return (CommittedWorld)ctor.Invoke(new object[] { world ?? source.World, receipts ?? source.AcceptedReceipts,
                source.MetadataReceipts, source.Events, scheduler ?? source.Scheduler, facts ?? source.Journal.Events });
        }
        private static CommandProcessor Started()
        {
            var p = F.Processor(); p.Execute(F.Start(p), F.Principal(p.InspectCommitted().World)); return p;
        }
        private static WorldSnapshot With(CommittedWorld source, IEnumerable<ResourceLot>? lots = null, IEnumerable<ActorState>? actors = null, WeavingState? weaving = null)
        {
            var w = source.World;
            return new WorldSnapshot(w.Identity, w.Tick, actors ?? w.Actors, lots ?? w.Lots, w.RngStates, origin: w.Origin, weaving: weaving ?? w.Weaving);
        }
        private static void Reject(CommittedWorld bad, string prefix = "WEAVING_HISTORY_")
        {
            var error = Assert.Throws<ArgumentException>(() => new CommandProcessor(bad, F.Rules(), weavingRecipe: F.Recipe));
            Assert.That(error!.Message, Does.StartWith(prefix));
        }
        private static CommittedEvent Fact(CommittedEvent source, string? type = null, IReadOnlyDictionary<string, object?>? payload = null)
            => new CommittedEvent(source.Id, type ?? source.EventType, source.AggregateId, source.Tick, source.CommitSequence, source.EventIndex,
                source.CausationId, source.CorrelationId, source.RuleVersion, source.PayloadVersion, payload ?? source.Payload, source.EnvelopeVersion);
        [Test]
        public void S17_01_OrphanStartReceiptCannotSupportHold()
        {
            var source = Started().InspectCommitted(); Reject(Recompose(source, receipts: Array.Empty<CommandReceipt>()));
        }
        [TestCase("fiber_cost")] [TestCase("duration_ticks")] [TestCase("cord_yield")] [TestCase("minimum_skill")]
        [TestCase("skill_value")] [TestCase("recipe_hash")] [TestCase("content_hash")] [TestCase("input_lot_id")]
        public void S17_01_ForgedEffectiveFactsRejected(string key)
        {
            var source = Started().InspectCommitted(); var fact = source.Journal.Events.Single(); var payload = fact.Payload.ToDictionary(x => x.Key, x => x.Value);
            payload[key] = key.EndsWith("hash", StringComparison.Ordinal) ? new string('f', 64) : key == "input_lot_id" ? "wrong" : (object)999L;
            Reject(Recompose(source, facts: new[] { Fact(fact, payload: payload) }));
        }
        [TestCase("input")] [TestCase("output")] [TestCase("reserve")]
        public void S17_01_ForgedStockOrHoldWithoutAcceptedCauseRejected(string change)
        {
            var p = F.Processor(); var source = p.InspectCommitted(); var s = source.World.Weaving!;
            var lots = source.World.Lots.Select(l => new ResourceLot(l.Id, l.Commodity, l.Owner, l.Location,
                change == "input" && l.Id.Equals(s.InputLotId) ? 7 : change == "output" && l.Id.Equals(s.OutputLotId) ? 1 : l.OnHand,
                change == "reserve" && l.Id.Equals(s.InputLotId) ? 1 : l.Reserved));
            Reject(Recompose(source, With(source, lots)));
        }
        [Test]
        public void S17_01_TerminalCompletionNeedsItsFactAndAdvanceCause()
        {
            var p = Started(); F.Advance(p, 30); var source = p.InspectCommitted();
            Reject(Recompose(source, facts: source.Journal.Events.Select(e => e.EventType == "WeavingCompleted" ? Fact(e, type: "Unrelated") : e)));
            Reject(Recompose(source, receipts: source.AcceptedReceipts.Where(r => r.OriginalEnvelope.CommandType != "AdvanceTime")));
        }
        [Test]
        public void S17_01_CancelReasonMustMatchCommittedFact()
        {
            var p = Started(); p.Execute(F.Cancel(p), F.Principal(p.InspectCommitted().World)); var source = p.InspectCommitted(); var s = source.World.Weaving!; var old = s.Activities.Single();
            var changed = new WeavingActivity(old.Id, old.ActorId, old.JobId, old.StartedTick, old.DueTick, old.StartCommandId, old.CorrelationId, old.Status, 0, "different");
            var weaving = new WeavingState(s.Recipe, s.ContentHash, s.ActorId, s.Worksite, s.InputLotId, s.OutputLotId, s.SkillValue, new[] { changed });
            Reject(Recompose(source, With(source, weaving: weaving)));
        }
        [TestCase("actor")] [TestCase("input")] [TestCase("output")]
        public void S17_01_DueRevalidatesLocationAndCancelsWithoutOutput(string moved)
        {
            var p = Started(); var source = p.InspectCommitted(); var w = source.World; var s = w.Weaving!;
            var stage = w.Identity.BeginStage(w.Revision.Value + 1); var away = stage.Allocate<Settlement>();
            var actors = w.Actors.Select(a => new ActorState(a.Id, a.Alive, moved == "actor" ? away : a.Location, a.ControlledOwnerIds));
            var lots = w.Lots.Select(l => new ResourceLot(l.Id, l.Commodity, l.Owner,
                (moved == "input" && l.Id.Equals(s.InputLotId)) || (moved == "output" && l.Id.Equals(s.OutputLotId)) ? away : l.Location, l.OnHand, l.Reserved));
            var changed = new WorldSnapshot(stage.PrepareSnapshot(), w.Tick, actors, lots, w.RngStates, origin: w.Origin, weaving: s);
            p = new CommandProcessor(Recompose(source, changed), F.Rules(), weavingRecipe: F.Recipe); F.Advance(p, 30);
            var final = p.InspectCommitted(); Assert.That(final.World.Weaving!.Activities.Single().Status, Is.EqualTo(WeavingActivityStatus.Cancelled));
            Assert.That(final.World.Weaving.Activities.Single().TerminalReason, Is.EqualTo("LOCATION_MISMATCH"));
            Assert.That(final.World.GetLot(s.InputLotId).OnHand, Is.EqualTo(8)); Assert.That(final.World.GetLot(s.InputLotId).Reserved, Is.Zero); Assert.That(final.World.GetLot(s.OutputLotId).OnHand, Is.Zero);
        }
        [TestCase("input")] [TestCase("output")]
        public void S17_01_DueRevalidatesRightsWithoutAssumingHouseholdAuthority(string denied)
        {
            var p = Started(); var source = p.InspectCommitted(); var w = source.World; var s = w.Weaving!; var household = w.Origin!.Household.Value;
            var lots = w.Lots.Select(l => new ResourceLot(l.Id, l.Commodity,
                (denied == "input" && l.Id.Equals(s.InputLotId)) || (denied == "output" && l.Id.Equals(s.OutputLotId)) ? household : l.Owner, l.Location, l.OnHand, l.Reserved));
            p = new CommandProcessor(Recompose(source, With(source, lots)), F.Rules(), weavingRecipe: F.Recipe); F.Advance(p, 30);
            var final = p.InspectCommitted(); Assert.That(final.World.Weaving!.Activities.Single().Status, Is.EqualTo(WeavingActivityStatus.Cancelled));
            Assert.That(final.World.Weaving.Activities.Single().TerminalReason, Is.EqualTo("NOT_AUTHORIZED")); Assert.That(final.World.GetLot(s.OutputLotId).OnHand, Is.Zero);
        }
        [TestCase("input")] [TestCase("output")]
        public void S17_01_DueRevalidatesChangedCommodity(string changed)
        {
            var p = Started(); var source = p.InspectCommitted(); var s = source.World.Weaving!;
            var lots = source.World.Lots.Select(l => new ResourceLot(l.Id,
                (changed == "input" && l.Id.Equals(s.InputLotId)) || (changed == "output" && l.Id.Equals(s.OutputLotId)) ? DefinitionId.Parse("commodity.other@1") : l.Commodity,
                l.Owner, l.Location, l.OnHand, l.Reserved));
            p = new CommandProcessor(Recompose(source, With(source, lots)), F.Rules(), weavingRecipe: F.Recipe); F.Advance(p, 30);
            Assert.That(p.InspectCommitted().World.Weaving!.Activities.Single().TerminalReason, Is.EqualTo("COMMODITY_MISMATCH"));
            Assert.That(p.InspectCommitted().World.GetLot(s.OutputLotId).OnHand, Is.Zero);
        }
        [Test]
        public void S17_01_DeathAfterAdvanceRequestCancelsAtDueWithoutFabricatingResource()
        {
            var p = Started(); var principal = F.Principal(p.InspectCommitted().World);
            p.Execute(F.Command(p, "before.death", "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = 30L }), principal);
            var source = p.InspectCommitted(); var s = source.World.Weaving!;
            var actors = source.World.Actors.Select(a => new ActorState(a.Id, false, a.Location, a.ControlledOwnerIds));
            p = new CommandProcessor(Recompose(source, With(source, actors: actors)), F.Rules(), weavingRecipe: F.Recipe); p.DrainAdvance(principal);
            Assert.That(p.InspectCommitted().World.Weaving!.Activities.Single().TerminalReason, Is.EqualTo("ACTOR_DEAD"));
            Assert.That(p.InspectCommitted().World.GetLot(s.InputLotId).OnHand, Is.EqualTo(8));
            Assert.That(p.InspectCommitted().World.GetLot(s.InputLotId).Reserved, Is.Zero);
            Assert.That(p.InspectCommitted().World.GetLot(s.OutputLotId).OnHand, Is.Zero);
        }
        [Test]
        public void S17_01_PendingBeforeAndAfterJobStatesRestoreAndDrainWithoutDoubleEffect()
        {
            var p = Started(); var principal = F.Principal(p.InspectCommitted().World);
            p.Execute(F.Command(p, "partial.advance", "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = 60L }), principal);
            var pending = p.InspectCommitted();
            CommittedWorld? afterJob = null;
            p = new CommandProcessor(pending, F.Rules(), afterCommit: () => {
                var published = p.InspectCommitted();
                if (published.World.Tick.Value == 30 && published.Scheduler.Workflow!.Status == AdvanceStatus.Pending)
                    afterJob = published;
            }, weavingRecipe: F.Recipe);
            Assert.That(p.InspectCommitted().Scheduler.Workflow!.Status, Is.EqualTo(AdvanceStatus.Pending));
            p.DrainAdvance(principal, 1);
            Assert.That(afterJob, Is.Not.Null, "Observe the actual job publication before CloseAdvance; budget1 may also close when no other jobs remain");
            var s = afterJob!.World.Weaving!;
            Assert.That(afterJob.World.Tick.Value, Is.EqualTo(30));
            Assert.That(afterJob.World.GetLot(s.InputLotId).OnHand, Is.EqualTo(6));
            Assert.That(afterJob.World.GetLot(s.OutputLotId).OnHand, Is.EqualTo(1));
            p = new CommandProcessor(afterJob, F.Rules(), weavingRecipe: F.Recipe); p.DrainAdvance(principal);
            var final = p.InspectCommitted(); Assert.That(final.World.Tick.Value, Is.EqualTo(60));
            Assert.That(final.World.GetLot(s.InputLotId).OnHand, Is.EqualTo(6));
            Assert.That(final.World.GetLot(s.OutputLotId).OnHand, Is.EqualTo(1));
            Assert.That(final.Scheduler.Executions.Count, Is.EqualTo(1));
            Assert.That(final.World.RngStates.Single().State.DrawIndex, Is.EqualTo(1));
        }
        [Test]
        public void S17_01_ForgedJobCreationSequenceRejected()
        {
            var source = Started().InspectCommitted(); var old = source.Scheduler.Jobs.Single();
            var forged = ScheduledJob.ForWeaving(old.Id, old.ActorId, old.DueTick, 2, old.WeavingActivityId!, old.CausationId, old.CorrelationId);
            Reject(Recompose(source, scheduler: SchedulerState.Restore(new[] { forged }, 3)));
        }
        [Test]
        public void S17_01_PendingJobCannotChangePinnedDue()
        {
            var source = Started().InspectCommitted(); var old = source.Scheduler.Jobs.Single();
            var changed = ScheduledJob.ForWeaving(old.Id, old.ActorId, new WorldTick(31), old.CreationSequence, old.WeavingActivityId!, old.CausationId, old.CorrelationId);
            Reject(Recompose(source, scheduler: SchedulerState.Restore(new[] { changed }, 2)), "WEAVING_JOB_PAIRING");
        }
    }
}
