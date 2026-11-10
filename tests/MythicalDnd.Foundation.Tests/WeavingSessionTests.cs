using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Application.Weaving;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Weaving;
using NUnit.Framework;

namespace Game.Tests
{
    public sealed class WeavingSessionTests
    {
        private string root = "";
        private WeavingContentRegistry registry = null!;
        [SetUp] public void Setup() { root = Path.Combine(Path.GetTempPath(), "mythical-weaving-session-" + Guid.NewGuid().ToString("N")); registry = WeavingFixture.Registry(); }
        [TearDown] public void Cleanup() { if (Directory.Exists(root)) Directory.Delete(root, true); }
        private WeavingCampaignGateway Gateway(string? seed = null) => new WeavingCampaignGateway(root, registry, () => seed ?? OriginFixture.SeedA);
        private IWeavingCampaignSession New(string? seed = null) => Gateway(seed).NewCampaign(WeavingFixture.Request, CancellationToken.None).Session!;
        private string FilePath() => Directory.GetFiles(root, "origin-*.db").Single();
        private SqliteWorldStore Store(Action<string>? probe = null) => new SqliteWorldStore(FilePath(), registry.ContentHash, probe: probe, expectedRuleVersion: SqliteWorldStore.WeavingRuleVersion);
        [Test]
        public void S17_02_ReadingAndPreviewArePureAndOnlySafeSelfDataLeavesSession()
        {
            var session = New(); var before = File.ReadAllBytes(FilePath()); var tokens = new System.Collections.Generic.HashSet<string>();
            for (int i = 0; i < 32; i++) { _ = session.Read(); _ = session.ReadWork(); var preview = session.Preview(); Assert.That(preview.Allowed, Is.True); Assert.That(tokens.Add(preview.Token!), Is.True); }
            Assert.That(File.ReadAllBytes(FilePath()), Is.EqualTo(before));
            Assert.That(Store().Load().World.RngStates.Single().State.DrawIndex, Is.EqualTo(1));
            var text = JsonSerializer.Serialize(session.ReadWork()) + JsonSerializer.Serialize(session.Read()) + JsonSerializer.Serialize(session.Preview());
            Assert.That(text, Does.Not.Contain("actor.").And.Not.Contain("w206f").And.Not.Contain("Revision").And.Not.Contain("Processor").And.Not.Contain(root).And.Not.Contain("WorldSeed").And.Not.Contain("SourceContentHash"));
        }
        [TestCase(OriginFixture.SeedA, "Vạc Lâm")][TestCase(OriginFixture.SeedB, "Bà Nhu")]
        public void S17_02_StartRunningReopenOriginalRetryCompleteExactlyOnce(string seed, string name)
        {
            var session = New(seed); var token = session.Preview().Token!;
            Assert.That(session.Read().ActorName, Is.EqualTo(name));
            Assert.That(session.Start(token, "start").Success, Is.True);
            Assert.That(session.ReadWork().FiberOnHand, Is.EqualTo(8)); Assert.That(session.ReadWork().FiberReserved, Is.EqualTo(2));
            Assert.That(session.ReadWork().CordOnHand, Is.Zero); Assert.That(session.ReadWork().DueTick, Is.EqualTo(30));
            Assert.That(session.Save().Success, Is.True);
            session = Gateway().Continue().Session!;
            Assert.That(session.Start(token, "start").Success, Is.True);
            Assert.That(session.Start(token + "changed", "start").Code, Is.EqualTo("request_mismatch"));
            Assert.That(session.Cancel("start").Code, Is.EqualTo("request_mismatch"));
            Assert.That(session.AdvanceBy(29, "before").Success, Is.True); Assert.That(session.ReadWork().CordOnHand, Is.Zero);
            Assert.That(session.AdvanceBy(1, "due").Success, Is.True); Assert.That(session.ReadWork().Status, Is.EqualTo("completed"));
            Assert.That(session.ReadWork().FiberOnHand, Is.EqualTo(6)); Assert.That(session.ReadWork().FiberReserved, Is.Zero); Assert.That(session.ReadWork().CordOnHand, Is.EqualTo(1));
            session = Gateway().Continue().Session!;
            Assert.That(session.Start(token, "start").Success, Is.True); Assert.That(session.AdvanceBy(1, "due").Success, Is.True);
            Assert.That(session.Read().Tick, Is.EqualTo(30)); Assert.That(session.ReadWork().CordOnHand, Is.EqualTo(1));
            Assert.That(Store().Load().World.Weaving!.Activities.Count, Is.EqualTo(1));
        }
        [Test]
        public void S17_02_CancelReleasesOwnHoldAndOriginalRetrySurvivesReload()
        {
            var s = New(); Assert.That(s.Start(s.Preview().Token!, "start").Success, Is.True);
            Assert.That(s.AdvanceBy(1, "one").Success, Is.True); Assert.That(s.Cancel("cancel").Success, Is.True);
            Assert.That(s.ReadWork().Status, Is.EqualTo("cancelled")); Assert.That(s.ReadWork().FiberOnHand, Is.EqualTo(8)); Assert.That(s.ReadWork().FiberReserved, Is.Zero);
            s = Gateway().Continue().Session!; Assert.That(s.Cancel("cancel").Success, Is.True);
            Assert.That(s.AdvanceBy(60, "later").Success, Is.True); Assert.That(s.ReadWork().CordOnHand, Is.Zero);
        }
        [Test]
        public void S17_02_UnsupportedOriginCannotStartOrMutateMetadataAndKeepsOtherOwnActions()
        {
            var s = New(string.Concat(Enumerable.Repeat("01234567", 8))); Assert.That(s.Read().ActorName, Is.EqualTo("Bến Kiên"));
            var before = File.ReadAllBytes(FilePath()); Assert.That(s.ReadWork().Qualified, Is.False);
            Assert.That(s.Preview().Code, Is.EqualTo("unsupported_origin")); Assert.That(s.Start("not-a-token", "start").Success, Is.False);
            Assert.That(File.ReadAllBytes(FilePath()), Is.EqualTo(before)); Assert.That(s.Save().Success && s.Reload().Success && s.AdvanceBy(1, "wait").Success, Is.True);
        }
        [Test]
        public void S17_02_StaleOrReloadedPreviewCannotStartAfterClockChanges()
        {
            var s = New(); var token = s.Preview().Token!; Assert.That(s.AdvanceBy(1, "wait").Success, Is.True);
            var before = File.ReadAllBytes(FilePath()); Assert.That(s.Start(token, "start").Code, Is.EqualTo("stale_preview")); Assert.That(File.ReadAllBytes(FilePath()), Is.EqualTo(before));
            token = s.Preview().Token!; Assert.That(s.Reload().Success, Is.True); Assert.That(s.Start(token, "start-reloaded").Code, Is.EqualTo("stale_preview"));
        }
        [Test]
        public void S17_02_FiniteStarterStockCannotProduceUnboundedCord()
        {
            var s = New();
            for (int i = 0; i < 4; i++) { Assert.That(s.Start(s.Preview().Token!, "start-" + i).Success, Is.True); Assert.That(s.AdvanceBy(30, "wait-" + i).Success, Is.True); }
            Assert.That(s.ReadWork().FiberOnHand, Is.Zero); Assert.That(s.ReadWork().CordOnHand, Is.EqualTo(4));
            var before = File.ReadAllBytes(FilePath()); Assert.That(s.Preview().Allowed, Is.False); Assert.That(s.Start("missing", "fifth").Success, Is.False); Assert.That(File.ReadAllBytes(FilePath()), Is.EqualTo(before));
        }
        [TestCase("before_commit")][TestCase("after_commit")]
        public void S17_02_CommitFaultOrResponseLossReconcilesThenRetriesOriginalIntent(string phase)
        {
            _ = New(); bool fail = true;
            var store = Store(point => { if (point == phase && fail) { fail = false; throw new IOException("private storage diagnostic"); } });
            CommandProcessor Restore(CommittedWorld state) => new CommandProcessor(state, new RuleRegistry(new[] { new TransferResourceRule() }), store, weavingRecipe: registry.Recipe);
            var state = store.Load(); var view = registry.Cohort.Get(state.World.Origin!.ActorDefinitionId);
            var s = new WeavingCampaignSession(Restore(state), view.Reveal, store.Load, Restore, store.CreateSnapshot);
            var token = s.Preview().Token!; Assert.That(s.Start(token, "start").Code, Is.EqualTo("session_error"));
            Assert.That(s.Start(token, "start").Success, Is.True);
            var reopened = Gateway().Continue().Session!; Assert.That(reopened.Start(token, "start").Success, Is.True);
            Assert.That(reopened.ReadWork().FiberReserved, Is.EqualTo(2)); Assert.That(store.Load().World.Weaving!.Activities.Count, Is.EqualTo(1));
            Assert.That(JsonSerializer.Serialize(s.ReadWork()), Does.Not.Contain("private storage diagnostic"));
        }
        [TestCase(false, false)][TestCase(true, false)][TestCase(false, true)][TestCase(true, true)]
        public void S17_02_ReopenOrReloadFinishesOnlyPreviouslyAcceptedPendingAdvance(bool afterJob, bool reload)
        {
            var session = New(); Assert.That(session.Start(session.Preview().Token!, "start").Success, Is.True);
            int commits = 0;
            var store = Store(point => { if (point == "after_commit" && ++commits == 2 && afterJob) throw new IOException("crash after job before close"); });
            var before = store.Load(); var actor = before.World.Origin!.Principal;
            var processor = new CommandProcessor(before, new RuleRegistry(new[] { new TransferResourceRule() }), store, weavingRecipe: registry.Recipe);
            var envelope = new CommandEnvelope("weaving-client/crash-wait", "AdvanceTime", actor.ToString(), "Player", 0, before.World.Revision.Value, 1,
                new System.Collections.Generic.Dictionary<string, object?> { ["target_tick"] = 30L }, "weaving-client/advance");
            Assert.That(processor.Execute(envelope, new ExecutionPrincipal(actor, CommandSource.Player)).Outcome.ToString(), Is.EqualTo("AcceptedPending"));
            if (afterJob) Assert.Throws<IOException>(() => processor.DrainAdvance(new ExecutionPrincipal(actor, CommandSource.Player)));
            var interrupted = Store().Load(); Assert.That(interrupted.Scheduler.Workflow!.Status.ToString(), Is.EqualTo("Pending"));
            Assert.That(interrupted.World.GetLot(interrupted.World.Weaving!.OutputLotId).OnHand, Is.EqualTo(afterJob ? 1 : 0));
            if (reload) Assert.That(session.Reload().Success, Is.True); else session = Gateway().Continue().Session!;
            Assert.That(session.Read().Tick, Is.EqualTo(30)); Assert.That(session.ReadWork().CordOnHand, Is.EqualTo(1));
            Assert.That(session.ReadWork().FiberOnHand, Is.EqualTo(6)); Assert.That(session.ReadWork().FiberReserved, Is.Zero);
            Assert.That(Store().Load().Scheduler.Workflow!.Status.ToString(), Is.EqualTo("Completed"));
            var stable = File.ReadAllBytes(FilePath()); Assert.That(session.AdvanceBy(30, "crash-wait").Success, Is.True);
            Assert.That(File.ReadAllBytes(FilePath()), Is.EqualTo(stable)); Assert.That(Store().Load().World.RngStates.Single().State.DrawIndex, Is.EqualTo(1));
        }
        [TestCase(0, "good")][TestCase(1441, "good")][TestCase(1, "../bad")][TestCase(1, "")]
        public void S17_02_InvalidRequestsNeverMutate(long minutes, string request)
        {
            var s = New(); var before = File.ReadAllBytes(FilePath()); Assert.That(s.AdvanceBy(minutes, request).Code, Is.EqualTo("invalid_request")); Assert.That(File.ReadAllBytes(FilePath()), Is.EqualTo(before));
        }
    }
}
