using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Domain.Repair;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Repair;
using Game.Infrastructure.Replay;

namespace Game.Headless
{
    internal static class BoatRepairProbe
    {
        internal static int Run(string[] args)
        {
            if (args.Length != 4)
            { Console.Error.WriteLine("Usage: --repair-probe <profile-json> <scenario-json> <new-db>"); return 2; }
            try
            {
                var profile = RepairContentRegistry.Compile(BoatRepairJson.Read(args[1]));
                var scenario = BoatRepairJson.Read(args[2]);
                BoatRepairJson.Fields(scenario, "schema_version", "id", "scope", "world_id", "initial_tick", "initial_revision",
                    "timber_on_hand", "boat_condition", "seed", "profile_source", "source_contract", "expectations");
                if (BoatRepairJson.Number(scenario, "schema_version") != 1 || BoatRepairJson.Number(scenario, "initial_revision") != 1)
                    throw new InvalidDataException("REPAIR_SCENARIO_VERSION");
                var expectations = scenario["expectations"] as IReadOnlyDictionary<string, object?> ?? throw new InvalidDataException("REPAIR_SCENARIO_EXPECTATIONS");
                BoatRepairJson.Fields(expectations, "reserved_after_start", "on_hand_after_start", "condition_before_due", "due_tick",
                    "on_hand_after_completion", "reserved_after_completion", "condition_after_completion",
                    "on_hand_after_cancel", "reserved_after_cancel", "condition_after_cancel");
                long Expected(string key) => BoatRepairJson.Number(expectations, key);
                var path = Path.GetFullPath(args[3]); var cancelPath = path + ".cancel"; var recoveredPath = path + ".recovered";
                if (new[] { path, cancelPath, recoveredPath }.Any(File.Exists)) throw new IOException("REPAIR_PROBE_FILE_EXISTS");
                var stage = IdentitySnapshot.Empty(checked((ulong)BoatRepairJson.Number(scenario, "world_id"))).BeginStage(1);
                var actor = stage.Allocate<Actor>(); var owner = stage.Allocate<Household>(); var location = stage.Allocate<Settlement>();
                var material = stage.Allocate<Lot>(); var boat = stage.Allocate<Item>(); var tool = stage.Allocate<Item>();
                var repair = new RepairState(profile.Recipe, new[] {
                    new RepairItemState(boat, RepairItemKind.Boat, owner.Value, location, checked((int)BoatRepairJson.Number(scenario, "boat_condition"))),
                    new RepairItemState(tool, RepairItemKind.RepairTool, owner.Value, location, 1000)
                }, new[] { new RepairCapability(actor, true, tool) });
                var world = new WorldSnapshot(stage.PrepareSnapshot(), new WorldTick(BoatRepairJson.Number(scenario, "initial_tick")),
                    new[] { new ActorState(actor, true, location, new[] { owner.Value }) },
                    new[] { new ResourceLot(material, profile.Recipe.MaterialCommodity, owner.Value, location, BoatRepairJson.Number(scenario, "timber_on_hand"), 0) }, repair: repair);
                RuleRegistry Rules() => new RuleRegistry(new[] { new TransferResourceRule() });
                CommandProcessor Processor(CommittedWorld state, ICommittedWorldStore? store = null)
                    => new CommandProcessor(state, Rules(), store, repairRecipe: profile.Recipe);
                var initial = new CommandProcessor(world, Rules(), repairRecipe: profile.Recipe).InspectCommitted();
                var principal = new ExecutionPrincipal(actor, CommandSource.Player);
                CommandEnvelope Command(CommandProcessor processor, string id, string type, Dictionary<string, object?> payload)
                {
                    var w = processor.InspectCommitted().World;
                    return new CommandEnvelope(id, type, actor.ToString(), "Player", w.Tick.Value, w.Revision.Value, 1, payload, "repair-probe");
                }
                var startPayload = new Dictionary<string, object?> { ["actor_id"] = actor.ToString(), ["item_id"] = boat.ToString(),
                    ["material_lot_id"] = material.ToString(), ["recipe_id"] = profile.Recipe.Id.ToString() };
                var seed = BoatRepairJson.Text(scenario, "seed");
                var store = new SqliteWorldStore(path, profile.ContentHash, expectedRuleVersion: SqliteWorldStore.RepairRuleVersion);
                store.InitializeSave(initial, seed);
                var processor = Processor(store.Load(), store);
                var actions = new List<ReplayAction>();
                CommandReceipt Execute(CommandEnvelope command)
                { actions.Add(ReplayAction.Execute(command, principal)); return processor.Execute(command, principal); }
                var start = Command(processor, "probe-start", "StartRepair", startPayload);
                var first = Execute(start); var started = processor.InspectCommitted();
                if (first.Outcome != CommandOutcome.Accepted) throw new InvalidOperationException("REPAIR_START_REJECTED:" + first.ErrorCode);
                var startedActivity = started.World.Repair!.Activities.Single();
                bool startValid = started.World.GetLot(material).OnHand == Expected("on_hand_after_start") &&
                    started.World.GetLot(material).Reserved == Expected("reserved_after_start") && startedActivity.DueTick.Value == Expected("due_tick");
                store.CreateSnapshot(started);
                store = new SqliteWorldStore(path, profile.ContentHash, expectedRuleVersion: SqliteWorldStore.RepairRuleVersion);
                var loaded = store.Load(); processor = Processor(loaded, store);
                var retry = Execute(start);
                bool retryValid = ReferenceEquals(retry, loaded.AcceptedReceipts.Single(r => r.CommandId == start.CommandId));
                Execute(Command(processor, "probe-before-due", "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = Expected("due_tick") - 1 }));
                var beforeDue = processor.InspectCommitted();
                bool beforeValid = beforeDue.World.Repair!.GetItem(boat).Condition == Expected("condition_before_due") &&
                    beforeDue.World.GetLot(material).Reserved == Expected("reserved_after_start");
                Execute(Command(processor, "probe-advance-due", "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = Expected("due_tick") }));
                actions.Add(ReplayAction.Drain(principal)); processor.DrainAdvance(principal);
                var completed = processor.InspectCommitted();
                bool completeValid = completed.World.GetLot(material).OnHand == Expected("on_hand_after_completion") &&
                    completed.World.GetLot(material).Reserved == Expected("reserved_after_completion") &&
                    completed.World.Repair!.GetItem(boat).Condition == Expected("condition_after_completion") &&
                    completed.World.Repair.Activities.Single().Status == RepairActivityStatus.Completed;
                var forbidden = Execute(Command(processor, "probe-forged-complete", "CompleteRepair", new Dictionary<string, object?> { ["activity_id"] = startedActivity.Id.ToString() }));
                bool forbiddenValid = forbidden.Outcome == CommandOutcome.Rejected && forbidden.ErrorCode == "NOT_AUTHORIZED";
                var final = processor.InspectCommitted(); var finalImage = CommittedWorldCodec.Encode(final);
                bool reloadValid = CommittedWorldCodec.Encode(new SqliteWorldStore(path, profile.ContentHash, expectedRuleVersion: SqliteWorldStore.RepairRuleVersion).Load()).SequenceEqual(finalImage);
                var recoveredState = store.RecoverToNewFile(recoveredPath);
                bool recoveryValid = CommittedWorldCodec.Encode(recoveredState).SequenceEqual(finalImage) &&
                    CommittedWorldCodec.Encode(new SqliteWorldStore(recoveredPath, profile.ContentHash, expectedRuleVersion: SqliteWorldStore.RepairRuleVersion).Load()).SequenceEqual(finalImage);
                var originalFile = File.ReadAllBytes(path); bool refused = false;
                try { new SqliteWorldStore(path, profile.ContentHash).Load(); }
                catch (InvalidDataException) { refused = true; }
                bool profileRefusedWithoutWrite = refused && originalFile.SequenceEqual(File.ReadAllBytes(path));
                var cancelStore = new SqliteWorldStore(cancelPath, profile.ContentHash, expectedRuleVersion: SqliteWorldStore.RepairRuleVersion);
                cancelStore.InitializeSave(initial, seed); var cancelling = Processor(cancelStore.Load(), cancelStore);
                cancelling.Execute(Command(cancelling, "cancel-start", "StartRepair", startPayload), principal);
                var cancelActivity = cancelling.InspectCommitted().World.Repair!.Activities.Single();
                cancelling = Processor(cancelStore.Load(), cancelStore);
                var cancelledReceipt = cancelling.Execute(Command(cancelling, "cancel-command", "CancelActivity", new Dictionary<string, object?> {
                    ["activity_id"] = cancelActivity.Id.ToString(), ["reason"] = "probe-cancel" }), principal);
                var cancelled = cancelStore.Load();
                bool cancelValid = cancelledReceipt.Outcome == CommandOutcome.Accepted &&
                    cancelled.World.GetLot(material).OnHand == Expected("on_hand_after_cancel") &&
                    cancelled.World.GetLot(material).Reserved == Expected("reserved_after_cancel") &&
                    cancelled.World.Repair!.GetItem(boat).Condition == Expected("condition_after_cancel") &&
                    cancelled.World.Repair.Activities.Single().Status == RepairActivityStatus.Cancelled;
                var manifest = new ReplayManifest(profile.ContentHash, SqliteWorldStore.RepairRuleVersion, worldSeed: seed);
                var artifact = ReplayHarness.Record(initial, actions, manifest, Rules(), repairRecipe: profile.Recipe);
                var replay = ReplayHarness.Replay(artifact, manifest, Rules(), repairRecipe: profile.Recipe);
                bool replayValid = replay.Success && CommittedWorldCodec.Encode(replay.FinalState).SequenceEqual(finalImage);
                var firstCheckpoint = artifact.Checkpoints[0];
                var corrupted = new ReplayCheckpoint(firstCheckpoint.Revision, new string('0', 64), firstCheckpoint.GameplayHash,
                    firstCheckpoint.IntegrityHash, firstCheckpoint.JournalHash, firstCheckpoint.JobsHash);
                var negativeArtifact = new ReplayArtifact(manifest, artifact.InitialImage, artifact.Actions,
                    new[] { corrupted }.Concat(artifact.Checkpoints.Skip(1)));
                var negative = ReplayHarness.Replay(negativeArtifact, manifest, Rules(), repairRecipe: profile.Recipe);
                bool negativeDetected = !negative.Success && negative.Divergence!.Index == 0 && negative.Divergence.Category == "result";
                bool valid = startValid && retryValid && beforeValid && completeValid && forbiddenValid && reloadValid &&
                    recoveryValid && profileRefusedWithoutWrite && cancelValid && replayValid && negativeDetected;
                Console.WriteLine(JsonSerializer.Serialize(new {
                    scope = "headless-boat-repair-v1-not-Unity-or-full-profession", passed = valid,
                    contentHash = profile.ContentHash, recipeHash = profile.Recipe.Fingerprint,
                    ruleVersion = SqliteWorldStore.RepairRuleVersion, imageVersion = 2, sqliteSchemaVersion = store.GetSaveInfo().SchemaVersion,
                    timberAfterStart = started.World.GetLot(material).OnHand, reservedAfterStart = started.World.GetLot(material).Reserved,
                    dueTick = startedActivity.DueTick.Value, conditionBeforeDue = beforeDue.World.Repair!.GetItem(boat).Condition,
                    timberAfterCompletion = completed.World.GetLot(material).OnHand, reservedAfterCompletion = completed.World.GetLot(material).Reserved,
                    conditionAfterCompletion = completed.World.Repair!.GetItem(boat).Condition,
                    timberAfterCancel = cancelled.World.GetLot(material).OnHand, reservedAfterCancel = cancelled.World.GetLot(material).Reserved,
                    conditionAfterCancel = cancelled.World.Repair!.GetItem(boat).Condition,
                    startValid, retryValid, beforeValid, completeValid, forbiddenValid, reloadValid, recoveryValid,
                    profileRefusedWithoutWrite, cancelValid, replayValid, negativeDetected, replayActions = actions.Count,
                    finalRevision = final.World.Revision.Value, finalTick = final.World.Tick.Value,
                    finalImageHash = CommittedWorldCodec.Hash(finalImage), cancelledImageHash = CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(cancelled))
                }, new JsonSerializerOptions { WriteIndented = true }));
                return valid ? 0 : 1;
            }
            catch (Exception error) when (error is JsonException || error is ArgumentException || error is InvalidDataException || error is InvalidOperationException || error is IOException || error is FormatException)
            { Console.Error.WriteLine(error.GetType().Name + ": " + error.Message); return 1; }
        }
    }
}
