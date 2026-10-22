using System;
using System.IO;
using System.Text.Json;
using Game.Infrastructure.Conformance;
namespace Game.Headless
{
    internal static class ConformanceProbe
    {
        internal static int Run(string[] args)
        {
            if (args.Length != 3) { Console.Error.WriteLine("Usage: --conformance <replay-artifact> <new-db>"); return 2; }
            try
            {
                var result = RuntimeConformance.Run(args[1], args[2]);
                Console.WriteLine(JsonSerializer.Serialize(new { status = "Passed", profile = RuntimeConformance.Profile,
                    checks = result.Checks, fingerprint = result.Fingerprint, sqliteVersion = result.SqliteVersion,
                    runtime = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription, unityVersion = (string?)null }));
                return 0;
            }
            catch (Exception e) when (e is InvalidDataException || e is IOException || e is ArgumentException)
            { Console.Error.WriteLine(JsonSerializer.Serialize(new { status = "Failed", error = e.Message })); return 1; }
        }
    }
}
