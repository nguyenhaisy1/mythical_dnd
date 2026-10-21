using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Random;
using Game.Foundation.Time;
using Game.Infrastructure.Persistence;
using Game.Infrastructure.Replay;

namespace Game.Headless
{
    internal static class ReplayProbe
    {
        private static RuleRegistry Rules() => new RuleRegistry(new[] { new TransferResourceRule() });
        private static ReplayArtifact Fixture(string contentHash)
        {
            var ids = IdentitySnapshot.Empty(1).BeginStage(1);
            var actor = ids.Allocate<Actor>();
            var other = ids.Allocate<Actor>();
            var location = ids.Allocate<Settlement>();
            var source = ids.Allocate<Lot>();
            var destination = ids.Allocate<Lot>();
            var seed = new string('0', 64);
            var rng = new RngStreamState("replay", new RngState(new RngContext(seed, "replay", "transfer", "one", actor.Value, "main"), 7));
            var world = new WorldSnapshot(ids.PrepareSnapshot(), new WorldTick(0),
                new[] { new ActorState(actor, true, location), new ActorState(other, true, location) },
                new[] { new ResourceLot(source, DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 10, 0),
                    new ResourceLot(destination, DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 0, 0) },
                new[] { rng });
            var initial = new CommandProcessor(world, Rules()).InspectCommitted();
            var principal = new ExecutionPrincipal(actor, CommandSource.Player);
            var unauthorized = new ExecutionPrincipal(other, CommandSource.Player);
            Dictionary<string, object?> Payload(long quantity) => new Dictionary<string, object?> {
                ["source_lot_id"] = source.ToString(), ["destination_lot_id"] = destination.ToString(), ["quantity"] = quantity };
            CommandEnvelope Command(string id, ulong revision, string type, Dictionary<string, object?> payload) =>
                new CommandEnvelope(id, type, actor.ToString(), "Player", 0, revision, 1, payload, "replay.golden");
            ReplayAction Execute(CommandEnvelope command) => ReplayAction.Execute(command, principal);
            var original = Command("transfer", 1, "TransferResource", Payload(3));
            var noop = Payload(3); noop["destination_lot_id"] = source.ToString();
            var actions = new List<ReplayAction> {
                ReplayAction.Drain(principal, 1),
                Execute(original), Execute(original),
                Execute(Command("transfer", 1, "TransferResource", Payload(4))),
                Execute(Command("unknown", 2, "Unknown", Payload(3))),
                Execute(Command("stale", 1, "TransferResource", Payload(3))),
                Execute(Command("noop", 2, "TransferResource", noop)),
                ReplayAction.Execute(Command("unauthorized", 2, "TransferResource", Payload(3)), unauthorized)
            };
            for (int i = 0; i < 2; i++)
            {
                var payload = Payload(2);
                payload["due_tick"] = 10L + i; payload["phase"] = 3; payload["priority"] = 100;
                actions.Add(Execute(Command("job." + i, (ulong)(2 + i), "ScheduleTransfer", payload)));
            }
            var advance = Command("advance", 4, "AdvanceTime", new Dictionary<string, object?> { ["target_tick"] = 20L });
            actions.Add(Execute(advance));
            actions.Add(ReplayAction.Drain(unauthorized, 1));
            actions.Add(ReplayAction.Drain(principal, 1));
            actions.Add(ReplayAction.Drain(principal, 1));
            actions.Add(Execute(advance));
            return ReplayHarness.Record(initial, actions, new ReplayManifest(contentHash, worldSeed: seed), Rules());
        }

        private static byte[] ReadBoundedArtifact(string path)
        {
            const int maximum = 64 * 1024 * 1024;
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var length = stream.Length;
            if (length > maximum) throw new InvalidDataException("REPLAY_SIZE");
            var bytes = new byte[checked((int)length)];
            int offset = 0;
            while (offset < bytes.Length)
            {
                int count = stream.Read(bytes, offset, bytes.Length - offset);
                if (count == 0) throw new InvalidDataException("REPLAY_TRUNCATED");
                offset += count;
            }
            if (stream.ReadByte() != -1) throw new InvalidDataException("REPLAY_FILE_CHANGED");
            return bytes;
        }

        public static int Run(string mode, string path, string contentHash, string? seed)
        {
            if (mode == "--record-replay-fixture")
            {
                var fixture = Fixture(contentHash);
                using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                var bytes = ReplayArtifactCodec.Encode(fixture);
                stream.Write(bytes, 0, bytes.Length);
                Console.WriteLine(JsonSerializer.Serialize(new { recorded = fixture.Actions.Count, sha256 = CommittedWorldCodec.Hash(bytes) }));
                return 0;
            }
            var artifact = ReplayArtifactCodec.Decode(ReadBoundedArtifact(path));
            if (artifact.Actions.Count == 0) throw new InvalidDataException("Replay CLI requires nonempty action log");
            if (mode == "--replay-negative-control")
            {
                var checkpoints = artifact.Checkpoints.ToArray();
                var first = checkpoints[0];
                checkpoints[0] = new ReplayCheckpoint(first.Revision, new string('0', 64), first.GameplayHash, first.IntegrityHash, first.JournalHash, first.JobsHash);
                artifact = new ReplayArtifact(artifact.Manifest, artifact.InitialImage, artifact.Actions, checkpoints);
            }
            var result = ReplayHarness.Replay(artifact, new ReplayManifest(contentHash, worldSeed: seed), Rules());
            var state = result.FinalState;
            Console.WriteLine(JsonSerializer.Serialize(new {
                passed = result.Success, actions = artifact.Actions.Count,
                stateHash = CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(state)),
                revision = state.World.Revision.Value, tick = state.World.Tick.Value,
                lots = state.World.Lots.Select(l => l.OnHand).ToArray(),
                executions = state.Scheduler.Executions.Count,
                rngDraws = state.World.RngStates.Select(r => r.State.DrawIndex).ToArray(),
                divergence = result.Divergence
            }));
            return result.Success ? 0 : 1;
        }
    }
}
