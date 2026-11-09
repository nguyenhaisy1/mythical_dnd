using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Infrastructure.Origins;
using Game.Infrastructure.SettlementContent;
using NUnit.Framework;

namespace Game.Tests
{
    internal static class OriginFixture
    {
        internal const string SeedA = "0000000000000000000000000000000000000000000000000000000000000000";
        internal const string SeedB = "1111111111111111111111111111111111111111111111111111111111111111";
        private static object? Decode(JsonElement x) => x.ValueKind switch {
            JsonValueKind.Object => x.EnumerateObject().ToDictionary(p => p.Name, p => Decode(p.Value), StringComparer.Ordinal),
            JsonValueKind.Array => x.EnumerateArray().Select(Decode).ToArray(), JsonValueKind.String => x.GetString(),
            JsonValueKind.Number => x.GetInt64(), JsonValueKind.True => true, JsonValueKind.False => false,
            JsonValueKind.Null => null, _ => throw new ArgumentException("fixture") };
        internal static OriginCohortDocument Document()
        {
            using var source = JsonDocument.Parse(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, "settlement-pack.json")));
            return OriginCohortCompiler.FromSettlement(SettlementCompiler.Compile((Dictionary<string, object?>)Decode(source.RootElement)!));
        }
        internal static OriginCohortRegistry Registry() => OriginCohortCompiler.Compile(Document());
    }
    public sealed class OriginCohortTests
    {
        [Test]
        public void S16_02_DerivesOnlyEightAuthoredBenVacAdultsAndExactResourceRoundtrip()
        {
            var d = OriginFixture.Document(); var registry = OriginCohortCompiler.Compile(d);
            Assert.That(d.records.Length, Is.EqualTo(8));
            Assert.That(d.records.All(r => r.alive && r.age >= 18 && r.community_id == "community.ben-vac@1"), Is.True);
            Assert.That(d.records.Any(r => r.actor_id == "actor.vac-nhu@1" && r.age == 77), Is.True);
            Assert.That(d.records.Any(r => r.actor_id == "actor.ben-ngo@1"), Is.False);
            var options = new JsonSerializerOptions { IncludeFields = true };
            var roundtrip = JsonSerializer.Deserialize<OriginCohortDocument>(JsonSerializer.Serialize(d, options), options)!;
            Assert.That(OriginCohortCompiler.Compile(roundtrip).ContentHash, Is.EqualTo(registry.ContentHash));
        }
        [Test]
        public void S16_02_RecordOrderIsCanonicalAndSourceChangesAreCompatibilityChanges()
        {
            var d = OriginFixture.Document(); var hash = OriginCohortCompiler.Compile(d).ContentHash;
            Array.Reverse(d.records); Assert.That(OriginCohortCompiler.Compile(d).ContentHash, Is.EqualTo(hash));
            d.source_hash = new string('f', 64); Assert.That(OriginCohortCompiler.Compile(d).ContentHash, Is.Not.EqualTo(hash));
        }
        [Test]
        public void S16_02_RegistryIsDeepImmutableAndProjectionHasOnlySelfMemory()
        {
            var d = OriginFixture.Document(); var r = OriginCohortCompiler.Compile(d); var hash = r.ContentHash;
            var actor = d.records[0]; var view = r.Get(actor.actor_id).Reveal(0);
            actor.display_name = "tampered"; actor.history[0].description = "tampered"; actor.needs[0] = "tampered"; actor.claims[0].text = "tampered";
            var again = r.Get(actor.actor_id).Reveal(0);
            Assert.That(again.ActorName, Is.EqualTo(view.ActorName));
            Assert.That(again.History[0].Description, Is.EqualTo(view.History[0].Description));
            Assert.That(again.Needs[0], Is.EqualTo(view.Needs[0]));
            Assert.That(again.Claims[0].Text, Is.EqualTo(view.Claims[0].Text));
            Assert.That(r.ContentHash, Is.EqualTo(hash));
            var text = JsonSerializer.Serialize(again);
            Assert.That(text, Does.Not.Contain("prototype-authored").And.Not.Contain("actor.").And.Not.Contain("WorldSeed")
                .And.Not.Contain("stock").And.Not.Contain("revision").And.Not.Contain("SourceContentHash"));
            Assert.That(again.Claims[0].Source, Does.Contain("Nhận định của bản thân"));
            Assert.Throws<NotSupportedException>(() => ((IList<string>)again.Needs)[0] = "bad");
        }
        [TestCase("count")][TestCase("dead")][TestCase("minor")][TestCase("home")][TestCase("household")]
        [TestCase("duplicate")][TestCase("hash")][TestCase("skill-source")][TestCase("claim-owner")][TestCase("claim-source")]
        [TestCase("history-age")][TestCase("empty-needs")][TestCase("unknown-skill")]
        public void S16_02_InvalidDerivedContentIsRefused(string mutation)
        {
            var d = OriginFixture.Document(); var a = d.records[0];
            switch (mutation)
            {
                case "count": d.records = d.records.Take(7).ToArray(); break;
                case "dead": a.alive = false; break;
                case "minor": a.age = 17; break;
                case "home": a.community_id = "community.con-may@1"; break;
                case "household": a.household_id = "household.vac-tong@1"; break;
                case "duplicate": d.records[1] = a; break;
                case "hash": d.source_hash = "bad"; break;
                case "skill-source": a.skills[0].history_id = "missing"; break;
                case "claim-owner": a.claims[0].id = "fact.vac-tong-claim@1"; break;
                case "claim-source": a.claims[0].source = ""; break;
                case "history-age": a.history[0].at_age = 121; break;
                case "empty-needs": a.needs = Array.Empty<string>(); break;
                case "unknown-skill": a.skills[0].id = "injected"; break;
            }
            Assert.Throws<ArgumentException>(() => OriginCohortCompiler.Compile(d));
        }
    }
}
