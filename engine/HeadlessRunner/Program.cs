using System;
using System.Text.Json;

namespace Game.Headless
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "--status")
            {
                Console.WriteLine(JsonSerializer.Serialize(new
                {
                    project = "mythical_dnd",
                    stage = "in-memory-scheduler",
                    commandPipelineImplemented = true,
                    inMemorySchedulerImplemented = true,
                    durablePersistenceImplemented = false,
                    simulationImplemented = false,
                    runtimeLlm = false,
                    documentation = "docs/production/HANDOFF.md",
                    nextTask = "FND-11"
                }, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }
            if (args.Length == 1 && args[0] == "--transfer-demo") return TransferDemo.Run();
            if (args.Length == 1 && args[0] == "--scheduler-demo") return SchedulerDemo.Run();
            Console.Error.WriteLine("Usage: Game.Headless --status | --transfer-demo | --scheduler-demo");
            Console.Error.WriteLine("Simulation and replay are not implemented. See docs/production/BACKLOG.csv.");
            return 2;
        }
    }
}
