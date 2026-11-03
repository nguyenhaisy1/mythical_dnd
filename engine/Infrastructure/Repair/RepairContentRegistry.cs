using System;
using System.Collections.Generic;
using System.Linq;
using Game.Domain.Repair;
using Game.Foundation.Identity;
using Game.Foundation.Random;

namespace Game.Infrastructure.Repair
{
    /// <summary>Strict standalone engineering profile. It does not widen kernel-content-v1.</summary>
    public sealed class RepairContentRegistry
    {
        public const string SupportedRuleVersion = "transfer-resource-v1+scheduler-v1+boat-repair-v1";
        public string RuleVersion => SupportedRuleVersion;
        public string ValidationProfile => RepairState.ProfileId;
        public RepairRecipe Recipe { get; }
        public string ContentHash { get; }
        private RepairContentRegistry(RepairRecipe recipe)
        { Recipe = recipe; ContentHash = HashForRecipe(recipe); }

        public static RepairContentRegistry Compile(IReadOnlyDictionary<string, object?> authoring)
        {
            if (authoring == null) throw new ArgumentNullException(nameof(authoring));
            Fields(authoring, "schema_version", "id", "profile", "status", "recipe");
            Number(authoring, "schema_version", 1, 1);
            Exact(authoring, "id", "content.boat-repair@1");
            Exact(authoring, "profile", RepairState.ProfileId);
            Exact(authoring, "status", "engineering-prototype-not-balanced");
            if (!(authoring["recipe"] is IReadOnlyDictionary<string, object?> recipe)) throw new FormatException("REPAIR_CONTENT_RECIPE");
            Fields(recipe, "id", "version", "handler", "material", "material_quantity", "duration_ticks", "condition_gain", "condition_max");
            Exact(recipe, "id", "recipe.boat-repair@1"); Number(recipe, "version", 1, 1);
            Exact(recipe, "handler", RepairState.ProfileId); Exact(recipe, "material", "commodity.timber@1");
            var parsed = new RepairRecipe(DefinitionId.Parse("recipe.boat-repair@1"), DefinitionId.Parse("commodity.timber@1"),
                Number(recipe, "material_quantity", 1, long.MaxValue), Number(recipe, "duration_ticks", 1, long.MaxValue),
                checked((int)Number(recipe, "condition_gain", 1, 1000)), checked((int)Number(recipe, "condition_max", 1000, 1000)));
            return new RepairContentRegistry(parsed);
        }
        internal static string HashForRecipe(RepairRecipe recipe)
        {
            if (recipe == null) throw new ArgumentNullException(nameof(recipe));
            if (recipe.Id.ToString() != "recipe.boat-repair@1" || recipe.MaterialCommodity.ToString() != "commodity.timber@1")
                throw new FormatException("REPAIR_CONTENT_REFERENCE");
            // Every authoring field, including engineering status and handler/version, participates.
            return CanonicalCodec.Hash(new Dictionary<string, object?> {
                ["schema_version"] = 1L, ["id"] = "content.boat-repair@1", ["profile"] = RepairState.ProfileId,
                ["status"] = "engineering-prototype-not-balanced", ["recipe"] = new Dictionary<string, object?> {
                    ["id"] = recipe.Id.ToString(), ["version"] = 1L, ["handler"] = RepairState.ProfileId,
                    ["material"] = recipe.MaterialCommodity.ToString(), ["material_quantity"] = recipe.MaterialQuantity,
                    ["duration_ticks"] = recipe.DurationTicks, ["condition_gain"] = (long)recipe.ConditionGain,
                    ["condition_max"] = (long)recipe.ConditionMax } });
        }
        private static void Fields(IReadOnlyDictionary<string, object?> map, params string[] keys)
        {
            if (map.Count != keys.Length || map.Keys.Any(k => !keys.Contains(k, StringComparer.Ordinal)))
                throw new FormatException("REPAIR_CONTENT_FIELDS");
        }
        private static void Exact(IReadOnlyDictionary<string, object?> map, string key, string expected)
        { if (!(map[key] is string value) || value != expected) throw new FormatException("REPAIR_CONTENT_" + key.ToUpperInvariant()); }
        private static long Number(IReadOnlyDictionary<string, object?> map, string key, long minimum, long maximum)
        {
            var raw = map[key]; var value = raw is long l ? l : raw is int i ? i : throw new FormatException("REPAIR_CONTENT_INTEGER");
            if (value < minimum || value > maximum) throw new FormatException("REPAIR_CONTENT_RANGE"); return value;
        }
    }
}
