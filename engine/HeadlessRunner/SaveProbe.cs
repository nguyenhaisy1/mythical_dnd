using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Game.Application.CommandPipeline;
using Game.Domain.Kernel;
using Game.Foundation.Identity;
using Game.Foundation.Time;
using Game.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;

namespace Game.Headless
{
    // Host-only process crash seam. Wall-clock waiting never decides gameplay.
    internal static class SaveProbe
    {
        internal static int Run(string path, string mode, string? point)
        {
            var store = new SqliteWorldStore(path, new string('a', 64), step =>
            {
                if (point != null && step == point)
                {
                    Console.WriteLine("READY:" + step);
                    Console.Out.Flush();
                    Thread.Sleep(Timeout.Infinite);
                }
            });
            var rules = new RuleRegistry(new[] { new TransferResourceRule() });
            if (mode == "init" || mode == "init-legacy")
            {
                var ids = IdentitySnapshot.Empty(1).BeginStage(1);
                var actor = ids.Allocate<Actor>();
                var location = ids.Allocate<Settlement>();
                var source = ids.Allocate<Lot>();
                var destination = ids.Allocate<Lot>();
                var world = new WorldSnapshot(ids.PrepareSnapshot(), new WorldTick(0),
                    new[] { new ActorState(actor, true, location) },
                    new[] {
                        new ResourceLot(source, DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 10, 0),
                        new ResourceLot(destination, DefinitionId.Parse("commodity.timber@1"), actor.Value, location, 0, 0)
                    });
                var initial = new CommandProcessor(world, rules).InspectCommitted();
                if (mode == "init") store.InitializeSave(initial, new string('0', 64));
                else store.Initialize(initial);
                var processor = new CommandProcessor(store.Load(), rules, store);
                processor.Execute(new CommandEnvelope("save-transfer", "TransferResource", actor.ToString(), "Player", 0, 1, 1,
                    new Dictionary<string, object?> {
                        { "source_lot_id", source.ToString() }, { "destination_lot_id", destination.ToString() }, { "quantity", 3L }
                    }, "save-probe"), new ExecutionPrincipal(actor, CommandSource.Player));
            }
            else if (mode == "snapshot") store.CreateSnapshot(store.Load());
            else if (mode == "migrate") store.MigrateToNewFile(path + ".migrated", path + ".backup", new string('0', 64));
            else if (mode != "inspect") throw new ArgumentException("Unknown save probe mode");

            var current = store.Load();
            using var connection = new SqliteConnection(new SqliteConnectionStringBuilder {
                DataSource = path, Mode = SqliteOpenMode.ReadOnly, Pooling = false
            }.ToString());
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT schema_version FROM world_meta WHERE singleton=1";
            var schema = Convert.ToInt32(command.ExecuteScalar());
            long snapshots = 0;
            if (schema == 2)
            {
                command.CommandText = "SELECT count(*) FROM save_points";
                snapshots = Convert.ToInt64(command.ExecuteScalar());
            }
            Console.WriteLine(JsonSerializer.Serialize(new {
                stateHash = CommittedWorldCodec.Hash(CommittedWorldCodec.Encode(current)),
                revision = current.World.Revision.Value,
                source = current.World.Lots[0].OnHand,
                schema, snapshots
            }));
            return 0;
        }
    }
}
