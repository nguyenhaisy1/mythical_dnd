using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Infrastructure.EventEligibility;
using Game.Infrastructure.SettlementContent;

namespace Game.Headless
{
    // Developer QA only. Supplied observations are trusted fixtures, not player projections.
    internal static class EventEligibilityProbe
    {
        internal static int Run(string[] args)
        {
            if (args.Length != 3 && args.Length != 4)
            {
                Console.Error.WriteLine("Usage: --event-eligibility <settlement-pack> <event-pack> [scenarios]");
                return 2;
            }
            try
            {
                var settlement = SettlementCompiler.Compile(SettlementJson.Read(args[1]));
                var registry = EventEligibilityCompiler.Compile(SettlementJson.Read(args[2]), settlement);
                var outcomes = new List<object>();
                bool passed = true;
                if (args.Length == 4)
                {
                    var root = SettlementJson.Read(args[3]);
                    Shape(root, "profile", "scenarios");
                    if (Text(root, "profile") != "event-eligibility-fixtures-v1") throw Error("PROFILE");
                    var scenarios = Array(root, "scenarios");
                    if (scenarios.Length < 1 || scenarios.Length > 256) throw Error("SCENARIO_COUNT");
                    var ids = new HashSet<string>(StringComparer.Ordinal);
                    foreach (var value in scenarios)
                    {
                        var scenario = Map(value);
                        Shape(scenario, "scenario_id", "template_id", "expected_eligible", "reason", "context");
                        var id = Text(scenario, "scenario_id");
                        if (!ids.Add(id)) throw Error("DUPLICATE_SCENARIO");
                        _ = Text(scenario, "reason");
                        var c = Map(scenario["context"]);
                        Shape(c, "actor_id", "alive", "now_tick", "last_occurrence_tick", "physical", "perceived", "known_fact_ids", "resources");
                        var facts = Array(c, "known_fact_ids").Select(x => x is string s ? s : throw Error("FACT_TYPE")).ToArray();
                        if (facts.Length > 256 || facts.Distinct(StringComparer.Ordinal).Count() != facts.Length) throw Error("FACTS");
                        var resources = Map(c["resources"]).ToDictionary(x => x.Key, x => x.Value is long n ? n : throw Error("RESOURCE_TYPE"), StringComparer.Ordinal);
                        var last = c["last_occurrence_tick"] == null ? (long?)null : Integer(c, "last_occurrence_tick");
                        var context = new EligibilityContext(Text(c, "actor_id"), Boolean(c, "alive"), Integer(c, "now_tick"), last,
                            Map(c["physical"]), Map(c["perceived"]), facts, resources);
                        var result = registry.Evaluate(Text(scenario, "template_id"), context);
                        bool expected = Boolean(scenario, "expected_eligible");
                        passed &= result.Eligible == expected;
                        outcomes.Add(new { scenarioId = id, eligible = result.Eligible, expectedEligible = expected, reason = result.Reason });
                    }
                }
                Console.WriteLine(JsonSerializer.Serialize(new { passed, profile = "event-eligibility-v1", contentHash = registry.ContentHash,
                    settlementHash = registry.SettlementHash, templateCount = registry.Templates.Count, outcomes,
                    scope = "trusted developer observations; candidate eligibility only, no event occurrence, world mutation or player projection" }));
                return passed ? 0 : 1;
            }
            catch (Exception e) when (e is InvalidDataException || e is JsonException || e is IOException || e is ArgumentException || e is KeyNotFoundException)
            {
                Console.Error.WriteLine(JsonSerializer.Serialize(new { passed = false, error = e.Message }));
                return 1;
            }
        }
        private static InvalidDataException Error(string name) => new InvalidDataException("EVENT_FIXTURE_" + name);
        private static Dictionary<string, object?> Map(object? value) => value as Dictionary<string, object?> ?? throw Error("OBJECT");
        private static object?[] Array(Dictionary<string, object?> map, string key) => map[key] as object?[] ?? throw Error("ARRAY");
        private static string Text(Dictionary<string, object?> map, string key) => map[key] is string text && text.Length > 0 && text.Length <= 2048 ? text : throw Error("STRING");
        private static long Integer(Dictionary<string, object?> map, string key) => map[key] is long value ? value : throw Error("INTEGER");
        private static bool Boolean(Dictionary<string, object?> map, string key) => map[key] is bool value ? value : throw Error("BOOLEAN");
        private static void Shape(Dictionary<string, object?> map, params string[] fields)
        {
            if (map.Count != fields.Length || fields.Any(f => !map.ContainsKey(f))) throw Error("FIELDS");
        }
    }
}
