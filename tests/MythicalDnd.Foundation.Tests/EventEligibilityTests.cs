using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Infrastructure.EventEligibility;
using Game.Infrastructure.SettlementContent;
using NUnit.Framework;

namespace Game.Tests
{
    public class EventEligibilityTests
    {
        private static object? Decode(JsonElement x) => x.ValueKind switch {
            JsonValueKind.Object => x.EnumerateObject().ToDictionary(p => p.Name, p => Decode(p.Value), StringComparer.Ordinal),
            JsonValueKind.Array => x.EnumerateArray().Select(Decode).ToArray(), JsonValueKind.String => x.GetString(),
            JsonValueKind.Number => x.GetInt64(), JsonValueKind.True => true, JsonValueKind.False => false, JsonValueKind.Null => null,
            _ => throw new InvalidDataException("fixture") };
        private static Dictionary<string, object?> Load(string name) { using var d = JsonDocument.Parse(File.ReadAllText(Path.Combine(TestContext.CurrentContext.TestDirectory, name))); return Map(Decode(d.RootElement)); }
        private static Dictionary<string, object?> Map(object? value) => (Dictionary<string, object?>)value!;
        private static object[] Items(object? value) => (object[])value!;
        private static Dictionary<string, object?> Pack() => Load("event-pack.json");
        private static SettlementRegistry Settlement() => SettlementCompiler.Compile(Load("settlement-pack.json"));
        private static EventEligibilityRegistry Compile(Dictionary<string, object?>? pack = null) => EventEligibilityCompiler.Compile(pack ?? Pack(), Settlement());
        private static Dictionary<string, object?> Template(Dictionary<string, object?> p) => Map(Items(p["templates"])[0]);
        private static Dictionary<string, object?> Compare(string field, object value, string comparison = "eq") => new Dictionary<string, object?> { ["op"] = "Compare", ["field"] = field, ["comparison"] = comparison, ["value"] = value };
        private static Dictionary<string, object?> Not(object child) => new Dictionary<string, object?> { ["op"] = "Not", ["child"] = child };
        private static EligibilityContext Context(long now = 3000, long? last = null, bool alive = true, string actor = "actor.vac-tong@1", Dictionary<string, object?>? physical = null, Dictionary<string, object?>? perceived = null, string[]? facts = null, Dictionary<string, long>? resources = null)
            => new EligibilityContext(actor, alive, now, last, physical ?? new Dictionary<string, object?> { ["deadline_remaining_ticks"] = 30L }, perceived ?? new Dictionary<string, object?> { ["transport_request"] = true }, facts ?? System.Array.Empty<string>(), resources ?? new Dictionary<string, long>());

