using System;
using System.Text.Json;

namespace Game.Headless
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if ((args.Length == 3 || args.Length == 4) &&
                (args[0] == "--replay" || args[0] == "--replay-negative-control" || args[0] == "--record-replay-fixture"))
                return ReplayProbe.Run(args[0], args[1], args[2], args.Length == 4 ? args[3] : null);
            if ((args.Length == 3 || args.Length == 4) && args[0] == "--save-probe") return SaveProbe.Run(args[1], args[2], args.Length == 4 ? args[3] : null);
            if ((args.Length == 3 || args.Length == 4) && args[0] == "--persistence-probe") return PersistenceProbe.Run(args[1],args[2],args.Length == 4 ? args[3] : null);
            if (args.Length == 1 && args[0] == "--status")
            {
                Console.WriteLine(JsonSerializer.Serialize(new
                {
                    project = "mythical_dnd",
                    stage = "headless-command-replay",
                    commandPipelineImplemented = true,
                    inMemorySchedulerImplemented = true,
                    durablePersistenceImplemented = true,
                    snapshotRecoveryImplemented = true,
                    commandReplayImplemented = true,
                    simulationImplemented = false,
                    runtimeLlm = false,
                    documentation = "docs/production/HANDOFF.md",
                    nextTask = "FND-14"
                }, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }
            if (args.Length == 1 && args[0] == "--transfer-demo") return TransferDemo.Run();
            if (args.Length == 1 && args[0] == "--scheduler-demo") return SchedulerDemo.Run();
            Console.Error.WriteLine("Usage: Game.Headless --status | --transfer-demo | --scheduler-demo | --replay <artifact> <content-hash> [seed]");
            Console.Error.WriteLine("Full simulation is not implemented; replay is a trusted QA diagnostic. See docs/production/BACKLOG.csv.");
            return 2;
        }
    }
}
