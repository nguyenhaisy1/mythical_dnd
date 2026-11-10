using System;
using System.Collections.Generic;
using Game.Domain.Weaving;
using Game.Foundation.Random;
using Game.Infrastructure.Origins;

namespace Game.Infrastructure.Weaving
{
    [Serializable] public sealed class WeavingProfileDocument
    {
        public string profile = "", recipe_id = "", material_id = "", output_id = "", tool_mode = "", source = "";
        public int fiber_cost, cord_yield, minimum_skill, starter_fiber;
        public long duration_ticks;
    }
    /// <summary>Explicit engineering genesis, never inferred household stock or permission.</summary>
    public sealed class WeavingContentRegistry
    {
        public const string ProfileId = "personal-weaving-v1";
        public OriginCohortRegistry Cohort { get; }
        public WeavingRecipe Recipe { get; }
        public string ContentHash { get; }
        public int StarterFiber { get; }
        private WeavingContentRegistry(OriginCohortRegistry cohort, WeavingRecipe recipe, string hash, int starter)
        { Cohort = cohort; Recipe = recipe; ContentHash = hash; StarterFiber = starter; }
        public static WeavingContentRegistry Compile(OriginCohortRegistry cohort, WeavingProfileDocument document)
        {
            if (cohort == null || document == null) throw new ArgumentNullException();
            if (document.profile != ProfileId || document.recipe_id != "recipe.hand-braid-cord@1" ||
                document.material_id != "commodity.fiber@1" || document.output_id != "commodity.cord@1" || document.tool_mode != "hands" ||
                document.fiber_cost != 2 || document.duration_ticks != 30 || document.cord_yield != 1 || document.minimum_skill != 40 ||
                document.starter_fiber != 8 || document.source != "prototype-authored: ADR026 Sprint17 engineering fixture; not balanced or historical evidence; starter stock is personal, not household authority")
                throw new FormatException("WEAVING_PROFILE");
            var parameters = Parameters(document);
            var recipe = new WeavingRecipe(document.fiber_cost, document.duration_ticks, document.cord_yield, document.minimum_skill, CanonicalCodec.Hash(parameters));
            var hash = CanonicalCodec.Hash(new Dictionary<string, object?> {
                ["profile"] = ProfileId, ["cohort_hash"] = cohort.ContentHash, ["parameters"] = parameters });
            return new WeavingContentRegistry(cohort, recipe, hash, document.starter_fiber);
        }
        public int SkillValue(string actorDefinitionId) => Cohort.Get(actorDefinitionId).GetSkillValue("weaving");
        private static Dictionary<string, object?> Parameters(WeavingProfileDocument d) => new Dictionary<string, object?> {
            ["profile"] = d.profile, ["recipe_id"] = d.recipe_id, ["material_id"] = d.material_id, ["output_id"] = d.output_id,
            ["fiber_cost"] = (long)d.fiber_cost, ["duration_ticks"] = d.duration_ticks, ["cord_yield"] = (long)d.cord_yield,
            ["minimum_skill"] = (long)d.minimum_skill, ["starter_fiber"] = (long)d.starter_fiber, ["tool_mode"] = d.tool_mode, ["source"] = d.source };
    }
}
