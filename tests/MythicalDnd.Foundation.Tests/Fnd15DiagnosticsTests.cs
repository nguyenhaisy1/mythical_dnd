using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Foundation.Time;
using Game.Infrastructure.Diagnostics;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Replay;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class Fnd15DiagnosticsTests
    {
        private static EntityId<Actor> ActorId => EntityId<Actor>.Parse("w0000000000000001/actor/0000000000000001");
        private static EntityId<Lot> Source => EntityId<Lot>.Parse("w0000000000000001/lot/0000000000000001");
        private static EntityId<Lot> Destination => EntityId<Lot>.Parse("w0000000000000001/lot/0000000000000002");
        private static ExecutionPrincipal Principal => new ExecutionPrincipal(ActorId, CommandSource.Player);
        private static WorldSnapshot World(ulong draw = 7)
        {
            var ids = IdentitySnapshot.Empty(1).BeginStage(1);
            var actor = ids.Allocate<Actor>(); var location = ids.Allocate<Settlement>();
            var a = ids.Allocate<Lot>(); var b = ids.Allocate<Lot>();
            var rng = new RngStreamState("diagnostics", new RngState(new RngContext(new string('0', 64), "test", "transfer", "once", actor.Value, "main"), draw));
            return new WorldSnapshot(ids.PrepareSnapshot(), new WorldTick(0), new[] { new ActorState(actor, true, location) },
                new[] { new ResourceLot(a, DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 10, 0),
                    new ResourceLot(b, DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 0, 0) }, new[] { rng });
        }
        private static CommandProcessor Processor(IRuleHandler? handler = null, ulong draw = 7)
            => new CommandProcessor(World(draw), new RuleRegistry(new[] { handler ?? new TransferResourceRule() }));
        private static Dictionary<string, object?> Payload(long quantity = 2) => new Dictionary<string, object?> {
            ["source_lot_id"] = Source.ToString(), ["destination_lot_id"] = Destination.ToString(), ["quantity"] = quantity };
        private static CommandEnvelope Command(string id = "transfer", ulong revision = 1, string type = "TransferResource", Dictionary<string, object?>? payload = null)
            => new CommandEnvelope(id, type, ActorId.ToString(), "Player", 0, revision, 1, payload ?? Payload(), "shared.correlation");
        private static void Schedule(CommandProcessor p, string id = "schedule", ulong revision = 1)
        {
            var payload = Payload(); payload["due_tick"] = 10L; payload["phase"] = 3; payload["priority"] = 100;
            p.Execute(Command(id, revision, "ScheduleTransfer", payload), Principal);
        }
        private sealed class CountingRule : IRuleHandler
        {
            public int Calls { get; private set; }
            public bool Throw { get; set; }
            public string CommandType => "TransferResource";
            public int PayloadVersion => 1;
            public Type PayloadType => typeof(TransferResource);
            public RuleResult Evaluate(WorldSnapshot world, EntityId<Actor> issuer, IKernelCommand command)
            { Calls++; if (Throw) throw new InvalidOperationException("injected"); return new TransferResourceRule().Evaluate(world, issuer, command); }
        }
        [Test]
        public void FND_15_IdenticalCommittedImagesHaveNoDiff()
        {
            var state = Processor().InspectCommitted();
            var diff = StateDiffer.Compare(state, CommittedWorldCodec.Decode(CommittedWorldCodec.Encode(state)));
            Assert.That(diff.Entries, Is.Empty); Assert.That(diff.TotalChanges, Is.Zero); Assert.That(diff.Truncated, Is.False);
        }
        [Test]
        public void FND_15_TransferDiffHasExactResourceValuesAndRevision()
        {
            var p = Processor(); var before = p.InspectCommitted(); p.Execute(Command(), Principal);
            var after = p.InspectCommitted(); var bytes = CommittedWorldCodec.Encode(after);
            var diff = StateDiffer.Compare(before, after);
            var source = diff.Entries.Single(x => x.Path == "/lots/0/on_hand");
            Assert.That(source.Before.Value, Is.EqualTo(10)); Assert.That(source.After.Value, Is.EqualTo(8));
            Assert.That(diff.Entries.Single(x => x.Path == "/lots/1/on_hand").After.Value, Is.EqualTo(2));
            Assert.That(diff.BeforeRevision, Is.EqualTo(1)); Assert.That(diff.AfterRevision, Is.EqualTo(2));
            Assert.That(CommittedWorldCodec.Encode(after), Is.EqualTo(bytes));
        }
        [Test]
        public void FND_15_MetadataOnlyRejectionIsVisibleWithoutGameplayChange()
        {
            var p = Processor(); var before = p.InspectCommitted(); p.Execute(Command(type: "Unknown"), Principal);
            var diff = StateDiffer.Compare(before, p.InspectCommitted());
            Assert.That(diff.Entries, Is.Not.Empty);
            Assert.That(diff.Entries.All(x => (x.Path == "/metadata" || x.Path.StartsWith("/metadata/", StringComparison.Ordinal))), Is.True);
            Assert.That(diff.BeforeRevision, Is.EqualTo(diff.AfterRevision));
        }
        [Test]
        public void FND_15_RngCounterDiffDoesNotConsumeRandomness()
        {
            var before = Processor(draw: 7).InspectCommitted(); var after = Processor(draw: 8).InspectCommitted();
            var diff = StateDiffer.Compare(before, after);
            Assert.That(diff.TotalChanges, Is.EqualTo(1)); Assert.That(diff.Entries.Single().Path, Is.EqualTo("/rng/0/draw"));
            Assert.That(diff.Entries.Single().Before.Value, Is.EqualTo(7)); Assert.That(diff.Entries.Single().After.Value, Is.EqualTo(8));
            Assert.That(before.World.RngStates.Single().State.DrawIndex, Is.EqualTo(7));
        }
        [Test]
        public void FND_15_DiffOrderingAndBoundsAreStableAcrossCulture()
        {
            var p = Processor(); var before = p.InspectCommitted(); p.Execute(Command(), Principal); var after = p.InspectCommitted();
            var culture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR"); var full = StateDiffer.Compare(before, after);
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("vi-VN"); var bounded = StateDiffer.Compare(before, after, 2);
                Assert.That(bounded.Entries.Count, Is.EqualTo(2)); Assert.That(bounded.Truncated, Is.True);
                Assert.That(bounded.TotalChanges, Is.EqualTo(full.TotalChanges));
                Assert.That(bounded.Entries.Select(x => x.Path), Is.EqualTo(full.Entries.Take(2).Select(x => x.Path)));
                Assert.That(full.Entries.Select(x => x.Path), Is.Ordered.Using<string>(StringComparer.Ordinal));
            }
            finally { CultureInfo.CurrentCulture = culture; }
        }
        [Test]
        public void FND_15_RemovedArrayElementIsMissingNotNull()
        {
            var p = Processor(); var initial = p.InspectCommitted(); p.Execute(Command(), Principal);
            var diff = StateDiffer.Compare(p.InspectCommitted(), initial);
            Assert.That(diff.Entries.Any(x => x.Path.StartsWith("/accepted/", StringComparison.Ordinal) && x.After.Kind.ToString() == "Missing"), Is.True);
        }
        [TestCase(0)][TestCase(-1)]
        public void FND_15_DiffRejectsInvalidBounds(int limit)
            => Assert.Throws<ArgumentOutOfRangeException>(() => StateDiffer.Compare(Processor().InspectCommitted(), Processor().InspectCommitted(), limit));
        [Test]
        public void FND_15_TraceCapturesRealEvaluationOnceAndInspectionHasNoSideEffects()
        {
            var inner = new CountingRule(); var inspector = new RuleInspectionHandler(inner); var p = Processor(inspector);
            var initial = p.InspectCommitted(); p.Execute(Command(), Principal); var final = p.InspectCommitted(); var bytes = CommittedWorldCodec.Encode(final);
            Assert.That(inner.Calls, Is.EqualTo(1)); var trace = inspector.Traces.Single();
            Assert.That(trace.Input, Is.SameAs(initial.World)); Assert.That(trace.BeforeRevision, Is.EqualTo(1));
            Assert.That(trace.CommandType, Is.EqualTo("TransferResource")); Assert.That(trace.PayloadVersion, Is.EqualTo(1));
            Assert.That(trace.Result.Changes!.LotChanges.Select(x => x.NewOnHand), Is.EqualTo(new long[] { 8, 2 }));
            StateDiffer.Compare(initial, final); CausalInspector.Inspect(final); SchedulerInspector.Inspect(final);
            Assert.That(inspector.Traces.Count, Is.EqualTo(1)); Assert.That(inner.Calls, Is.EqualTo(1));
            Assert.That(CommittedWorldCodec.Encode(final), Is.EqualTo(bytes)); Assert.That(final.World.RngStates.Single().State.DrawIndex, Is.EqualTo(7));
        }
        [Test]
        public void FND_15_RetryAndValidationFailuresDoNotFabricateRuleTraces()
        {
            var inner = new CountingRule(); var inspector = new RuleInspectionHandler(inner); var p = Processor(inspector);
            var command = Command(); p.Execute(command, Principal); p.Execute(command, Principal);
            p.Execute(Command("stale"), Principal); p.Execute(Command("unknown", 2, "Unknown"), Principal);
            Assert.That(inner.Calls, Is.EqualTo(1)); Assert.That(inspector.Traces.Count, Is.EqualTo(1));
        }
        [Test]
        public void FND_15_RejectedRuleTracePreservesActualReason()
        {
            var inspector = new RuleInspectionHandler(new TransferResourceRule()); var p = Processor(inspector);
            p.Execute(Command(payload: Payload(99)), Principal);
            Assert.That(inspector.Traces.Single().Result.ErrorCode, Is.EqualTo("INSUFFICIENT_RESOURCE"));
            Assert.That(p.InspectCommitted().World.Revision.Value, Is.EqualTo(1));
        }
        [Test]
        public void FND_15_BoundedTracingDoesNotStopGameplayEvaluations()
        {
            var inner = new CountingRule(); var inspector = new RuleInspectionHandler(inner, 1); var p = Processor(inspector);
            p.Execute(Command(), Principal); p.Execute(Command("second", 2), Principal);
            Assert.That(inner.Calls, Is.EqualTo(2)); Assert.That(inspector.Traces.Count, Is.EqualTo(1)); Assert.That(inspector.DroppedCount, Is.EqualTo(1));
            Assert.That(p.InspectCommitted().World.Lots[0].OnHand, Is.EqualTo(6));
        }
        [Test]
        public void FND_15_HandlerExceptionPropagatesWithoutFakeSuccessfulTrace()
        {
            var inner = new CountingRule { Throw = true }; var inspector = new RuleInspectionHandler(inner); var p = Processor(inspector);
            Assert.Throws<InvalidOperationException>(() => p.Execute(Command(), Principal));
            Assert.That(inner.Calls, Is.EqualTo(1)); Assert.That(inspector.Traces, Is.Empty);
            Assert.That(p.InspectCommitted().World.Revision.Value, Is.EqualTo(1));
        }
        [Test]
        public void FND_15_SharedCorrelationDoesNotCreateCausalEdgeBetweenCommands()
        {
            var p = Processor(); p.Execute(Command("first"), Principal); p.Execute(Command("second", 2), Principal);
            var state = p.InspectCommitted(); var graph = CausalInspector.Inspect(state);
            foreach (var fact in state.Journal.Events)
                Assert.That(graph.Edges.Any(e => e.From == "command:" + fact.CausationId && e.To == "event:" + fact.Id && e.FromResolved && e.ToResolved), Is.True);
            Assert.That(graph.Edges.Any(e => e.From == "command:first" && e.To == "command:second" || e.From == "command:second" && e.To == "command:first"), Is.False);
            Assert.That(graph.Edges.Any(e => e.From.Contains("shared.correlation") || e.To.Contains("shared.correlation")), Is.False);
        }
        [Test]
        public void FND_15_PendingAndExecutedJobsRetainActualExecutionLinks()
        {
            var p = Processor(); Schedule(p); var pending = SchedulerInspector.Inspect(p.InspectCommitted()).Single();
            Assert.That(pending.Job.Status, Is.EqualTo(ScheduledJobStatus.Pending)); Assert.That(pending.Execution, Is.Null);
            p.Execute(Command("advance", 2, "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = 20L }), Principal);
            p.DrainAdvance(Principal); var state = p.InspectCommitted(); var completed = SchedulerInspector.Inspect(state).Single();
            Assert.That(completed.Job.Status, Is.EqualTo(ScheduledJobStatus.Completed)); Assert.That(completed.Execution, Is.Not.Null);
            Assert.That(completed.Execution!.JobId, Is.EqualTo(completed.Job.Id)); Assert.That(completed.Execution.Tick.Value, Is.EqualTo(10));
            var graph = CausalInspector.Inspect(state);
            Assert.That(graph.Edges.Any(e => e.From == "job:" + completed.Job.Id && e.To == "command:" + completed.Execution.CommandId && e.FromResolved && e.ToResolved), Is.True);
            Assert.That(graph.Edges.Any(e => e.From == "command:schedule" && e.To == "job:" + completed.Job.Id), Is.True);
        }
        [Test]
        public void FND_15_CausalBoundsDiscloseTotalAndDoNotMutateState()
        {
            var p = Processor(); p.Execute(Command(), Principal); p.Execute(Command("next", 2), Principal); var state = p.InspectCommitted();
            var bytes = CommittedWorldCodec.Encode(state); var full = CausalInspector.Inspect(state); var limited = CausalInspector.Inspect(state, 1);
            Assert.That(limited.Edges.Count, Is.EqualTo(1)); Assert.That(limited.Truncated, Is.True); Assert.That(limited.TotalEdges, Is.EqualTo(full.TotalEdges));
            Assert.That(CommittedWorldCodec.Encode(state), Is.EqualTo(bytes));
        }
        [Test]
        public void FND_15_NullMissingAndTypedPayloadValuesRemainDistinct()
        {
            var p = Processor(); var before = p.InspectCommitted();
            p.Execute(Command(type: "Unknown", payload: new Dictionary<string, object?> { ["nullable"] = null, ["number"] = 1L, ["text"] = "1" }), Principal);
            var diff = StateDiffer.Compare(before, p.InspectCommitted());
            var entry = diff.Entries.Single(x => x.Path == "/metadata/0/envelope/payload/nullable");
            Assert.That(entry.Before.Kind, Is.EqualTo(DiagnosticValueKind.Missing)); Assert.That(entry.After.Kind, Is.EqualTo(DiagnosticValueKind.Null));
            Assert.That(diff.Entries.Single(x => x.Path.EndsWith("/payload/number", StringComparison.Ordinal)).After.Kind, Is.EqualTo(DiagnosticValueKind.Integer));
            Assert.That(diff.Entries.Single(x => x.Path.EndsWith("/payload/text", StringComparison.Ordinal)).After.Kind, Is.EqualTo(DiagnosticValueKind.String));
        }
        [Test]
        public void FND_15_UnknownUpstreamIsExplicitlyUnresolved()
        {
            var world = World(); var stage = world.Identity.BeginStage(2); var id = stage.Allocate<Job>();
            var withJob = new WorldSnapshot(stage.PrepareSnapshot(), world.Tick, world.Actors, world.Lots, world.RngStates);
            var job = new ScheduledJob(id, ActorId, new WorldTick(10), SchedulerPhase.ProductionHealth, 100, 1,
                new TransferResource(Source, Destination, 2), "upstream.not.in.image", "shared.correlation");
            var p = new CommandProcessor(withJob, new RuleRegistry(new[] { new TransferResourceRule() }), initialScheduler: SchedulerState.Restore(new[] { job }, 2));
            var edge = CausalInspector.Inspect(p.InspectCommitted()).Edges.Single();
            Assert.That(edge.From, Is.EqualTo("external:upstream.not.in.image")); Assert.That(edge.FromResolved, Is.False);
            Assert.That(edge.To, Is.EqualTo("job:" + id)); Assert.That(edge.ToResolved, Is.True);
        }
        [Test]
        public void FND_15_ExplicitCancellationDoesNotInventAReasonOrExecution()
        {
            var p = Processor(); Schedule(p); var id = p.InspectCommitted().Scheduler.Jobs.Single().Id;
            p.Execute(Command("cancel", 2, "CancelJob", new Dictionary<string, object?> { ["job_id"] = id.ToString() }), Principal);
            var inspection = SchedulerInspector.Inspect(p.InspectCommitted()).Single();
            Assert.That(inspection.Job.Status, Is.EqualTo(ScheduledJobStatus.Cancelled)); Assert.That(inspection.Execution, Is.Null);
            Assert.That(inspection.TerminalEvent!.CausationId, Is.EqualTo("cancel")); Assert.That(inspection.CancellationReason, Is.Null);
            Assert.That(inspection.CancellationReasonStatus, Is.EqualTo("not-recorded"));
        }
        [Test]
        public void FND_15_CausalTraversalRespectsDirectionAndDisconnectedCorrelation()
        {
            var p = Processor(); p.Execute(Command("first"), Principal); p.Execute(Command("second", 2), Principal);
            var state = p.InspectCommitted(); var graph = CausalInspector.Inspect(state); var firstEvent = "event:" + state.Journal.Events[0].Id;
            var upstream = CausalInspector.Traverse(graph, firstEvent, true);
            Assert.That(upstream.Edges.All(e => e.From == "command:first" && e.To == firstEvent), Is.True); Assert.That(upstream.Edges, Is.Not.Empty);
            Assert.That(CausalInspector.Traverse(graph, firstEvent, false).Edges, Is.Empty);
            Assert.That(CausalInspector.Traverse(graph, "command:first", false).Edges.Select(e => e.To).Distinct(), Is.EqualTo(new[] { firstEvent }));
        }
        [Test]
        public void FND_15_ActualChecksStopAtFailedPrecondition()
        {
            var inspector = new RuleInspectionHandler(new TransferResourceRule());
            var p = Processor(inspector);
            var receipt = p.Execute(Command(payload: Payload(11)), Principal);
            var checks = inspector.Traces.Single().Result.Checks;
            Assert.That(receipt.ErrorCode, Is.EqualTo("INSUFFICIENT_RESOURCE"));
            Assert.That(checks.Select(c => c.Name), Is.EqualTo(new[] { "payload_type", "quantity_positive", "actor_alive", "controls_source", "commodity_match", "location_match", "available_quantity" }));
            Assert.That(checks.Last().Passed, Is.False);
            Assert.That(checks.Take(checks.Count - 1).All(c => c.Passed), Is.True);
            Assert.That(checks.Any(c => c.Name == "destination_capacity"), Is.False);
        }
        [Test]
        public void FND_15_ReplayObserverReportsActualStatesAndDoesNotChangeCheckpoints()
        {
            var initial = Processor().InspectCommitted();
            var actions = new[] { ReplayAction.Drain(Principal), ReplayAction.Execute(Command(), Principal), ReplayAction.Execute(Command(), Principal) };
            var manifest = new ReplayManifest(new string('a', 64));
            RuleRegistry Rules() => new RuleRegistry(new[] { new TransferResourceRule() });
            var seen = new List<ReplayObservation>();
            var recorded = ReplayHarness.Record(initial, actions, manifest, Rules(), observer: seen.Add);
            var plain = ReplayHarness.Record(initial, actions, manifest, Rules());
            Assert.That(ReplayArtifactCodec.Encode(recorded), Is.EqualTo(ReplayArtifactCodec.Encode(plain)));
            Assert.That(seen.Count, Is.EqualTo(3));
            Assert.That(seen[0].ErrorCode, Is.EqualTo("NO_ADVANCE_WORKFLOW"));
            Assert.That(seen[1].Before.World.GetLot(Source).OnHand, Is.EqualTo(10));
            Assert.That(seen[1].After.World.GetLot(Source).OnHand, Is.EqualTo(8));
            Assert.That(StateDiffer.Compare(seen[2].Before, seen[2].After).TotalChanges, Is.Zero);
            seen.Clear();
            Assert.That(ReplayHarness.Replay(recorded, manifest, Rules(), observer: seen.Add).Success, Is.True);
            Assert.That(seen.Select(x => x.Index), Is.EqualTo(new[] { 0, 1, 2 }));
        }

        [Test]
        public void FND_15_FailedScheduledTransferShowsRecordedCancellationReason()
        {
            var p = Processor(); Schedule(p);
            p.Execute(Command("consume", 2, payload: Payload(9)), Principal);
            p.Execute(Command("advance", 3, "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = 10L }), Principal);
            p.DrainAdvance(Principal);
            var inspection = SchedulerInspector.Inspect(p.InspectCommitted()).Single();
            Assert.That(inspection.Job.Status, Is.EqualTo(ScheduledJobStatus.Cancelled));
            Assert.That(inspection.CancellationReasonStatus, Is.EqualTo("recorded"));
            Assert.That(inspection.CancellationReason, Is.EqualTo("INSUFFICIENT_RESOURCE"));
        }

    }
}
