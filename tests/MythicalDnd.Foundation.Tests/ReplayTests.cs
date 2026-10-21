using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Game.Application.CommandPipeline;
using Game.Application.SimulationScheduler;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Foundation.Time;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Replay;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class ReplayTests
    {
        private static readonly string Content = new string('a', 64);
        private static ReplayManifest Manifest => new ReplayManifest(Content, "transfer-resource-v1+scheduler-v1");
        private static EntityId<Actor> ActorId => EntityId<Actor>.Parse("w0000000000000001/actor/0000000000000001");
        private static ExecutionPrincipal Principal => new ExecutionPrincipal(ActorId, CommandSource.Player);
        private static RuleRegistry Rules(IRuleHandler? rule = null) => new RuleRegistry(new[] { rule ?? new TransferResourceRule() });
        private static CommittedWorld Initial()
        {
            var ids = IdentitySnapshot.Empty(1).BeginStage(1);
            var actor = ids.Allocate<Actor>(); var other = ids.Allocate<Actor>();
            var location = ids.Allocate<Settlement>(); var a = ids.Allocate<Lot>(); var b = ids.Allocate<Lot>();
            var rng = new RngStreamState("replay", new RngState(new RngContext(new string('0', 64), "replay", "transfer", "one", actor.Value, "main"), 7));
            var world = new WorldSnapshot(ids.PrepareSnapshot(), new WorldTick(0),
                new[] { new ActorState(actor, true, location), new ActorState(other, true, location) },
                new[] { new ResourceLot(a, DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 10, 0),
                    new ResourceLot(b, DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 0, 0) }, new[] { rng });
            return new CommandProcessor(world, Rules()).InspectCommitted();
        }
        private static Dictionary<string, object?> Payload(long amount = 3) => new Dictionary<string, object?> {
            ["source_lot_id"] = "w0000000000000001/lot/0000000000000001",
            ["destination_lot_id"] = "w0000000000000001/lot/0000000000000002", ["quantity"] = amount };
        private static CommandEnvelope Command(string id = "transfer", ulong revision = 1, string type = "TransferResource", Dictionary<string, object?>? payload = null)
            => new CommandEnvelope(id, type, ActorId.ToString(), "Player", 0, revision, 1, payload ?? Payload(), "replay.test");
        private static ReplayAction Execute(CommandEnvelope command) => ReplayAction.Execute(command, Principal);
        private static ReplayArtifact Record(params ReplayAction[] actions) => ReplayHarness.Record(Initial(), actions, Manifest, Rules());
        private sealed class CountingRule : IRuleHandler
        {
            public int Calls { get; private set; }
            public bool Reject { get; set; }
            public bool Throw { get; set; }
            public string CommandType => "TransferResource";
            public int PayloadVersion => 1;
            public Type PayloadType => typeof(TransferResource);
            public RuleResult Evaluate(WorldSnapshot world, EntityId<Actor> issuer, IKernelCommand command)
            {
                Calls++;
                if (Throw) throw new InvalidOperationException("Unexpected rule defect");
                return Reject ? RuleResult.Reject("TEST_CHANGED_RULE") : new TransferResourceRule().Evaluate(world, issuer, command);
            }
        }
        [Test]
        public void FND_13_ReplayActuallyExecutesRulesAndPreservesInitialRng()
        {
            var initial = Initial(); var before = CommittedWorldCodec.Encode(initial); var rule = new CountingRule();
            var artifact = ReplayHarness.Record(initial, new[] { Execute(Command()) }, Manifest, Rules(rule));
            Assert.That(rule.Calls, Is.EqualTo(1));
            var report = ReplayHarness.Replay(ReplayArtifactCodec.Decode(ReplayArtifactCodec.Encode(artifact)), Manifest, Rules(rule));
            Assert.That(rule.Calls, Is.EqualTo(2), "Replay must call the rule again rather than load the recorded final image");
            Assert.That(report.Success, Is.True); Assert.That(report.Divergence, Is.Null);
            Assert.That(report.FinalState.World.Lots[0].OnHand, Is.EqualTo(7));
            Assert.That(report.FinalState.World.Revision.Value, Is.EqualTo(2));
            Assert.That(report.FinalState.World.RngStates[0].State.DrawIndex, Is.EqualTo(7), "Transfer does not draw RNG; only preservation is proven");
            Assert.That(CommittedWorldCodec.Encode(initial), Is.EqualTo(before));
        }
        [Test]
        public void FND_13_ChangedRuleReportsFirstDivergentAttemptAndStops()
        {
            var artifact = Record(Execute(Command("metadata", type: "Unknown")), Execute(Command()), Execute(Command("later", 2)));
            var rule = new CountingRule { Reject = true };
            var report = ReplayHarness.Replay(artifact, Manifest, Rules(rule));
            Assert.That(report.Success, Is.False); Assert.That(rule.Calls, Is.EqualTo(1));
            var diff = report.Divergence!;
            Assert.That(diff.Index, Is.EqualTo(1)); Assert.That(diff.Category, Is.EqualTo("result"));
            Assert.That(diff.ExpectedRevision, Is.EqualTo(2)); Assert.That(diff.ActualRevision, Is.EqualTo(1));
            Assert.That(diff.ExpectedHash, Is.Not.EqualTo(diff.ActualHash));
            Assert.That(report.FinalState.World.Lots[0].OnHand, Is.EqualTo(10));
        }
        [TestCase("content")][TestCase("rule")][TestCase("codec")][TestCase("rng")][TestCase("artifact")]
        public void FND_13_IncompatibleManifestFailsBeforeAnyRuleExecutes(string field)
        {
            var artifact = Record(Execute(Command())); var rule = new CountingRule();
            var expected = new ReplayManifest(field == "content" ? new string('b', 64) : Content,
                field == "rule" ? "other-rule" : Manifest.RuleVersion, field == "artifact" ? 2 : 1,
                field == "codec" ? "mdc2" : "mdc1", field == "rng" ? "other-rng" : "sha256-counter-v1");
            Assert.Throws<ReplayCompatibilityException>(() => ReplayHarness.Replay(artifact, expected, Rules(rule)));
            Assert.That(rule.Calls, Is.Zero);
        }
        [Test]
        public void FND_13_AllAttemptsRetainDuplicateMismatchStaleRejectionNoOpAndUnauthorized()
        {
            var accepted = Command(); var noop = Payload(); noop["destination_lot_id"] = noop["source_lot_id"];
            var wrong = new ExecutionPrincipal(EntityId<Actor>.Parse("w0000000000000001/actor/0000000000000002"), CommandSource.Player);
            var artifact = Record(Execute(accepted), Execute(accepted), Execute(Command(payload: Payload(4))),
                Execute(Command("stale")), Execute(Command("reject", 2, "Unknown")),
                Execute(Command("noop", 2, payload: noop)), ReplayAction.Execute(Command("unauthorized", 2), wrong));
            Assert.That(artifact.Actions.Count, Is.EqualTo(7));
            var roundtrip = ReplayArtifactCodec.Decode(ReplayArtifactCodec.Encode(artifact));
            var report = ReplayHarness.Replay(roundtrip, Manifest, Rules());
            Assert.That(report.Success, Is.True); var final = report.FinalState;
            Assert.That(final.World.Revision.Value, Is.EqualTo(2)); Assert.That(final.World.Lots[0].OnHand, Is.EqualTo(7));
            Assert.That(final.AcceptedReceipts.Count, Is.EqualTo(1)); Assert.That(final.MetadataReceipts.Count, Is.EqualTo(3));
            Assert.That(final.MetadataReceipts.Single(x => x.CommandId == "stale").ErrorCode, Is.EqualTo("STALE_REVISION"));
            Assert.That(final.MetadataReceipts.Single(x => x.CommandId == "reject").ErrorCode, Is.EqualTo("COMMAND_UNKNOWN"));
            Assert.That(final.MetadataReceipts.Single(x => x.CommandId == "noop").Outcome, Is.EqualTo(CommandOutcome.NoOp));
            Assert.That(artifact.Checkpoints[1].IntegrityHash, Is.EqualTo(artifact.Checkpoints[0].IntegrityHash));
            Assert.That(artifact.Checkpoints[2].IntegrityHash, Is.EqualTo(artifact.Checkpoints[1].IntegrityHash));
            Assert.That(artifact.Checkpoints[2].ResultHash, Is.Not.EqualTo(artifact.Checkpoints[1].ResultHash));
            Assert.That(artifact.Checkpoints[6].IntegrityHash, Is.EqualTo(artifact.Checkpoints[5].IntegrityHash));
        }
        [Test]
        public void FND_13_MetadataChangesIntegrityButNotGameplayJournalOrJobs()
        {
            var artifact = Record(Execute(Command("one", type: "Unknown")), Execute(Command("two", type: "Unknown")));
            var first = artifact.Checkpoints[0]; var second = artifact.Checkpoints[1];
            Assert.That(second.Revision, Is.EqualTo(first.Revision)); Assert.That(second.GameplayHash, Is.EqualTo(first.GameplayHash));
            Assert.That(second.JournalHash, Is.EqualTo(first.JournalHash)); Assert.That(second.JobsHash, Is.EqualTo(first.JobsHash));
            Assert.That(second.IntegrityHash, Is.Not.EqualTo(first.IntegrityHash));
            Assert.That(ReplayHarness.Replay(artifact, Manifest, Rules()).Success, Is.True);
        }
        [Test]
        public void FND_13_SchedulerBudgetPauseResumeReplaysJobOrder()
        {
            var a = Payload(); a["due_tick"] = 10L; a["phase"] = 3; a["priority"] = 100;
            var b = Payload(); b["due_tick"] = 11L; b["phase"] = 3; b["priority"] = 100;
            var actions = new[] { Execute(Command("job.a", 1, "ScheduleTransfer", a)), Execute(Command("job.b", 2, "ScheduleTransfer", b)),
                Execute(Command("advance", 3, "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = 20L })),
                ReplayAction.Drain(Principal, 1), ReplayAction.Drain(Principal, 1) };
            var artifact = Record(actions);
            var paused = ReplayHarness.Replay(new ReplayArtifact(artifact.Manifest, artifact.InitialImage, artifact.Actions.Take(4), artifact.Checkpoints.Take(4)), Manifest, Rules());
            Assert.That(paused.Success, Is.True); Assert.That(paused.FinalState.Scheduler.Workflow!.Status, Is.EqualTo(AdvanceStatus.PausedBudget));
            var replay = ReplayHarness.Replay(ReplayArtifactCodec.Decode(ReplayArtifactCodec.Encode(artifact)), Manifest, Rules());
            Assert.That(replay.Success, Is.True); Assert.That(replay.FinalState.World.Tick.Value, Is.EqualTo(20));
            Assert.That(replay.FinalState.Scheduler.Workflow!.Status, Is.EqualTo(AdvanceStatus.Completed));
            Assert.That(replay.FinalState.Scheduler.Executions.Count, Is.EqualTo(2));
            Assert.That(replay.FinalState.World.Lots[0].OnHand, Is.EqualTo(4));
            Assert.That(artifact.Checkpoints[3].JobsHash, Is.Not.EqualTo(artifact.Checkpoints[4].JobsHash));
        }
        [Test]
        public void FND_13_CultureAndPayloadInsertionOrderDoNotChangeArtifact()
        {
            var original = CultureInfo.CurrentCulture; var ui = CultureInfo.CurrentUICulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR"); CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
                var first = ReplayArtifactCodec.Encode(Record(Execute(Command())));
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("vi-VN"); CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;
                var reversed = Payload().Reverse().ToDictionary(x => x.Key, x => x.Value);
                var second = ReplayArtifactCodec.Encode(Record(Execute(Command(payload: reversed))));
                Assert.That(second, Is.EqualTo(first));
                Assert.That(ReplayHarness.Replay(ReplayArtifactCodec.Decode(first), Manifest, Rules()).Success, Is.True);
            }
            finally { CultureInfo.CurrentCulture = original; CultureInfo.CurrentUICulture = ui; }
        }
        [TestCase("trailing")][TestCase("truncate")][TestCase("depth")][TestCase("length")][TestCase("noncanonical")][TestCase("duplicate")]
        public void FND_13_ArtifactRejectsMalformedOrUnboundedInput(string flaw)
        {
            var bytes = ReplayArtifactCodec.Encode(Record(Execute(Command())));
            if (flaw == "trailing") bytes = bytes.Concat(new byte[] { 0 }).ToArray();
            if (flaw == "truncate") bytes = bytes.Take(bytes.Length - 1).ToArray();
            if (flaw == "depth") bytes = Encoding.ASCII.GetBytes("MDC1" + string.Concat(Enumerable.Repeat("a1:", 50)) + "n");
            if (flaw == "length") bytes = Encoding.ASCII.GetBytes("MDC1a2147483647:n");
            if (flaw == "noncanonical") bytes = Encoding.ASCII.GetBytes("MDC1i01;");
            if (flaw == "duplicate") bytes = Encoding.ASCII.GetBytes("MDC1o2:s1:ans1:an");
            Assert.Catch(() => ReplayArtifactCodec.Decode(bytes));
        }
        private static Dictionary<string, object?> ArtifactMap(ReplayArtifact artifact)
        {
            var reader = typeof(CommittedWorldCodec).Assembly.GetType("Game.Infrastructure.Persistence.MdcReader", true)!;
            return (Dictionary<string, object?>)reader.GetMethod("Decode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .Invoke(null, new object[] { ReplayArtifactCodec.Encode(artifact) })!;
        }
        [Test]
        public void FND_13_ArtifactRejectsUnknownFields()
        {
            var map = ArtifactMap(Record(Execute(Command()))); map["unrecognized"] = true;
            Assert.Catch(() => ReplayArtifactCodec.Decode(CanonicalCodec.Encode(map)));
        }
        [TestCase("result")][TestCase("gameplay")][TestCase("integrity")][TestCase("journal")][TestCase("jobs")]
        public void FND_13_TamperedCheckpointIdentifiesSpecificHashCategory(string category)
        {
            var original = Record(Execute(Command())); var c = original.Checkpoints[0]; var wrong = new string('0', 64);
            var changed = new ReplayCheckpoint(c.Revision, category == "result" ? wrong : c.ResultHash,
                category == "gameplay" ? wrong : c.GameplayHash, category == "integrity" ? wrong : c.IntegrityHash,
                category == "journal" ? wrong : c.JournalHash, category == "jobs" ? wrong : c.JobsHash);
            var artifact = new ReplayArtifact(original.Manifest, original.InitialImage, original.Actions, new[] { changed });
            var report = ReplayHarness.Replay(artifact, Manifest, Rules());
            Assert.That(report.Success, Is.False); Assert.That(report.Divergence!.Index, Is.Zero);
            Assert.That(report.Divergence.Category, Is.EqualTo(category)); Assert.That(report.Divergence.ExpectedHash, Is.EqualTo(wrong));
            Assert.That(report.Divergence.ActualHash, Is.Not.EqualTo(wrong));
        }
        [Test]
        public void FND_13_ActionCountLimitAndInvalidDrainBudgetAreRejected()
        {
            Assert.Throws<ArgumentException>(() => ReplayHarness.Record(Initial(), Enumerable.Repeat(Execute(Command()), 10001), Manifest, Rules()));
            Assert.Throws<ArgumentOutOfRangeException>(() => ReplayAction.Drain(Principal, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => ReplayAction.Drain(Principal, CommandProcessor.MaximumJobsPerInvocation + 1));
        }
        [Test]
        public void FND_13_WorldSeedMismatchFailsBeforeAnyRuleExecutes()
        {
            var expected = new ReplayManifest(Content, Manifest.RuleVersion, worldSeed: new string('1', 64));
            var artifact = Record(Execute(Command())); var rule = new CountingRule();
            Assert.Throws<ReplayCompatibilityException>(() => ReplayHarness.Replay(artifact, expected, Rules(rule)));
            Assert.That(rule.Calls, Is.Zero);
        }
        [Test]
        public void FND_13_UnexpectedRuleExceptionIsNotReportedAsSuccessfulReplay()
        {
            var artifact = Record(Execute(Command())); var rule = new CountingRule { Throw = true };
            var error = Assert.Throws<InvalidOperationException>(() => ReplayHarness.Replay(artifact, Manifest, Rules(rule)));
            Assert.That(error!.Message, Is.EqualTo("Unexpected rule defect")); Assert.That(rule.Calls, Is.EqualTo(1));
        }
        [Test]
        public void FND_13_DrainFailuresReplayWithoutChangingState()
        {
            var wrong = new ExecutionPrincipal(EntityId<Actor>.Parse("w0000000000000001/actor/0000000000000002"), CommandSource.Player);
            var artifact = Record(ReplayAction.Drain(Principal), ReplayAction.Drain(Principal),
                Execute(Command("advance", 1, "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = 20L })),
                ReplayAction.Drain(wrong), ReplayAction.Drain(Principal));
            Assert.That(artifact.Checkpoints[0].Revision, Is.EqualTo(1));
            Assert.That(artifact.Checkpoints[1].ResultHash, Is.EqualTo(artifact.Checkpoints[0].ResultHash));
            Assert.That(artifact.Checkpoints[1].IntegrityHash, Is.EqualTo(artifact.Checkpoints[0].IntegrityHash));
            Assert.That(artifact.Checkpoints[3].IntegrityHash, Is.EqualTo(artifact.Checkpoints[2].IntegrityHash));
            Assert.That(artifact.Checkpoints[3].ResultHash, Is.Not.EqualTo(artifact.Checkpoints[2].ResultHash));
            var replay = ReplayHarness.Replay(ReplayArtifactCodec.Decode(ReplayArtifactCodec.Encode(artifact)), Manifest, Rules());
            Assert.That(replay.Success, Is.True); Assert.That(replay.FinalState.World.Tick.Value, Is.EqualTo(20));
            Assert.That(replay.FinalState.Scheduler.Workflow!.Status, Is.EqualTo(AdvanceStatus.Completed));
        }
        [Test]
        public void FND_13_InitialImageIsDefensivelyCopied()
        {
            var artifact = Record(Execute(Command())); var exposed = artifact.InitialImage; exposed[0] = 0;
            Assert.That(ReplayHarness.Replay(artifact, Manifest, Rules()).Success, Is.True);
        }
    }
}
