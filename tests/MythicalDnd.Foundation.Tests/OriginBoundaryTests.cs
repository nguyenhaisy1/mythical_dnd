using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Domain.Origins;
using Game.Foundation.Random;
using Game.Infrastructure.Origins;
using Game.Infrastructure.Persistence;
using NUnit.Framework;

namespace Game.Tests
{
    /// <summary>Independent reviewer cases: genesis assignment cannot be rewritten by a later durable commit.</summary>
    public sealed class OriginBoundaryTests
    {
        [TestCase("actor")]
        [TestCase("household")]
        [TestCase("source")]
        [TestCase("draw")]
        public void S16_02_DurableCommitCannotRewriteGenesisOrigin(string mutation)
        {
            var root = Path.Combine(Path.GetTempPath(), "mythical-s16-independent-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);
            try
            {
                var registry = OriginFixture.Registry();
                var gateway = new OriginCampaignGateway(root, registry, () => OriginFixture.SeedA);
                var opened = gateway.NewCampaign("00000000000000000000000000000001", CancellationToken.None);
                Assert.That(opened.Result.Success, Is.True);
                var path = Directory.GetFiles(root, "origin-*.db").Single();
                var store = new SqliteWorldStore(path, registry.ContentHash, expectedRuleVersion: SqliteWorldStore.OriginRuleVersion);
                var before = store.Load(); var original = before.World.Origin!;
                Assert.That(original.ActorDefinitionId, Is.EqualTo("actor.vac-lam@1"));
                var processor = new CommandProcessor(before, new RuleRegistry(new[] { new TransferResourceRule() }));
                var principal = new ExecutionPrincipal(original.Principal, CommandSource.Player);
                var envelope = new CommandEnvelope("boundary/wait", "AdvanceTime", original.Principal.ToString(), "Player",
                    before.World.Tick.Value, before.World.Revision.Value, 1,
                    new Dictionary<string, object?> { ["target_tick"] = 1L }, "boundary");
                var receipt = processor.Execute(envelope, principal);
                Assert.That(receipt.Outcome, Is.Not.EqualTo(CommandOutcome.Rejected));
                if (receipt.Outcome == CommandOutcome.AcceptedPending) processor.DrainAdvance(principal);
                var candidate = processor.InspectCommitted();
                var raw = Encoding.UTF8.GetString(CommittedWorldCodec.Encode(candidate));
                string Wire(string value) => "s" + Encoding.UTF8.GetByteCount(value).ToString(CultureInfo.InvariantCulture) + ":" + value;
                if (mutation == "actor") raw = raw.Replace(Wire(original.ActorDefinitionId), Wire("actor.vac-nhu@1"));
                if (mutation == "household") raw = raw.Replace(Wire(original.HouseholdDefinitionId), Wire("household.ben-sen@1"));
                if (mutation == "source") raw = raw.Replace(Wire(original.SourceContentHash), Wire(new string('f', 64)));
                if (mutation == "draw") raw = raw.Replace("s4:drawi" + original.DrawIndex.ToString(CultureInfo.InvariantCulture) + ";",
                    "s4:drawi" + (original.DrawIndex + 1).ToString(CultureInfo.InvariantCulture) + ";");
                var forged = CommittedWorldCodec.Decode(Encoding.UTF8.GetBytes(raw));
                Assert.That(forged.World.Origin, Is.Not.Null);
                Assert.That(CommittedWorldCodec.Encode(forged), Is.Not.EqualTo(CommittedWorldCodec.Encode(candidate)));
                // Establish that this is an otherwise valid next image, not a malformed-image rejection.
                Assert.That(CommittedWorldCodec.Encode(CommittedWorldCodec.Decode(CommittedWorldCodec.Encode(forged))),
                    Is.EqualTo(CommittedWorldCodec.Encode(forged)));
                var bytesBefore = File.ReadAllBytes(path);
                Assert.Throws<InvalidDataException>(() => store.Commit(before, forged));
                Assert.That(File.ReadAllBytes(path), Is.EqualTo(bytesBefore));
                Assert.That(gateway.Continue().Result.Success, Is.True);
            }
            finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
        }
    }
}
