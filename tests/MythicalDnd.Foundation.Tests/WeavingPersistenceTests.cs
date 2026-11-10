using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Foundation.Random;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Weaving;
using Microsoft.Data.Sqlite;
using NUnit.Framework;

namespace Game.Tests
{
    public sealed class WeavingPersistenceTests
    {
        private string root = "";
        private WeavingContentRegistry registry = null!;
        private const string OtherRequest = "0000000000000000000000000000000b";
        [SetUp] public void Setup() { root = Path.Combine(Path.GetTempPath(), "mythical-weaving-save-" + Guid.NewGuid().ToString("N")); registry = WeavingFixture.Registry(); }
        [TearDown] public void Cleanup() { if (Directory.Exists(root)) Directory.Delete(root, true); }
        private WeavingCampaignGateway Gateway(Action<string>? probe = null, Func<string>? seed = null) => new WeavingCampaignGateway(root, registry, seed ?? (() => OriginFixture.SeedA), probe);
        private string PathOfSave() => Directory.GetFiles(root, "origin-*.db").Single();
        private SqliteWorldStore Store(string? path = null) => new SqliteWorldStore(path ?? PathOfSave(), registry.ContentHash, expectedRuleVersion: SqliteWorldStore.WeavingRuleVersion);
        private static object Decode(byte[] bytes)
        {
            var reader=typeof(CommittedWorldCodec).Assembly.GetType("Game.Infrastructure.Persistence.MdcReader",true)!;
            return reader.GetMethod("Decode",System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic)!.Invoke(null,new object[]{bytes})!;
        }
        [Test]
        public void S17_02_NewAtomicIdempotencyAndOwnStarterKeepOriginalCohortHash()
        {
            int calls = 0; var g = Gateway(seed: () => { calls++; return OriginFixture.SeedA; });
            var first = g.NewCampaign(WeavingFixture.Request, CancellationToken.None); Assert.That(first.Result.Success && first.Result.Durable, Is.True);
            var initial = Store().Load(); var bytes = CommittedWorldCodec.Encode(initial);
            Assert.That(g.NewCampaign(WeavingFixture.Request, CancellationToken.None).Result.Success, Is.True); Assert.That(calls, Is.EqualTo(1));
            Assert.That(Directory.GetFiles(root, "origin-*.db").Length, Is.EqualTo(1));
            Assert.That(initial.World.Origin!.CohortContentHash, Is.EqualTo(registry.Cohort.ContentHash));
            Assert.That(initial.World.Weaving!.ContentHash, Is.EqualTo(registry.ContentHash)); Assert.That(initial.World.Lots.Count, Is.EqualTo(2));
            Assert.That(initial.World.Lots.All(l => l.Owner.Equals(initial.World.Origin.Principal.Value)), Is.True);
            Assert.That(initial.World.GetActor(initial.World.Origin.Principal).ControlledOwnerIds.Count, Is.EqualTo(1));
            var image = (Dictionary<string, object?>)Decode(bytes)!; Assert.That(image["image_version"], Is.EqualTo(4));
            using var c = new SqliteConnection("Data Source=" + PathOfSave()); c.Open(); using var command = c.CreateCommand(); command.CommandText = "SELECT schema_version FROM world_meta"; Assert.That((long)command.ExecuteScalar()!, Is.EqualTo(2));
        }
        [TestCase("before_initialize")][TestCase("after_initialize")][TestCase("before_publish")]
        public void S17_02_FaultBeforePublishKeepsExistingContinueAndCleansOnlyOwnPending(string phase)
        {
            var name = Gateway().NewCampaign(WeavingFixture.Request, CancellationToken.None).Session!.Read().ActorName;
            var bytes = File.ReadAllBytes(PathOfSave()); Directory.CreateDirectory(root); var orphan = Path.Combine(root, ".pending-foreign.db"); File.WriteAllText(orphan, "foreign");
            var result = Gateway(p => { if (p == phase) throw new IOException("private failure"); }).NewCampaign(OtherRequest, CancellationToken.None);
            Assert.That(result.Result.Code, Is.EqualTo("storage_error")); Assert.That(File.ReadAllBytes(PathOfSave()), Is.EqualTo(bytes));
            Assert.That(Gateway().Continue().Session!.Read().ActorName, Is.EqualTo(name));
            Assert.That(Directory.GetFiles(root, ".pending-*.db"), Is.EqualTo(new[] { orphan })); Assert.That(File.ReadAllText(orphan), Is.EqualTo("foreign"));
        }
        [TestCase("before_initialize")][TestCase("after_initialize")][TestCase("before_publish")]
        public void S17_02_CancelBeforePublicationCreatesNoActiveCampaign(string phase)
        {
            using var cancel = new CancellationTokenSource();
            var result = Gateway(p => { if (p == phase) cancel.Cancel(); }).NewCampaign(WeavingFixture.Request, cancel.Token);
            Assert.That(result.Result.Code, Is.EqualTo("cancelled")); Assert.That(Gateway().ReadMenu().HasCampaign, Is.False);
            Assert.That(Directory.GetFiles(root, "origin-*.db"), Is.Empty); Assert.That(Directory.GetFiles(root, ".pending-*.db"), Is.Empty);
        }
        [Test]
        public void S17_02_AfterPublishResponseLossRetriesExistingCampaignWithoutDrawingAgain()
        {
            int calls = 0;
            var failed = Gateway(p => { if (p == "after_publish") throw new IOException("lost response"); }, () => { calls++; return OriginFixture.SeedA; });
            Assert.That(failed.NewCampaign(WeavingFixture.Request, CancellationToken.None).Result.Code, Is.EqualTo("storage_error"));
            var retry = Gateway(seed: () => throw new Exception("may not draw"));
            Assert.That(retry.NewCampaign(WeavingFixture.Request, CancellationToken.None).Result.Success, Is.True); Assert.That(calls, Is.EqualTo(1));
        }
        [Test]
        public void S17_02_LatestCorruptCampaignFailsClosedWithoutOverwritingOrFallback()
        {
            _ = Gateway().NewCampaign(WeavingFixture.Request, CancellationToken.None);
            var original = File.ReadAllBytes(PathOfSave()); var latest = Path.Combine(root, "origin-0000000002-" + OtherRequest + ".db"); File.WriteAllText(latest, "corrupt");
            Assert.That(Gateway().Continue().Result.Code, Is.EqualTo("storage_error")); Assert.That(Gateway().ReadMenu().Code, Is.EqualTo("storage_error"));
            Assert.That(File.ReadAllText(latest), Is.EqualTo("corrupt")); Assert.That(File.ReadAllBytes(Directory.GetFiles(root, "origin-0000000001-*.db").Single()), Is.EqualTo(original));
        }
        [TestCase(SqliteWorldStore.LegacyRuleVersion)][TestCase(SqliteWorldStore.RepairRuleVersion)][TestCase(SqliteWorldStore.OriginRuleVersion)]
        public void S17_02_PreviousStoreProfilesRefuseImage4WithoutFileMutation(string rule)
        {
            _ = Gateway().NewCampaign(WeavingFixture.Request, CancellationToken.None); var before = File.ReadAllBytes(PathOfSave());
            Assert.Throws<InvalidDataException>(() => new SqliteWorldStore(PathOfSave(), registry.ContentHash, expectedRuleVersion: rule).Load());
            Assert.That(File.ReadAllBytes(PathOfSave()), Is.EqualTo(before));
        }
        [TestCase("skill")][TestCase("recipe")][TestCase("origin")]
        public void S17_02_DurableCommitPinsGenesisEvenWhenCandidateIsCanonical(string mutation)
        {
            _ = Gateway().NewCampaign(WeavingFixture.Request, CancellationToken.None); var store = Store(); var before = store.Load();
            var image = (Dictionary<string, object?>)Decode(CommittedWorldCodec.Encode(before))!;
            var work = (Dictionary<string, object?>)image["weaving"]!;
            if (mutation == "skill") work["skill"] = 44L;
            if (mutation == "recipe") ((Dictionary<string, object?>)work["recipe"]!)["minimum_skill"] = 41L;
            if (mutation == "origin") ((Dictionary<string, object?>)image["origin"]!)["source_hash"] = new string('a', 64);
            var candidate = CommittedWorldCodec.Decode(CanonicalCodec.Encode(image)); var file = File.ReadAllBytes(PathOfSave());
            Assert.Throws<InvalidDataException>(() => store.Commit(before, candidate)); Assert.That(File.ReadAllBytes(PathOfSave()), Is.EqualTo(file));
        }
        [Test]
        public void S17_02_Image4RejectsUnknownFieldsAndFabricatedMaterialBalance()
        {
            _ = Gateway().NewCampaign(WeavingFixture.Request, CancellationToken.None);
            var image = (Dictionary<string, object?>)Decode(CommittedWorldCodec.Encode(Store().Load()))!;
            ((Dictionary<string, object?>)image["weaving"]!)["unknown"] = "forged";
            Assert.Throws<FormatException>(() => CommittedWorldCodec.Decode(CanonicalCodec.Encode(image)));
            image = (Dictionary<string, object?>)Decode(CommittedWorldCodec.Encode(Store().Load()))!;
            ((Dictionary<string, object?>)((IEnumerable<object?>)image["lots"]!).First()!)["on_hand"] = 800L;
            Assert.Throws<FormatException>(() => CommittedWorldCodec.Decode(CanonicalCodec.Encode(image)));
        }
        [Test]
        public void S17_02_RunningSnapshotRecoveryPreservesHoldAndCompletesOnce()
        {
            var s = Gateway().NewCampaign(WeavingFixture.Request, CancellationToken.None).Session!; Assert.That(s.Start(s.Preview().Token!, "start").Success, Is.True); Assert.That(s.Save().Success, Is.True);
            var destination = Path.Combine(root, "recovered.db"); var recovered = Store().RecoverToNewFile(destination);
            Assert.That(recovered.World.Weaving!.Activities.Single().Status.ToString(), Is.EqualTo("Running"));
            var store = Store(destination); var processor = new CommandProcessor(store.Load(), new RuleRegistry(new[] { new TransferResourceRule() }), store, weavingRecipe: registry.Recipe);
            var actor = recovered.World.Origin!.Principal;
            var command = new CommandEnvelope("recovered-wait", "AdvanceTime", actor.ToString(), "Player", 0, recovered.World.Revision.Value, 1,
                new Dictionary<string, object?> { ["target_tick"] = 30L }, "recovered");
            _ = processor.Execute(command, new ExecutionPrincipal(actor, CommandSource.Player)); processor.DrainAdvance(new ExecutionPrincipal(actor, CommandSource.Player));
            var final = store.Load(); Assert.That(final.World.GetLot(final.World.Weaving!.InputLotId).OnHand, Is.EqualTo(6)); Assert.That(final.World.GetLot(final.World.Weaving.OutputLotId).OnHand, Is.EqualTo(1));
            Assert.That(final.World.RngStates.Single().State.DrawIndex, Is.EqualTo(1));
        }
    }
}
