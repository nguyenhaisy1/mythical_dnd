using System;
using System.IO;
using System.Text.Json;
using Game.Infrastructure.Repair;
using NUnit.Framework;
using F = MythicalDnd.Foundation.Tests.BoatRepairTestFixture;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class RepairDemoPersistenceTests
    {
        private string directory = "";
        private string Database => Path.Combine(directory, "client.db");
        [SetUp] public void SetUp() { directory = Path.Combine(Path.GetTempPath(), "mythical-s15-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory); }
        [TearDown] public void TearDown() { if (Directory.Exists(directory)) Directory.Delete(directory, true); }

        [Test]
        public void S15_04_StartSaveReopenRetryAndCompleteUsesDurableRules()
        {
            var registry = F.Registry(); var session = RepairDemoFactory.Open(Database, registry);
            var start = session.ConfirmRepair(session.PreviewRepair().Token!, "start");
            Assert.That(start.Success && start.Durable, Is.True);
            Assert.That(session.Read().TimberOnHand, Is.EqualTo(10)); Assert.That(session.Read().TimberReserved, Is.EqualTo(2));
            Assert.That(session.SaveSnapshot().Success, Is.True);
            session = RepairDemoFactory.Open(Database, registry);
            Assert.That(session.Read().ActivityStatus, Is.EqualTo("running"));
            Assert.That(session.Retry("start").Success, Is.True);
            Assert.That(session.Read().TimberReserved, Is.EqualTo(2));
            Assert.That(session.AdvanceBy(59, "before-due").Success, Is.True);
            Assert.That(session.Read().BoatCondition, Is.EqualTo(500));
            Assert.That(session.AdvanceBy(1, "due").Success, Is.True);
            var done = RepairDemoFactory.Open(Database, registry).Read();
            Assert.That(done.BoatCondition, Is.EqualTo(700)); Assert.That(done.TimberOnHand, Is.EqualTo(8));
            Assert.That(done.TimberReserved, Is.Zero); Assert.That(done.ActivityStatus, Is.EqualTo("completed"));
        }

        [Test]
        public void S15_04_CancelAfterReopenReleasesOnlyHeldMaterial()
        {
            var registry = F.Registry(); var session = RepairDemoFactory.Open(Database, registry);
            session.ConfirmRepair(session.PreviewRepair().Token!, "start");
            session = RepairDemoFactory.Open(Database, registry);
            var result = session.CancelOwnRepair("cancel"); Assert.That(result.Success && result.Durable, Is.True);
            Assert.That(session.Retry("cancel").Success, Is.True);
            var view = RepairDemoFactory.Open(Database, registry).Read();
            Assert.That(view.TimberOnHand, Is.EqualTo(10)); Assert.That(view.TimberReserved, Is.Zero);
            Assert.That(view.BoatCondition, Is.EqualTo(500)); Assert.That(view.ActivityStatus, Is.EqualTo("cancelled"));
        }

        [Test]
        public void S15_04_WaitWithoutStartingRepairDoesNotChangeBoatOrWood()
        {
            var session = RepairDemoFactory.Open(Database, F.Registry());
            Assert.That(session.AdvanceBy(60, "wait").Success, Is.True);
            Assert.That(session.Reload().Success, Is.True);
            var view = session.Read(); Assert.That(view.Tick, Is.EqualTo(60));
            Assert.That(view.BoatCondition, Is.EqualTo(500)); Assert.That(view.TimberOnHand, Is.EqualTo(10));
            Assert.That(view.TimberReserved, Is.Zero); Assert.That(view.ActivityStatus, Is.EqualTo("none"));
        }

        [Test]
        public void S15_04_CompatibleRepairSaveFromOtherFixtureIsRefusedWithoutOverwrite()
        {
            var registry = F.Registry(); var store = F.Store(Database, registry);
            store.InitializeSave(F.Initial(registry), "different-trusted-fixture");
            var before = File.ReadAllBytes(Database);
            Assert.Throws<InvalidDataException>(() => RepairDemoFactory.Open(Database, registry));
            Assert.That(File.ReadAllBytes(Database), Is.EqualTo(before));
        }

        [Test]
        public void S15_04_CorruptFileIsNeverReinitialized()
        {
            var original = new byte[] { 42, 10, 33, 14, 55 }; File.WriteAllBytes(Database, original);
            Assert.Catch(() => RepairDemoFactory.Open(Database, F.Registry()));
            Assert.That(File.ReadAllBytes(Database), Is.EqualTo(original));
        }

        [Test]
        public void S15_04_PrivateFixtureBoatAndInventoryNeverAppearInPlayerDto()
        {
            var session = RepairDemoFactory.Open(Database, F.Registry()); var text = JsonSerializer.Serialize(session.Read());
            Assert.That(text, Does.Not.Contain("917")); Assert.That(text, Does.Not.Contain("321"));
            Assert.That(text, Does.Not.Contain("actor/0000000000000002"));
            Assert.That(text, Does.Not.Contain("WorldSnapshot")); Assert.That(text, Does.Not.Contain("OriginalEnvelope"));
            Assert.That(session.Read().BoatVisible, Is.True); Assert.That(session.Read().BoatCondition, Is.EqualTo(500));
        }
    }
}
