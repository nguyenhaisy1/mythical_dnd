using System;
using System.Text.Json;

namespace Game.Headless
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--settlement-compile") return SettlementProbe.Run(args);
            if (args.Length > 0 && args[0] == "--conformance") return ConformanceProbe.Run(args);
            if (args.Length > 0 && (args[0] == "--diagnose" || args[0] == "--record-diagnostic-fixture")) return DiagnosticProbe.Run(args);
            if (args.Length > 0 && (args[0] == "--content-compile" || args[0] == "--content-demo" || args[0] == "--content-open")) return ContentProbe.Run(args);
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
                    stage = "settlement-authoring",
                    commandPipelineImplemented = true,
                    inMemorySchedulerImplemented = true,
                    durablePersistenceImplemented = true,
                    snapshotRecoveryImplemented = true,
                    commandReplayImplemented = true,
                    contentRegistryImplemented = true,
                    foundationDiagnosticsImplemented = true,
                    settlementAuthoringImplemented = true,
                    simulationImplemented = false,
                    runtimeLlm = false,
                    documentation = "docs/production/HANDOFF.md",
                    nextTask = "VS-02B"
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
