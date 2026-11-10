using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Game.Application.CommandPipeline;
using Game.Foundation.Random;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Weaving;
using NUnit.Framework;

namespace Game.Tests
{
    /// <summary>Independent reviewer cases; reconstruct coherent impossible history, not merely malformed fields.</summary>
    public sealed class WeavingAdversarialTests
    {
        private static Dictionary<string, object?> Image(CommittedWorld state)
        {
            var reader = typeof(CommittedWorldCodec).Assembly.GetType("Game.Infrastructure.Persistence.MdcReader", true)!;
            return (Dictionary<string, object?>)reader.GetMethod("Decode", BindingFlags.Static | BindingFlags.NonPublic)!
                .Invoke(null, new object[] { CommittedWorldCodec.Encode(state) })!;
        }
        private static object? Retarget(object? value, string oldPrefix, string newPrefix)
        {
            if (value is string text) return text.Replace(oldPrefix, newPrefix, StringComparison.Ordinal);
            if (value is Dictionary<string, object?> map)
                return map.ToDictionary(p => p.Key, p => Retarget(p.Value, oldPrefix, newPrefix), StringComparer.Ordinal);
            if (value is IEnumerable<object?> rows) return rows.Select(x => Retarget(x, oldPrefix, newPrefix)).ToArray();
            return value;
        }
        private static void Rehash(Dictionary<string, object?> receipt)
        {
            var e = (Dictionary<string, object?>)receipt["envelope"]!;
            var command = new CommandEnvelope((string)e["id"]!, (string)e["type"]!, (string)e["issuer"]!, (string)e["source"]!,
                (long)e["tick"]!, Convert.ToUInt64(e["expected"]), Convert.ToInt32(e["version"]),
                (Dictionary<string, object?>)e["payload"]!, (string)e["correlation"]!);
            receipt["fingerprint"] = command.Fingerprint();
        }
        [TestCase(false)]
        [TestCase(true)]
        public void S17_04_NonWeaverCannotRestoreCoherentQualifiedHistory(bool completed)
        {
            var folder = Path.Combine(Path.GetTempPath(), "mythical-weaving-review-" + Guid.NewGuid().ToString("N"));
            try
            {
                var registry = WeavingFixture.Registry(); var qualifiedRoot = Path.Combine(folder, "qualified"); var blockedRoot = Path.Combine(folder, "blocked");
                var qualified = new WeavingCampaignGateway(qualifiedRoot, registry, () => OriginFixture.SeedA)
                    .NewCampaign(WeavingFixture.Request, CancellationToken.None).Session!;
                var blocked = new WeavingCampaignGateway(blockedRoot, registry, () => string.Concat(Enumerable.Repeat("01234567", 8)))
                    .NewCampaign(WeavingFixture.Request, CancellationToken.None).Session!;
                Assert.That(blocked.Read().ActorName, Is.EqualTo("Bến Kiên"));
                Assert.That(blocked.ReadWork().SkillValue, Is.Zero);
                Assert.That(blocked.Preview().Allowed, Is.False);
                Assert.That(qualified.Start(qualified.Preview().Token!, "review-start").Success, Is.True);
                if (completed) Assert.That(qualified.AdvanceBy(30, "review-wait").Success, Is.True);
                SqliteWorldStore Store(string root) => new SqliteWorldStore(Directory.GetFiles(root, "origin-*.db").Single(), registry.ContentHash,
                    expectedRuleVersion: SqliteWorldStore.WeavingRuleVersion);
                var valid = Store(qualifiedRoot).Load(); var genesis = Store(blockedRoot).Load();
                var oldPrefix = "w" + valid.World.WorldId.ToString("x16"); var newPrefix = "w" + genesis.World.WorldId.ToString("x16");
                var forged = (Dictionary<string, object?>)Retarget(Image(valid), oldPrefix, newPrefix)!;
                var blockedImage = Image(genesis);
                forged["world"] = blockedImage["world"]; forged["origin"] = blockedImage["origin"]; forged["rng"] = blockedImage["rng"];
                ((Dictionary<string, object?>)forged["weaving"]!)["skill"] = 0L;
                foreach (var item in (IEnumerable<object?>)forged["events"]!)
                {
                    var fact = (Dictionary<string, object?>)item!; var payload = (Dictionary<string, object?>)fact["payload"]!;
                    if (payload.ContainsKey("skill_value")) payload["skill_value"] = 0L;
                }
                foreach (var name in new[] { "accepted", "metadata" })
                    foreach (var item in (IEnumerable<object?>)forged[name]!) Rehash((Dictionary<string, object?>)item!);
                // Identity, jobs, receipt fingerprints, Origin seed binding and every effective skill fact
                // now agree with real Bến Kiên genesis. The impossible part is having ever started weaving.
                Assert.Throws<FormatException>(() => CommittedWorldCodec.Decode(CanonicalCodec.Encode(forged)),
                    "A coherent activity cannot create qualification that this immutable origin never had");
            }
            finally { if (Directory.Exists(folder)) Directory.Delete(folder, true); }
        }
    }
}
