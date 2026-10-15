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
                    stage = "foundation-identity",
                    simulationImplemented = false,
                    runtimeLlm = false,
                    documentation = "docs/production/HANDOFF.md",
                    nextTask = "FND-03"
                }, new JsonSerializerOptions { WriteIndented = true }));
                return 0;
            }
            Console.Error.WriteLine("Usage: Game.Headless --status");
            Console.Error.WriteLine("Simulation and replay are not implemented. See docs/production/BACKLOG.csv.");
            return 2;
        }
    }
}