        private static IEnumerable<TestCaseData> Scenarios()
        {
            foreach (var s in Items(Load("event-scenarios.json")["scenarios"]).Select(Map))
                yield return new TestCaseData(s).SetName("VS_02B_Authored_" + (string)s["scenario_id"]!);
        }
        [TestCaseSource(nameof(Scenarios))]
        public void VS_02B_AuthoredScenario(Dictionary<string, object?> scenario)
        {
            var c = Map(scenario["context"]);
            var context = new EligibilityContext((string)c["actor_id"]!, (bool)c["alive"]!, (long)c["now_tick"]!, (long?)c["last_occurrence_tick"], Map(c["physical"]), Map(c["perceived"]), Items(c["known_fact_ids"]).Cast<string>(), Map(c["resources"]).ToDictionary(x => x.Key, x => (long)x.Value!));
            var registry = Compile(); var result = registry.Evaluate((string)scenario["template_id"]!, context);
            Assert.That(result.Eligible, Is.EqualTo((bool)scenario["expected_eligible"]!), result.Reason);
            Assert.That(registry.Evaluate((string)scenario["template_id"]!, context).Reason, Is.EqualTo(result.Reason));
        }
        [Test]
        public void VS_02B_CompiledPackHasEightTemplatesAndSettlementBinding()
        {
            var registry = Compile(); Assert.That(registry.Templates.Count, Is.EqualTo(8)); Assert.That(registry.SettlementHash, Is.EqualTo(Settlement().ContentHash));
            Assert.That(registry.Templates.Select(x => x.DefinitionId), Is.Ordered.Using<string>(StringComparer.Ordinal));
        }
        [Test]
        public void VS_02B_InputAndPublicViewsCannotMutateEvaluation()
        {
            var p = Pack(); var registry = Compile(p); string id = (string)Template(p)["definition_id"]!; string hash = registry.ContentHash;
            Template(p)["cooldown_ticks"] = 1000000L; Map(Template(p)["perceived_condition"])["value"] = false;
            var physical = new Dictionary<string, object?> { ["deadline_remaining_ticks"] = 30L }; var perceived = new Dictionary<string, object?> { ["transport_request"] = true }; var resources = new Dictionary<string, long> { ["grain"] = 10 };
            var context = Context(physical: physical, perceived: perceived, resources: resources); physical.Clear(); perceived.Clear(); resources.Clear();
            Assert.That(registry.Evaluate(id, context).Eligible, Is.True); Assert.That(registry.ContentHash, Is.EqualTo(hash));
            Assert.Throws<NotSupportedException>(() => ((IDictionary<string, object?>)context.Physical).Clear());
            Assert.Throws<NotSupportedException>(() => ((IDictionary<string, long>)context.Resources).Clear());
            Assert.Throws<NotSupportedException>(() => ((IList<string>)context.KnownFactIds).Add("fact.fake@1"));
            Assert.Throws<NotSupportedException>(() => ((IList<EventEligibilityTemplate>)registry.Templates).Clear());
            Assert.Throws<NotSupportedException>(() => ((IList<string>)registry.Templates[0].ActorIds).Clear());
            Assert.Throws<NotSupportedException>(() => ((IDictionary<string, string>)registry.Localization).Clear());
        }
        [Test]
        public void VS_02B_HashStableUnderMapAndTemplateOrderingButBindsThresholds()
        {
            var p = Pack(); string hash = Compile(p).ContentHash;
            p["templates"] = Items(p["templates"]).Reverse().ToArray(); p["localization"] = Map(p["localization"]).Reverse().ToDictionary(x => x.Key, x => x.Value);
            Assert.That(Compile(p).ContentHash, Is.EqualTo(hash)); Template(p)["cooldown_ticks"] = 999L; Assert.That(Compile(p).ContentHash, Is.Not.EqualTo(hash));
        }
        [TestCase("All", false, false)][TestCase("All", true, false)][TestCase("Any", false, false)][TestCase("Any", true, true)]
        public void VS_02B_UnknownUnderCompositeAndNotUsesThreeValuedLogic(string op, bool known, bool expected)
        {
            var p = Pack(); Template(p)["physical_condition"] = new Dictionary<string, object?> { ["op"] = op, ["children"] = new object[] { Not(Compare("boat_condition", 200L)), Compare("rumor_delivered", true) } };
            var c = Context(physical: new Dictionary<string, object?> { ["rumor_delivered"] = known });
            Assert.That(Compile(p).Evaluate((string)Template(p)["definition_id"]!, c).Eligible, Is.EqualTo(expected));
        }
        [Test]
        public void VS_02B_DoubleNotOfUnknownStillCannotBecomeCandidate()
        {
            var p = Pack(); Template(p)["physical_condition"] = Not(Not(Compare("boat_condition", 100L)));
            Assert.That(Compile(p).Evaluate((string)Template(p)["definition_id"]!, Context()).Reason, Is.EqualTo("physical-unknown"));
        }
        [Test]
        public void VS_02B_MissingResourceIsUnknownEvenUnderNotButKnownZeroIsAbsent()
        {
            var p = Pack(); Template(p)["physical_condition"] = Not(new Dictionary<string, object?> { ["op"] = "HasResource", ["resource"] = "grain", ["minimum"] = 1L }); var registry = Compile(p); var id = (string)Template(p)["definition_id"]!;
            Assert.That(registry.Evaluate(id, Context()).Reason, Is.EqualTo("physical-unknown"));
            Assert.That(registry.Evaluate(id, Context(resources: new Dictionary<string, long> { ["grain"] = 0 })).Eligible, Is.True);
        }
        [Test]
        public void VS_02B_KnowledgeMembershipUsesActorContextNotRosterOrPhysicalTruth()
        {
            var p = Pack(); string fact = (string)Settlement().ByKind("fact")[0]["definition_id"]!;
            Template(p)["perceived_condition"] = new Dictionary<string, object?> { ["op"] = "HasKnownFact", ["fact_id"] = fact }; var registry = Compile(p); var id = (string)Template(p)["definition_id"]!;
            Assert.That(registry.Evaluate(id, Context()).Eligible, Is.False);
            Assert.That(registry.Evaluate(id, Context(facts: new[] { fact })).Eligible, Is.True);
        }
        [TestCase(59, false)][TestCase(60, true)][TestCase(61, true)]
        public void VS_02B_CooldownBoundaryNearInt64Maximum(long elapsed, bool expected)
        {
            Assert.That(Compile().Evaluate("event.transport-deadline@1", Context(now: long.MaxValue, last: long.MaxValue - elapsed)).Eligible, Is.EqualTo(expected));
        }
        [TestCase(-1, null)][TestCase(10, -1)][TestCase(10, 11)]
        public void VS_02B_InvalidClockRejected(long now, long? last) => Assert.Throws<InvalidDataException>(() => Context(now: now, last: last));
        [TestCase("actor.fake@1")][TestCase("actor.vac-tong@1\n")]
        public void VS_02B_UnknownOrMalformedActorRejected(string actor)
            => Assert.Throws<InvalidDataException>(() => Compile().Evaluate("event.transport-deadline@1", Context(actor: actor)));
        [Test]
        public void VS_02B_UnknownContextFactRejected()
            => Assert.Throws<InvalidDataException>(() => Compile().Evaluate("event.transport-deadline@1", Context(facts: new[] { "fact.fake@1" })));
        [TestCase("physical-scope")][TestCase("perceived-scope")][TestCase("bool-type")][TestCase("float")][TestCase("range")][TestCase("negative-resource")][TestCase("resource-kind")][TestCase("duplicate-fact")]
        public void VS_02B_InvalidContextRejected(string kind)
        {
            var physical = new Dictionary<string, object?>(); var perceived = new Dictionary<string, object?>(); var resources = new Dictionary<string, long>(); string[] facts = System.Array.Empty<string>();
            switch (kind) {
                case "physical-scope": physical["repair_need"] = true; break; case "perceived-scope": perceived["boat_condition"] = 300L; break;
                case "bool-type": perceived["repair_need"] = 1L; break; case "float": physical["boat_condition"] = 300.0; break;
                case "range": physical["boat_condition"] = 1001L; break; case "negative-resource": resources["grain"] = -1; break;
                case "resource-kind": resources["gold"] = 1; break; case "duplicate-fact": facts = new[] { "fact.fake@1", "fact.fake@1" }; break;
            }
            Assert.Throws<InvalidDataException>(() => Context(physical: physical, perceived: perceived, facts: facts, resources: resources));
        }
        [TestCase("profile")][TestCase("extra")][TestCase("version")][TestCase("settlement")][TestCase("handler")][TestCase("id-newline")][TestCase("duplicate-id")][TestCase("actor")][TestCase("duplicate-actor")][TestCase("owner")][TestCase("classification")][TestCase("localization")][TestCase("localization-key")][TestCase("cooldown")][TestCase("script")][TestCase("field")][TestCase("scope")][TestCase("type")][TestCase("bool-order")][TestCase("known-physical")][TestCase("known-ref")][TestCase("resource-perceived")][TestCase("resource-kind")][TestCase("resource-zero")][TestCase("empty-all")][TestCase("node-extra")][TestCase("depth")][TestCase("cycle")]
        public void VS_02B_InvalidAuthoringRejected(string kind)
        {
            var p = Pack(); var d = Template(p);
            switch (kind) {
                case "profile": p["profile"] = "arbitrary-script"; break; case "extra": p["unknown"] = true; break; case "version": p["version"] = 2L; break;
                case "settlement": p["settlement_hash"] = new string('0', 64); break; case "handler": d["handler"] = "repair-boat-v1"; break;
                case "id-newline": d["definition_id"] += "\n"; break; case "duplicate-id": Map(Items(p["templates"])[1])["definition_id"] = d["definition_id"]; break;
                case "actor": d["actor_ids"] = new object[] { "actor.fake@1" }; break; case "duplicate-actor": d["actor_ids"] = new object[] { "actor.vac-tong@1", "actor.vac-tong@1" }; break;
                case "owner": d["owner_domain"] = "Assets"; break; case "classification": d["classification"] = "historical-fact"; break;
                case "localization": d["display_key"] = "missing"; break; case "localization-key": Map(p["localization"])["bad\n"] = "bad"; break;
                case "cooldown": d["cooldown_ticks"] = -1L; break; case "script": d["physical_condition"] = new Dictionary<string, object?> { ["op"] = "Eval", ["code"] = "true" }; break;
                case "field": d["physical_condition"] = Compare("hidden_secret", true); break; case "scope": d["perceived_condition"] = Compare("boat_condition", 10L); break;
                case "type": d["physical_condition"] = Compare("boat_condition", true); break; case "bool-order": d["perceived_condition"] = Compare("repair_need", true, "gt"); break;
                case "known-physical": d["physical_condition"] = new Dictionary<string, object?> { ["op"] = "HasKnownFact", ["fact_id"] = (string)Settlement().ByKind("fact")[0]["definition_id"]! }; break;
                case "known-ref": d["perceived_condition"] = new Dictionary<string, object?> { ["op"] = "HasKnownFact", ["fact_id"] = "fact.fake@1" }; break;
                case "resource-perceived": d["perceived_condition"] = new Dictionary<string, object?> { ["op"] = "HasResource", ["resource"] = "grain", ["minimum"] = 1L }; break;
                case "resource-kind": d["physical_condition"] = new Dictionary<string, object?> { ["op"] = "HasResource", ["resource"] = "gold", ["minimum"] = 1L }; break;
                case "resource-zero": d["physical_condition"] = new Dictionary<string, object?> { ["op"] = "HasResource", ["resource"] = "grain", ["minimum"] = 0L }; break;
                case "empty-all": d["physical_condition"] = new Dictionary<string, object?> { ["op"] = "All", ["children"] = System.Array.Empty<object>() }; break;
                case "node-extra": Map(d["perceived_condition"])["secret"] = true; break;
                case "depth": object node = Compare("boat_condition", 300L); for (int i = 0; i < 9; i++) node = Not(node); d["physical_condition"] = node; break;
                case "cycle": var cycle = Not(new object()); cycle["child"] = cycle; d["physical_condition"] = cycle; break;
            }
            Assert.Throws<InvalidDataException>(() => Compile(p));
        }
    }
}
