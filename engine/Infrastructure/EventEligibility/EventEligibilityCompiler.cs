using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Game.Foundation.Random;
using Game.Infrastructure.SettlementContent;

namespace Game.Infrastructure.EventEligibility
{
    internal enum EligibilityTruth { Unknown, False, True }
    internal sealed class EligibilityNode
    {
        private readonly Func<EligibilityContext, EligibilityTruth> evaluate;
        internal EligibilityNode(Func<EligibilityContext, EligibilityTruth> evaluate) { this.evaluate = evaluate; }
        internal EligibilityTruth Evaluate(EligibilityContext context) => evaluate(context);
    }
    public sealed class EventEligibilityTemplate
    {
        public string DefinitionId { get; }
        public string DisplayKey { get; }
        public IReadOnlyList<string> ActorIds { get; }
        public long CooldownTicks { get; }
        internal EligibilityNode Physical { get; }
        internal EligibilityNode Perceived { get; }
        internal EventEligibilityTemplate(string id, string displayKey, string[] actors, long cooldown, EligibilityNode physical, EligibilityNode perceived)
        { DefinitionId = id; DisplayKey = displayKey; ActorIds = Array.AsReadOnly(actors); CooldownTicks = cooldown; Physical = physical; Perceived = perceived; }
    }
    public sealed class EligibilityResult
    {
        public bool Eligible { get; }
        public string Reason { get; }
        internal EligibilityResult(bool eligible, string reason) { Eligible = eligible; Reason = reason; }
    }
    /// <summary>Developer-facing eligibility only: no selection, RNG, occurrence, cooldown write or gameplay effects.</summary>
    public sealed class EventEligibilityRegistry
    {
        public string ContentHash { get; }
        public string SettlementHash { get; }
        public IReadOnlyList<EventEligibilityTemplate> Templates { get; }
        public IReadOnlyDictionary<string, string> Localization { get; }
        private readonly HashSet<string> actors, facts;
        internal EventEligibilityRegistry(string hash, SettlementRegistry settlement, IEnumerable<EventEligibilityTemplate> templates, Dictionary<string, string> localization)
        {
            ContentHash = hash; SettlementHash = settlement.ContentHash;
            Templates = Array.AsReadOnly(templates.OrderBy(x => x.DefinitionId, StringComparer.Ordinal).ToArray());
            Localization = new ReadOnlyDictionary<string, string>(localization);
            actors = new HashSet<string>(settlement.ByKind("actor").Select(x => (string)x["definition_id"]!), StringComparer.Ordinal);
            facts = new HashSet<string>(settlement.ByKind("fact").Select(x => (string)x["definition_id"]!), StringComparer.Ordinal);
        }
        public EligibilityResult Evaluate(string templateId, EligibilityContext context)
        {
            EligibilityChecks.Require(context != null, "CONTEXT_NULL");
            var template = Templates.SingleOrDefault(x => x.DefinitionId == templateId);
            EligibilityChecks.Require(template != null, "TEMPLATE_REFERENCE");
            EligibilityChecks.Require(actors.Contains(context!.ActorId) && context.KnownFactIds.All(facts.Contains), "CONTEXT_REFERENCE");
            if (!context.Alive) return new EligibilityResult(false, "actor-dead");
            if (!template!.ActorIds.Contains(context.ActorId)) return new EligibilityResult(false, "actor-not-listed");
            // Ordered nonnegative ticks make subtraction safe even near Int64.MaxValue.
            if (context.LastOccurrenceTick.HasValue && context.NowTick - context.LastOccurrenceTick.Value < template.CooldownTicks)
                return new EligibilityResult(false, "cooldown");
            var physical = template.Physical.Evaluate(context);
            if (physical != EligibilityTruth.True) return new EligibilityResult(false, physical == EligibilityTruth.Unknown ? "physical-unknown" : "physical-false");
            var perceived = template.Perceived.Evaluate(context);
            if (perceived != EligibilityTruth.True) return new EligibilityResult(false, perceived == EligibilityTruth.Unknown ? "perceived-unknown" : "perceived-false");
            return new EligibilityResult(true, "eligible-candidate");
        }
    }
    public static class EventEligibilityCompiler
    {
        public const string Profile = "event-eligibility-v1";
        private static void Require(bool condition, string code) => EligibilityChecks.Require(condition, code);
        private static Dictionary<string, object?> Map(object? value) { Require(value is Dictionary<string, object?>, "OBJECT"); return (Dictionary<string, object?>)value!; }
        private static void Fields(Dictionary<string, object?> map, params string[] keys) => Require(map.Count == keys.Length && keys.All(map.ContainsKey), "FIELDS");
        private static string Text(object? value) { Require(value is string s && !string.IsNullOrWhiteSpace(s) && s.Length <= 2048, "TEXT"); return (string)value!; }
        private static long Number(object? value, long min, long max) { Require(value is long || value is int, "INTEGER"); long n = Convert.ToInt64(value); Require(n >= min && n <= max, "RANGE"); return n; }
        private static object[] Items(object? value, int min, int max) { Require(value is object[], "ARRAY"); var a = (object[])value!; Require(a.Length >= min && a.Length <= max, "ARRAY_SIZE"); return a; }
        private static object? Copy(object? value, int depth, ref int count)
        {
            Require(depth <= 24 && ++count <= 10000, "DEPTH_SIZE");
            if (value is Dictionary<string, object?> map)
            {
                Require(map.Count <= 256, "OBJECT_SIZE"); var result = new Dictionary<string, object?>(StringComparer.Ordinal);
                foreach (var pair in map) result.Add(Text(pair.Key), Copy(pair.Value, depth + 1, ref count)); return result;
            }
            if (value is object[] array)
            {
                Require(array.Length <= 256, "ARRAY_SIZE"); var result = new object?[array.Length];
                for (int i = 0; i < array.Length; i++) result[i] = Copy(array[i], depth + 1, ref count); return result;
            }
            if (value is string) return Text(value);
            Require(value == null || value is bool || value is int || value is long, "PRIMITIVE");
            return value is int integer ? (object)(long)integer : value;
        }
        public static EventEligibilityRegistry Compile(Dictionary<string, object?> input, SettlementRegistry settlement)
        {
            Require(settlement != null, "SETTLEMENT_NULL"); int count = 0; var root = Map(Copy(input, 0, ref count));
            Fields(root, "pack_id", "version", "schema_version", "profile", "settlement_hash", "templates", "localization");
            Require(Text(root["pack_id"]) == "slice.events" && Text(root["profile"]) == Profile, "PROFILE");
            Number(root["version"], 1, 1); Number(root["schema_version"], 1, 1);
            Require(Text(root["settlement_hash"]) == settlement!.ContentHash, "SETTLEMENT_HASH");
            var localization = Map(root["localization"]).ToDictionary(x => x.Key, x => Text(x.Value), StringComparer.Ordinal);
            Require(localization.Keys.All(x => Regex.IsMatch(x, @"\A[a-z][a-z0-9_.-]*\z", RegexOptions.CultureInvariant)), "LOCALIZATION_KEY");
            var actors = new HashSet<string>(settlement.ByKind("actor").Select(x => (string)x["definition_id"]!), StringComparer.Ordinal);
            var facts = new HashSet<string>(settlement.ByKind("fact").Select(x => (string)x["definition_id"]!), StringComparer.Ordinal);
            var templates = new List<EventEligibilityTemplate>(); var ids = new HashSet<string>(StringComparer.Ordinal);
            var definitions = Items(root["templates"], 8, 12).Select(Map).ToArray();
            foreach (var d in definitions)
            {
                Fields(d, "definition_id", "version", "schema_version", "source", "classification", "owner_domain", "display_key", "handler", "actor_ids", "cooldown_ticks", "physical_condition", "perceived_condition", "information_route");
                string id = Text(d["definition_id"]); Require(Regex.IsMatch(id, @"\Aevent\.[a-z][a-z0-9_-]*@1\z", RegexOptions.CultureInvariant) && ids.Add(id), "ID");
                Number(d["version"], 1, 1); Number(d["schema_version"], 1, 1); Text(d["source"]);
                Require(Text(d["classification"]) == "game-canon" && Text(d["owner_domain"]) == "Narrative", "METADATA");
                Require(Text(d["handler"]) == "eligibility-candidate-v1" && Text(d["information_route"]) == "actor-known-facts", "HANDLER");
                string displayKey = Text(d["display_key"]); Require(localization.ContainsKey(displayKey), "LOCALIZATION");
                var actorIds = Items(d["actor_ids"], 1, 30).Select(Text).ToArray();
                Require(actorIds.All(actors.Contains) && actorIds.Distinct(StringComparer.Ordinal).Count() == actorIds.Length, "ACTOR_REFERENCE");
                long cooldown = Number(d["cooldown_ticks"], 0, 1000000);
                int nodeCount = 0;
                var physical = CompileNode(Map(d["physical_condition"]), false, facts, 1, ref nodeCount);
                var perceived = CompileNode(Map(d["perceived_condition"]), true, facts, 1, ref nodeCount);
                templates.Add(new EventEligibilityTemplate(id, displayKey, actorIds, cooldown, physical, perceived));
            }
            root["templates"] = definitions.OrderBy(x => (string)x["definition_id"]!, StringComparer.Ordinal).Cast<object?>().ToArray();
            root["localization"] = localization.OrderBy(x => x.Key, StringComparer.Ordinal).Select(x => (object?)new Dictionary<string, object?> { ["key"] = x.Key, ["text"] = x.Value }).ToArray();
            return new EventEligibilityRegistry(CanonicalCodec.Hash(root), settlement, templates, localization);
        }
        private static EligibilityNode CompileNode(Dictionary<string, object?> node, bool perceived, HashSet<string> facts, int depth, ref int count)
        {
            Require(depth <= 8 && ++count <= 256, "AST_SIZE"); Require(node.ContainsKey("op"), "FIELDS");
            string op = Text(node["op"]);
            if (op == "All" || op == "Any")
            {
                Fields(node, "op", "children"); var children = new List<EligibilityNode>();
                foreach (var child in Items(node["children"], 1, 16)) children.Add(CompileNode(Map(child), perceived, facts, depth + 1, ref count));
                return new EligibilityNode(context => {
                    bool unknown = false;
                    foreach (var child in children) { var value = child.Evaluate(context); if (value == EligibilityTruth.Unknown) unknown = true;
                        else if (op == "All" && value == EligibilityTruth.False) return EligibilityTruth.False;
                        else if (op == "Any" && value == EligibilityTruth.True) return EligibilityTruth.True; }
                    return unknown ? EligibilityTruth.Unknown : (op == "All" ? EligibilityTruth.True : EligibilityTruth.False);
                });
            }
            if (op == "Not")
            {
                Fields(node, "op", "child"); var child = CompileNode(Map(node["child"]), perceived, facts, depth + 1, ref count);
                return new EligibilityNode(context => { var value = child.Evaluate(context); return value == EligibilityTruth.Unknown ? value : value == EligibilityTruth.True ? EligibilityTruth.False : EligibilityTruth.True; });
            }
            if (op == "Compare")
            {
                Fields(node, "op", "field", "comparison", "value"); string field = Text(node["field"]), comparison = Text(node["comparison"]);
                var spec = EligibilityChecks.Field(field, perceived); object expected = spec.Validate(node["value"]);
                Require((spec.Boolean ? new[] { "eq", "ne" } : new[] { "eq", "ne", "lt", "lte", "gt", "gte" }).Contains(comparison), "COMPARISON");
                return new EligibilityNode(context => {
                    var fields = perceived ? context.Perceived : context.Physical; if (!fields.TryGetValue(field, out var actual)) return EligibilityTruth.Unknown;
                    int order = spec.Boolean ? ((bool)actual!).CompareTo((bool)expected) : ((long)actual!).CompareTo((long)expected);
                    bool result = comparison == "eq" ? order == 0 : comparison == "ne" ? order != 0 : comparison == "lt" ? order < 0 : comparison == "lte" ? order <= 0 : comparison == "gt" ? order > 0 : order >= 0;
                    return result ? EligibilityTruth.True : EligibilityTruth.False;
                });
            }
            if (op == "HasKnownFact")
            {
                Fields(node, "op", "fact_id"); string fact = Text(node["fact_id"]); Require(perceived && facts.Contains(fact), "FACT_SCOPE_REFERENCE");
                return new EligibilityNode(context => context.KnownFactIds.Contains(fact) ? EligibilityTruth.True : EligibilityTruth.False);
            }
            if (op == "HasResource")
            {
                Fields(node, "op", "resource", "minimum"); string resource = Text(node["resource"]); Require(!perceived && (resource == "grain" || resource == "timber"), "RESOURCE_SCOPE"); long minimum = Number(node["minimum"], 1, 1000000);
                return new EligibilityNode(context => !context.Resources.TryGetValue(resource, out var amount) ? EligibilityTruth.Unknown : amount >= minimum ? EligibilityTruth.True : EligibilityTruth.False);
            }
            throw new InvalidDataException("ELIGIBILITY_OPERATOR");
        }
    }
}
