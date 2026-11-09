using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using Game.Application.Origins;
using Game.Infrastructure.Origins;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Repair;
using NUnit.Framework;

namespace Game.Tests
{
    public sealed class OriginCampaignPersistenceTests
    {
        private string root = "";
        private const string RequestA = "0000000000000000000000000000000a";
        private const string RequestB = "0000000000000000000000000000000b";
        private OriginCohortRegistry registry = null!;
        [SetUp] public void Setup()
        { root = Path.Combine(Path.GetTempPath(), "mythical-s16-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root); registry = OriginFixture.Registry(); }
        [TearDown] public void Cleanup() { if (Directory.Exists(root)) Directory.Delete(root, true); }
        private OriginCampaignGateway Gateway(Func<string>? seed = null, Action<string>? probe = null)
            => new OriginCampaignGateway(root, registry, seed ?? (() => OriginFixture.SeedA), probe);
        private string[] Files() => Directory.GetFiles(root, "origin-*.db").OrderBy(x => x, StringComparer.Ordinal).ToArray();
        private SqliteWorldStore Store(string path) => new SqliteWorldStore(path, registry.ContentHash, expectedRuleVersion: SqliteWorldStore.OriginRuleVersion);

        [Test]
        public void S16_02_NewRevealReadSaveReopenContinueNeverRerollsOrDraws()
        {
            int entropyCalls = 0; var gateway = Gateway(() => { entropyCalls++; return OriginFixture.SeedA; });
            Assert.That(gateway.ReadMenu().HasCampaign, Is.False); Assert.That(gateway.Continue().Result.Code, Is.EqualTo("no_campaign"));
            var opened = gateway.NewCampaign(RequestA, CancellationToken.None);
            Assert.That(opened.Result.Success && opened.Result.Durable, Is.True);
            var before = Store(Files().Single()).Load(); var view = JsonSerializer.Serialize(opened.Session!.Read());
            for (int i = 0; i < 5; i++) Assert.That(JsonSerializer.Serialize(opened.Session.Read()), Is.EqualTo(view));
            Assert.That(opened.Session.Save().Success && opened.Session.Reload().Success, Is.True);
            var resumed = Gateway(() => throw new Exception("Resume may not ask for entropy")).Continue();
            Assert.That(resumed.Result.Success, Is.True); Assert.That(JsonSerializer.Serialize(resumed.Session!.Read()), Is.EqualTo(view));
            var after = Store(Files().Single()).Load();
            Assert.That(CommittedWorldCodec.Encode(after), Is.EqualTo(CommittedWorldCodec.Encode(before)));
            Assert.That(entropyCalls, Is.EqualTo(1)); Assert.That(after.World.RngStates.Single().State.DrawIndex, Is.EqualTo(before.World.Origin!.DrawIndex));
            Assert.That(after.World.Repair, Is.Null); Assert.That(after.World.Lots, Is.Empty);
            Assert.That(after.World.GetActor(after.World.Origin!.Principal).ControlledOwnerIds.Count, Is.EqualTo(1));
        }
        [Test]
        public void S16_02_NewIsIdempotentAndExplicitOtherCampaignKeepsFirst()
        {
            int entropyCalls = 0;
            var gateway = Gateway(() => ++entropyCalls == 1 ? OriginFixture.SeedA : OriginFixture.SeedB);
            var first = gateway.NewCampaign(RequestA, CancellationToken.None); var firstPath = Files().Single(); var oldBytes = File.ReadAllBytes(firstPath);
            Assert.That(gateway.NewCampaign(RequestA, CancellationToken.None).Session!.Read().ActorName, Is.EqualTo(first.Session!.Read().ActorName));
            Assert.That(entropyCalls, Is.EqualTo(1)); Assert.That(Files().Length, Is.EqualTo(1));
            var second = gateway.NewCampaign(RequestB, CancellationToken.None); Assert.That(second.Result.Success, Is.True);
            Assert.That(Files().Length, Is.EqualTo(2)); Assert.That(entropyCalls, Is.EqualTo(2));
            Assert.That(File.ReadAllBytes(firstPath), Is.EqualTo(oldBytes));
            Assert.That(gateway.Continue().Session!.Read().ActorName, Is.EqualTo(second.Session!.Read().ActorName));
            Assert.That(Store(Files()[0]).Load().World.WorldId, Is.Not.EqualTo(Store(Files()[1]).Load().World.WorldId));
        }
        [Test]
        public void S16_02_AdvanceUsesDurableOriginalEnvelopeAcrossReloadAndRejectsChangedIntent()
        {
            var session = Gateway().NewCampaign(RequestA, CancellationToken.None).Session!;
            Assert.That(session.AdvanceBy(1, "wait-one").Success, Is.True); Assert.That(session.Read().Tick, Is.EqualTo(1));
            session = Gateway().Continue().Session!;
            Assert.That(session.AdvanceBy(1, "wait-one").Success, Is.True); Assert.That(session.Read().Tick, Is.EqualTo(1));
            Assert.That(session.AdvanceBy(60, "wait-one").Code, Is.EqualTo("request_mismatch"));
            Assert.That(session.Read().Tick, Is.EqualTo(1));
            Assert.That(session.AdvanceBy(60, "wait-next").Success, Is.True);
            Assert.That(Gateway().Continue().Session!.Read().Tick, Is.EqualTo(61));
        }
        [TestCase(0, "valid")][TestCase(1441, "valid")][TestCase(1, "../escape")][TestCase(1, "")]
        public void S16_02_InvalidAdvanceDoesNotMutate(long minutes, string request)
        {
            var s = Gateway().NewCampaign(RequestA, CancellationToken.None).Session!;
            var before = File.ReadAllBytes(Files().Single());
            Assert.That(s.AdvanceBy(minutes, request).Code, Is.EqualTo("invalid_request"));
            Assert.That(File.ReadAllBytes(Files().Single()), Is.EqualTo(before));
        }
        [TestCase("before_initialize")][TestCase("after_initialize")][TestCase("before_publish")]
        public void S16_02_FaultBeforePublicationNeverChangesContinueOrPublishesHalfSave(string phase)
        {
            var first = Gateway().NewCampaign(RequestA, CancellationToken.None).Session!.Read().ActorName;
            var bytes = File.ReadAllBytes(Files().Single());
            var result = Gateway(() => OriginFixture.SeedB, p => { if (p == phase) throw new IOException("private diagnostic must not escape"); })
                .NewCampaign(RequestB, CancellationToken.None);
            Assert.That(result.Result.Code, Is.EqualTo("storage_error")); Assert.That(result.Session, Is.Null);
            Assert.That(Files().Length, Is.EqualTo(1)); Assert.That(File.ReadAllBytes(Files().Single()), Is.EqualTo(bytes));
            Assert.That(Gateway().Continue().Session!.Read().ActorName, Is.EqualTo(first));
            Assert.That(Directory.GetFiles(root, ".pending-*.db"), Is.Empty);
            Assert.That(JsonSerializer.Serialize(result), Does.Not.Contain("private diagnostic").And.Not.Contain(root));
        }
        [TestCase("before_initialize")][TestCase("after_initialize")][TestCase("before_publish")]
        public void S16_02_CancellationBeforePublicationIsAtomic(string phase)
        {
            using var cancellation = new CancellationTokenSource();
            var result = Gateway(probe: p => { if (p == phase) cancellation.Cancel(); }).NewCampaign(RequestA, cancellation.Token);
            Assert.That(result.Result.Code, Is.EqualTo("cancelled")); Assert.That(Files(), Is.Empty);
            Assert.That(Gateway().ReadMenu().HasCampaign, Is.False); Assert.That(Directory.GetFiles(root, ".pending-*.db"), Is.Empty);
        }
        [Test]
        public void S16_02_CancellationAfterPublishKeepsCommittedCampaign()
        {
            using var cancellation = new CancellationTokenSource();
            var result = Gateway(probe: p => { if (p == "after_publish") cancellation.Cancel(); }).NewCampaign(RequestA, cancellation.Token);
            Assert.That(result.Result.Success, Is.True); Assert.That(Files().Length, Is.EqualTo(1));
            Assert.That(Gateway().Continue().Session!.Read().ActorName, Is.EqualTo(result.Session!.Read().ActorName));
        }
        [Test]
        public void S16_02_FaultAfterPublishCanRetrySameRequestWithoutSecondEntropyDraw()
        {
            int calls = 0;
            var gateway = Gateway(() => { calls++; return OriginFixture.SeedA; }, p => { if (p == "after_publish") throw new IOException("ambiguous"); });
            Assert.That(gateway.NewCampaign(RequestA, CancellationToken.None).Result.Code, Is.EqualTo("storage_error"));
            Assert.That(Files().Length, Is.EqualTo(1));
            Assert.That(gateway.NewCampaign(RequestA, CancellationToken.None).Result.Success, Is.True);
            Assert.That(calls, Is.EqualTo(1)); Assert.That(Files().Length, Is.EqualTo(1));
        }
        [Test]
        public void S16_02_LockRefusalDoesNotConsumeEntropyOrTouchCampaign()
        {
            int calls = 0;
            using (var writer = new FileStream(Path.Combine(root, ".origin-create.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                Assert.That(Gateway(() => { calls++; return OriginFixture.SeedA; }).NewCampaign(RequestA, CancellationToken.None).Result.Code, Is.EqualTo("storage_error"));
            Assert.That(calls, Is.Zero); Assert.That(Files(), Is.Empty);
        }
        [Test]
        public void S16_02_CrashPendingAndUnrelatedFilesAreIgnoredNeverDeleted()
        {
            var pending = Path.Combine(root, ".pending-foreign.db"); var unrelated = Path.Combine(root, "notes.txt");
            File.WriteAllText(pending, "foreign pending"); File.WriteAllText(unrelated, "user content");
            Assert.That(Gateway().ReadMenu().HasCampaign, Is.False);
            Assert.That(Gateway().NewCampaign(RequestA, CancellationToken.None).Result.Success, Is.True);
            Assert.That(File.ReadAllText(pending), Is.EqualTo("foreign pending")); Assert.That(File.ReadAllText(unrelated), Is.EqualTo("user content"));
        }
        [Test]
        public void S16_02_CorruptLatestFailsClosedWithoutFallingBackOrOverwriting()
        {
            Gateway().NewCampaign(RequestA, CancellationToken.None);
            var corrupt = Path.Combine(root, "origin-0000000002-" + RequestB + ".db"); var bytes = new byte[] { 7, 2, 6, 9 };
            File.WriteAllBytes(corrupt, bytes);
            Assert.That(Gateway().Continue().Result.Code, Is.EqualTo("storage_error"));
            Assert.That(Gateway().ReadMenu().HasCampaign, Is.False);
            Assert.That(Gateway().ReadMenu().Code, Is.EqualTo("storage_error"));
            Assert.That(Gateway().NewCampaign(RequestB, CancellationToken.None).Result.Code, Is.EqualTo("storage_error"));
            Assert.That(File.ReadAllBytes(corrupt), Is.EqualTo(bytes)); Assert.That(Files().Length, Is.EqualTo(2));
        }
        [Test]
        public void S16_02_ForeignRepairFixtureIsPreservedAndOldFactoryRefusesOrigin()
        {
            var foreign = Path.Combine(root, "origin-0000000001-" + RequestA + ".db");
            RepairDemoFactory.Open(foreign, MythicalDnd.Foundation.Tests.BoatRepairTestFixture.Registry());
            var bytes = File.ReadAllBytes(foreign);
            Assert.That(Gateway().Continue().Result.Code, Is.EqualTo("storage_error"));
            Assert.That(File.ReadAllBytes(foreign), Is.EqualTo(bytes));
            var created = Gateway().NewCampaign(RequestB, CancellationToken.None);
            Assert.That(created.Result.Success, Is.True);
            var path = Files()[1]; var originBytes = File.ReadAllBytes(path);
            Assert.Catch(() => RepairDemoFactory.Open(path, MythicalDnd.Foundation.Tests.BoatRepairTestFixture.Registry()));
            Assert.That(File.ReadAllBytes(path), Is.EqualTo(originBytes));
        }
        [TestCase("sequence")][TestCase("request")][TestCase("zero")]
        public void S16_02_AmbiguousCatalogFailsClosedWithoutChoosingLexically(string mutation)
        {
            Gateway().NewCampaign(RequestA, CancellationToken.None);
            var first = Files().Single(); var bytes = File.ReadAllBytes(first);
            var name = mutation == "sequence" ? "origin-0000000001-" + RequestB + ".db"
                : mutation == "request" ? "origin-0000000002-" + RequestA + ".db"
                : "origin-0000000000-" + RequestB + ".db";
            File.Copy(first, Path.Combine(root, name));
            Assert.That(Gateway().ReadMenu().HasCampaign, Is.False);
            Assert.That(Gateway().Continue().Result.Code, Is.EqualTo("storage_error"));
            Assert.That(Gateway().NewCampaign("0000000000000000000000000000000c", CancellationToken.None).Result.Code, Is.EqualTo("storage_error"));
            Assert.That(File.ReadAllBytes(first), Is.EqualTo(bytes)); Assert.That(Files().Length, Is.EqualTo(2));
        }
        [Test]
        public void S16_02_ChangedCohortOrSourceHashCannotAdoptPublishedSave()
        {
            Gateway().NewCampaign(RequestA, CancellationToken.None); var bytes = File.ReadAllBytes(Files().Single());
            var document = OriginFixture.Document(); document.source_hash = new string('f', 64);
            var other = new OriginCampaignGateway(root, OriginCohortCompiler.Compile(document), () => throw new Exception("No entropy"));
            Assert.That(other.Continue().Result.Code, Is.EqualTo("storage_error"));
            Assert.That(File.ReadAllBytes(Files().Single()), Is.EqualTo(bytes));
        }
        [Test]
        public void S16_02_ImageThreeRoundtripUnknownFieldsAndRecoveryPreserveOrigin()
        {
            var session = Gateway().NewCampaign(RequestA, CancellationToken.None).Session!;
            session.AdvanceBy(60, "wait"); session.Save();
            var store = Store(Files().Single()); var state = store.Load(); var bytes = CommittedWorldCodec.Encode(state);
            Assert.That(Encoding.UTF8.GetString(bytes), Does.Contain("s13:image_versioni3;"));
            Assert.That(CommittedWorldCodec.Encode(CommittedWorldCodec.Decode(bytes)), Is.EqualTo(bytes));
            var text = Encoding.UTF8.GetString(bytes); var colon = text.IndexOf(':', 5);
            var count = int.Parse(text.Substring(5, colon - 5));
            var unknown = text.Substring(0, 5) + (count + 1) + text.Substring(colon) + "s7:unknownn";
            Assert.Throws<FormatException>(() => CommittedWorldCodec.Decode(Encoding.UTF8.GetBytes(unknown)));
            var recovered = store.RecoverToNewFile(Path.Combine(root, "recovered.db"));
            Assert.That(CommittedWorldCodec.Encode(recovered), Is.EqualTo(bytes)); Assert.That(recovered.World.Origin!.ActorDefinitionId, Is.EqualTo(state.World.Origin!.ActorDefinitionId));
        }
        [Test]
        public void S16_02_StorageFailureIsSanitizedAndExistingSessionReadRemainsUsable()
        {
            var session = Gateway().NewCampaign(RequestA, CancellationToken.None).Session!;
            var view = session.Read(); File.WriteAllText(Files().Single(), "not sqlite");
            Assert.That(session.Reload().Code, Is.EqualTo("storage_error"));
            Assert.That(session.Read().ActorName, Is.EqualTo(view.ActorName));
            Assert.That(session.Read().Tick, Is.EqualTo(view.Tick));
        }
        [TestCase("../escape")][TestCase("not-32-hex")][TestCase("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")]
        public void S16_02_InvalidCampaignRequestHasNoFilesystemEffects(string request)
        {
            Assert.That(Gateway().NewCampaign(request, CancellationToken.None).Result.Code, Is.EqualTo("invalid_request"));
            Assert.That(Directory.GetFiles(root), Is.Empty);
        }
    }
}
