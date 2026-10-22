using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using Game.Infrastructure.Content;
using Game.Infrastructure.Persistence;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class Fnd14ContentTests
    {
        private const string Rules = "transfer-resource-v1+scheduler-v1";
        private static ContentDefinitionInput Commodity(string id = "commodity.timber@1", string unit = "count") =>
            new ContentDefinitionInput(id, 1, "commodity", 1, "prototype-authored", "game-canon", "Assets", "commodity.timber", unit: unit);
        private static ContentDefinitionInput Template(string commodity = "commodity.timber@1", string handler = "TransferResource@1", long quantity = 3) =>
            new ContentDefinitionInput("transfer.timber@1", 1, "transfer-template", 1, "prototype-authored", "game-canon", "Assets", "transfer.timber", commodityId: commodity, handler: handler, quantity: quantity);
        private static ContentPackInput Pack(IEnumerable<ContentDefinitionInput>? definitions = null, IEnumerable<ContentDependency>? dependencies = null,
            string id = "test.kernel", int version = 1, int schema = 1, string rules = Rules) =>
            new ContentPackInput(id, version, schema, rules, dependencies ?? Array.Empty<ContentDependency>(), definitions ?? new[] { Commodity(), Template() });
        private static ContentRegistry Compile() => ContentCompiler.Compile(Pack());
        private static CommittedWorld Initial(string commodity = "commodity.timber@1")
        {
            var ids = IdentitySnapshot.Empty(1).BeginStage(1);
            var actor = ids.Allocate<Actor>(); var location = ids.Allocate<Settlement>();
            var a = ids.Allocate<Lot>(); var b = ids.Allocate<Lot>();
            var world = new WorldSnapshot(ids.PrepareSnapshot(), new WorldTick(0), new[] { new ActorState(actor, true, location) },
                new[] { new ResourceLot(a, DefinitionId.Parse(commodity), actor.Value, location, 10, 0), new ResourceLot(b, DefinitionId.Parse(commodity), actor.Value, location, 0, 0) });
            return new CommandProcessor(world, new RuleRegistry(new[] { new TransferResourceRule() })).InspectCommitted();
        }
        [Test]
        public void FND_14_DefinitionOrderDoesNotAffectHashButQuantityDoes()
        {
            var first = Compile(); var reversed = ContentCompiler.Compile(Pack(new[] { Template(), Commodity() }));
            var changed = ContentCompiler.Compile(Pack(new[] { Commodity(), Template(quantity: 4) }));
            Assert.That(first.ContentHash, Does.Match("^[0-9a-f]{64}$"));
            Assert.That(reversed.ContentHash, Is.EqualTo(first.ContentHash));
            Assert.That(changed.ContentHash, Is.Not.EqualTo(first.ContentHash));
        }
        [Test]
        public void FND_14_InputCollectionMutationCannotAlterCompiledRegistry()
        {
            var definitions = new List<ContentDefinitionInput> { Commodity(), Template() };
            var dependencies = new List<ContentDependency>(); var input = Pack(definitions, dependencies);
            var registry = ContentCompiler.Compile(input); var hash = registry.ContentHash;
            definitions.Clear(); definitions.Add(Commodity("commodity.stone@1"));
            dependencies.Add(new ContentDependency("absent.pack", 1, new string('a', 64)));
            Assert.That(registry.Definitions.Count, Is.EqualTo(2));
            Assert.That(registry.Dependencies.Count, Is.Zero);
            Assert.That(registry.ContentHash, Is.EqualTo(hash));
            Assert.DoesNotThrow(() => registry.ValidateWorld(Initial().World));
        }
        [Test]
        public void FND_14_DuplicateDefinitionsAreRejected()
        {
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { Commodity(), Commodity() })));
        }
        [TestCase("commodity.absent@1")]
        [TestCase("commodity.timber@2")]
        public void FND_14_MissingExactCommodityReferenceIdentifiesTemplate(string id)
        {
            var error = Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { Commodity(), Template(id) })))!;
            Assert.That(error.DefinitionId, Is.EqualTo("transfer.timber@1"));
        }
        [TestCase("RepairBridge@1")]
        [TestCase("TransferResource@2")]
        [TestCase("eval(world.lots[0]=0)")]
        public void FND_14_UnknownHandlerOrScriptRejectedWithOffendingDefinition(string handler)
        {
            var error = Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { Commodity(), Template(handler: handler) })))!;
            Assert.That(error.DefinitionId, Is.EqualTo("transfer.timber@1"));
        }
        [TestCase(0L)] [TestCase(-1L)]
        public void FND_14_NonpositiveTemplateQuantityRejected(long quantity)
        {
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { Commodity(), Template(quantity: quantity) })));
        }
        [TestCase(0)] [TestCase(2)]
        public void FND_14_UnsupportedPackSchemaRejected(int schema)
        {
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(schema: schema)));
        }
        [Test]
        public void FND_14_UnknownRuleVersionRejected()
        {
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(rules: "future")));
        }
        [Test]
        public void FND_14_DefinitionVersionAndSchemaMustMatchSupportedContract()
        {
            var mismatch = new ContentDefinitionInput("commodity.timber@2", 1, "commodity", 1, "prototype-authored", "game-canon", "Assets", "timber", unit: "count");
            var schema = new ContentDefinitionInput("commodity.timber@1", 1, "commodity", 2, "prototype-authored", "game-canon", "Assets", "timber", unit: "count");
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { mismatch })));
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { schema })));
        }
        [Test]
        public void FND_14_DependencyMustBeDeclaredAndAvailableAtExactHashAndVersion()
        {
            var dependency = ContentCompiler.Compile(Pack(new[] { Commodity() }, id: "test.base"));
            var reference = new ContentDependency(dependency.PackId, dependency.Version, dependency.ContentHash);
            var input = Pack(new[] { Template() }, new[] { reference });
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(input));
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { Template() }), new[] { dependency }));
            Assert.DoesNotThrow(() => ContentCompiler.Compile(input, new[] { dependency }).ValidateWorld(Initial().World));
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { Template() }, new[] { new ContentDependency(dependency.PackId, 2, dependency.ContentHash) }), new[] { dependency }));
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { Template() }, new[] { new ContentDependency(dependency.PackId, 1, new string('b', 64)) }), new[] { dependency }));
        }
        [Test]
        public void FND_14_SelfDependencyCannotCreateCycle()
        {
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(dependencies: new[] { new ContentDependency("test.kernel", 1, new string('a', 64)) })));
        }
        [TestCase("id")] [TestCase("version")] [TestCase("hash")]
        public void FND_14_CompatibilityRequiresExactIdentityVersionAndHash(string changed)
        {
            var registry = Compile();
            Assert.DoesNotThrow(() => registry.EnsureCompatible(registry.PackId, registry.Version, registry.ContentHash));
            Assert.Throws<ContentValidationException>(() => registry.EnsureCompatible(changed == "id" ? "test.other" : registry.PackId,
                changed == "version" ? 2 : registry.Version, changed == "hash" ? new string('b', 64) : registry.ContentHash));
        }
        [Test]
        public void FND_14_MissingWorldCommodityIsRejectedWithoutStateMutation()
        {
            var world = Initial("commodity.stone@1"); var before = CommittedWorldCodec.Encode(world);
            Assert.Throws<ContentValidationException>(() => Compile().ValidateWorld(world.World));
            Assert.That(CommittedWorldCodec.Encode(world), Is.EqualTo(before));
        }
        [Test]
        public void FND_14_CompiledTemplateProducesTypedCommandAndRuleControlsMutation()
        {
            var world = Initial(); var registry = Compile();
            var command = registry.CreateTransfer("transfer.timber@1", world.World, world.World.Lots[0].Id, world.World.Lots[1].Id);
            Assert.That(command.Quantity, Is.EqualTo(3));
            Assert.That(command.SourceLotId, Is.EqualTo(world.World.Lots[0].Id));
            Assert.That(world.World.Lots[0].OnHand, Is.EqualTo(10), "Binding content must not mutate the world");
            var result = new TransferResourceRule().Evaluate(world.World, world.World.Actors[0].Id, command);
            Assert.That(result.ErrorCode, Is.Null);
            Assert.That(result.Changes!.LotChanges.Select(x => x.NewOnHand), Is.EqualTo(new long[] { 7, 3 }));
        }
        private static ContentDependency Pin(ContentRegistry registry) => new ContentDependency(registry.PackId, registry.Version, registry.ContentHash);
        [Test]
        public void FND_14_DependencyOrderIsCanonicalAndDuplicateDeclarationsFail()
        {
            var timber = ContentCompiler.Compile(Pack(new[] { Commodity() }, id: "test.base"));
            var stone = ContentCompiler.Compile(Pack(new[] { Commodity("commodity.stone@1") }, id: "test.stone"));
            var available = new[] { timber, stone };
            var a = ContentCompiler.Compile(Pack(new[] { Template() }, new[] { Pin(timber), Pin(stone) }), available);
            var b = ContentCompiler.Compile(Pack(new[] { Template() }, new[] { Pin(stone), Pin(timber) }), available.Reverse());
            Assert.That(a.ContentHash, Is.EqualTo(b.ContentHash));
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { Template() }, new[] { Pin(timber), Pin(timber) }), available));
        }
        [Test]
        public void FND_14_TransitiveCycleAndConflictingDependencyClosureFail()
        {
            var oldBase = ContentCompiler.Compile(Pack(new[] { Commodity() }, id: "test.base"));
            var otherBase = ContentCompiler.Compile(Pack(new[] { Commodity(), Commodity("commodity.stone@1") }, id: "test.base"));
            var child = ContentCompiler.Compile(Pack(new[] { Template() }, new[] { Pin(oldBase) }, id: "test.child"), new[] { oldBase });
            var cycle = Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { Commodity("commodity.other@1") }, new[] { Pin(child) }, id: "test.base", version: 2), new[] { child }))!;
            Assert.That(cycle.Code, Is.EqualTo("CONTENT_DEPENDENCY_CYCLE"));
            var conflict = Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { Commodity("commodity.other@1") }, new[] { Pin(child), Pin(otherBase) }), new[] { child, otherBase }))!;
            Assert.That(conflict.Code, Is.EqualTo("CONTENT_DEPENDENCY_CONFLICT"));
        }
        [Test]
        public void FND_14_TransitiveReferenceResolvesButOverridingInheritedIdFails()
        {
            var basePack = ContentCompiler.Compile(Pack(new[] { Commodity() }, id: "test.base"));
            var child = ContentCompiler.Compile(Pack(new[] { Commodity("commodity.stone@1") }, new[] { Pin(basePack) }, id: "test.child"), new[] { basePack });
            var registry = ContentCompiler.Compile(Pack(new[] { Template() }, new[] { Pin(child) }), new[] { child });
            Assert.That(registry.Resolve("commodity.timber@1").Kind, Is.EqualTo("commodity"));
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { Commodity() }, new[] { Pin(child) }), new[] { child }));
            Assert.Throws<NotSupportedException>(() => ((IList<ContentDefinitionInput>)registry.Definitions).Clear());
        }
        [TestCase("source")] [TestCase("classification")] [TestCase("owner")] [TestCase("kind")] [TestCase("unit")]
        public void FND_14_UnsupportedOrMissingDefinitionMetadataRejected(string flaw)
        {
            var definition = new ContentDefinitionInput("commodity.timber@1", 1, flaw == "kind" ? "npc" : "commodity", 1,
                flaw == "source" ? "" : "prototype-authored", flaw == "classification" ? "unverified" : "game-canon",
                flaw == "owner" ? "Narrative" : "Assets", "timber", unit: flaw == "unit" ? "kg" : "count");
            Assert.Throws<ContentValidationException>(() => ContentCompiler.Compile(Pack(new[] { definition })));
        }
        [Test]
        public void FND_14_TemplateCannotBindDifferentCommodityOrCommodityAsTemplate()
        {
            var registry = ContentCompiler.Compile(Pack(new[] { Commodity(), Commodity("commodity.stone@1"), Template() }));
            var world = Initial("commodity.stone@1").World;
            var mismatch = Assert.Throws<ContentValidationException>(() => registry.CreateTransfer("transfer.timber@1", world, world.Lots[0].Id, world.Lots[1].Id))!;
            Assert.That(mismatch.Code, Is.EqualTo("CONTENT_TEMPLATE_COMMODITY"));
            Assert.Throws<ContentValidationException>(() => registry.CreateTransfer("commodity.timber@1", world, world.Lots[0].Id, world.Lots[1].Id));
        }
        [Test]
        public void FND_14_SaveCannotReopenWithChangedCompiledContentAndRemainsByteIdentical()
        {
            var folder = Path.Combine(Path.GetTempPath(), "mythical-content-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(folder);
            try
            {
                var path = Path.Combine(folder, "world.db"); var original = Compile();
                var changed = ContentCompiler.Compile(Pack(new[] { Commodity(), Template(quantity: 4) }));
                new SqliteWorldStore(path, original.ContentHash).InitializeSave(Initial()); var before = File.ReadAllBytes(path);
                Assert.Throws<InvalidDataException>(() => new SqliteWorldStore(path, changed.ContentHash).Load());
                Assert.That(File.ReadAllBytes(path), Is.EqualTo(before));
                Assert.That(new SqliteWorldStore(path, original.ContentHash).Load().World.Lots[0].OnHand, Is.EqualTo(10));
            }
            finally { Directory.Delete(folder, true); }
        }
    }
}
