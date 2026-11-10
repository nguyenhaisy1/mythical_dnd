using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Infrastructure.Origins;
using Game.Infrastructure.Weaving;
using NUnit.Framework;

namespace Game.Tests
{
    internal static class WeavingFixture
    {
        internal const string Request = "0000000000000000000000000000000a";
        internal static WeavingProfileDocument Document()
        {
            var directory = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "rules/parameters/personal-weaving-v1.json"))) directory = directory.Parent;
            return JsonSerializer.Deserialize<WeavingProfileDocument>(File.ReadAllText(Path.Combine(directory!.FullName, "rules/parameters/personal-weaving-v1.json")), new JsonSerializerOptions { IncludeFields = true })!;
        }
        internal static WeavingContentRegistry Registry() => WeavingContentRegistry.Compile(OriginFixture.Registry(), Document());
    }
    public sealed class WeavingContentTests
    {
        [Test]
        public void S17_02_ProfileHashesMatchIndependentPythonAnswersAndOnlyTwoAuthoredSkills()
        {
            var r = WeavingFixture.Registry();
            Assert.That(r.ContentHash, Is.EqualTo("f9d850edf58c22f525a185e8ba0e3430a0e189e0ccfe76531a088173a404a3bc"));
            Assert.That(r.Recipe.Fingerprint, Is.EqualTo("3001a817410943cc233bc064badf0c68493e6e9f52e99e48535e07ca05bd2df7"));
            Assert.That(r.SkillValue("actor.vac-lam@1"), Is.EqualTo(43)); Assert.That(r.SkillValue("actor.vac-nhu@1"), Is.EqualTo(61));
            Assert.That(r.Cohort.Candidates.Count(c => r.SkillValue(c.ActorDefinitionId) >= r.Recipe.MinimumSkill), Is.EqualTo(2));
            Assert.That(r.SkillValue("actor.ben-kien@1"), Is.Zero); Assert.That(r.StarterFiber, Is.EqualTo(8));
        }
        [TestCase("profile")][TestCase("recipe")][TestCase("material")][TestCase("output")][TestCase("tool")][TestCase("source")]
        [TestCase("cost")][TestCase("duration")][TestCase("yield")][TestCase("minimum")][TestCase("starter")]
        public void S17_02_UnknownOrChangedProfileValuesRequireNewExplicitContract(string field)
        {
            var d = WeavingFixture.Document();
            switch (field)
            {
                case "profile": d.profile = "unknown"; break; case "recipe": d.recipe_id = "recipe.other@1"; break;
                case "material": d.material_id = "commodity.grain@1"; break; case "output": d.output_id = "commodity.grain@1"; break;
                case "tool": d.tool_mode = "invented-tool"; break; case "source": d.source = "historical fact"; break;
                case "cost": d.fiber_cost = 0; break; case "duration": d.duration_ticks = -1; break;
                case "yield": d.cord_yield = 99; break; case "minimum": d.minimum_skill = 0; break; case "starter": d.starter_fiber = 999; break;
            }
            Assert.Throws<FormatException>(() => WeavingContentRegistry.Compile(OriginFixture.Registry(), d));
        }
        [Test]
        public void S17_02_CompilationFreezesParametersAndRawSkillLookupWithoutChangingReveal()
        {
            var cohortDocument = OriginFixture.Document(); var cohort = OriginCohortCompiler.Compile(cohortDocument);
            var d = WeavingFixture.Document(); var r = WeavingContentRegistry.Compile(cohort, d);
            var before = JsonSerializer.Serialize(cohort.Get("actor.vac-lam@1").Reveal(0)); var hash = cohort.ContentHash;
            d.fiber_cost = 80; d.source = "mutated";
            cohortDocument.records.Single(x => x.actor_id == "actor.vac-lam@1").skills[0].value = 0;
            Assert.That(r.Recipe.FiberCost, Is.EqualTo(2)); Assert.That(r.SkillValue("actor.vac-lam@1"), Is.EqualTo(43));
            Assert.That(cohort.Get("actor.vac-lam@1").GetSkillValue("unknown"), Is.Zero);
            Assert.That(JsonSerializer.Serialize(cohort.Get("actor.vac-lam@1").Reveal(0)), Is.EqualTo(before));
            Assert.That(cohort.ContentHash, Is.EqualTo(hash));
            Assert.That(before, Does.Not.Contain("SkillValues").And.Not.Contain("actor.vac-lam").And.Not.Contain("prototype-authored"));
        }
    }
}
