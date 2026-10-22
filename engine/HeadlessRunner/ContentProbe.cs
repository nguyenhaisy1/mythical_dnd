using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using Game.Infrastructure.Content;
using Game.Infrastructure.Persistence;

namespace Game.Headless
{
    internal static class ContentProbe
    {
        private static ContentRegistry Compile(string path, IEnumerable<string> dependencyPaths)
        {
            var paths = dependencyPaths.Take(65).ToArray();
            if (paths.Length > 64) throw new InvalidDataException("CONTENT_DEPENDENCY_LIMIT");
            var pending = paths.Select(ContentJson.Read).ToList();
            var root = ContentJson.Read(path);
            var ids = new HashSet<string>(StringComparer.Ordinal) { root.PackId };
            foreach (var input in pending)
                if (!ids.Add(input.PackId)) throw new ContentValidationException("CONTENT_DEPENDENCY_DUPLICATE", input.PackId);
            var compiled = new List<ContentRegistry>();
            while (pending.Count != 0)
            {
                var next = pending.FirstOrDefault(x => x.Dependencies.All(d => compiled.Any(c => c.PackId == d.PackId)));
                if (next == null) throw new InvalidDataException("CONTENT_DEPENDENCY_UNRESOLVED: missing or cyclic dependency");
                compiled.Add(ContentCompiler.Compile(next, compiled));
                pending.Remove(next);
            }
            return ContentCompiler.Compile(root, compiled);
        }

        internal static int Run(string[] args)
        {
            try
            {
                bool compileOnly = args[0] == "--content-compile";
                if (args.Length < (compileOnly ? 2 : 4)) throw new ArgumentException("CONTENT_ARGUMENTS");
                var registry = Compile(args[1], args.Skip(compileOnly ? 2 : 4));
                if (compileOnly)
                {
                    Console.WriteLine(JsonSerializer.Serialize(new { packId = registry.PackId, version = registry.Version,
                        contentHash = registry.ContentHash, schemaVersion = registry.SchemaVersion, ruleVersion = registry.RuleVersion,
                        definitionIds = registry.DefinitionIds, ruleReferences = registry.RuleReferences,
                        validationProfile = registry.ValidationProfile }));
                    return 0;
                }
                // Exact expected hash is supplied by the campaign/save selection, never silently replaced with latest.
                registry.EnsureCompatible(registry.PackId, registry.Version, args[3]);
                string database = args[2];
                var rules = registry.CreateRuleRegistry();
                if (args[0] == "--content-demo")
                {
                    if (File.Exists(database)) throw new IOException("CONTENT_DEMO_REQUIRES_NEW_FILE");
                    var ids = IdentitySnapshot.Empty(1).BeginStage(1);
                    var actor = ids.Allocate<Actor>();
                    var location = ids.Allocate<Settlement>();
                    var source = ids.Allocate<Lot>(); var destination = ids.Allocate<Lot>();
                    var world = new WorldSnapshot(ids.PrepareSnapshot(), new WorldTick(0),
                        new[] { new ActorState(actor, true, location) },
                        new[] { new ResourceLot(source, DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 10, 0),
                            new ResourceLot(destination, DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 0, 0) });
                    registry.ValidateWorld(world);
                    var transfer = registry.CreateTransfer("transfer.timber@1", world, source, destination);
                    var store = new SqliteWorldStore(database, registry.ContentHash);
                    store.InitializeSave(new CommandProcessor(world, rules).InspectCommitted(), new string('0', 64));
                    var processor = new CommandProcessor(store.Load(), rules, store);
                    var result = processor.Execute(new CommandEnvelope("content-transfer", transfer.CommandType, actor.ToString(), "Player", 0, 1,
                        transfer.PayloadVersion, new Dictionary<string, object?> { ["source_lot_id"] = transfer.SourceLotId.ToString(),
                            ["destination_lot_id"] = transfer.DestinationLotId.ToString(), ["quantity"] = transfer.Quantity }, "content-demo"),
                        new ExecutionPrincipal(actor, CommandSource.Player));
                    if (result.Outcome != CommandOutcome.Accepted) throw new InvalidDataException("CONTENT_DEMO_COMMAND_REJECTED");
                }
                else if (args[0] != "--content-open") throw new ArgumentException("CONTENT_MODE");
                if (!File.Exists(database)) throw new FileNotFoundException("CONTENT_SAVE_MISSING", database);
                var reopened = new SqliteWorldStore(database, registry.ContentHash).Load();
                registry.ValidateWorld(reopened.World);
                Console.WriteLine(JsonSerializer.Serialize(new { passed = true, contentHash = registry.ContentHash,
                    revision = reopened.World.Revision.Value, lots = reopened.World.Lots.Select(x => x.OnHand).ToArray(),
                    stateHash = CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(reopened)),
                    scope = "kernel-content-transfer-save" }));
                return 0;
            }
            catch (Exception error) when (error is ContentValidationException || error is JsonException ||
                error is IOException || error is InvalidDataException || error is ArgumentException || error is OverflowException)
            {
                Console.Error.WriteLine(JsonSerializer.Serialize(new { passed = false, error = error.Message }));
                return 1;
            }
        }
    }
}
