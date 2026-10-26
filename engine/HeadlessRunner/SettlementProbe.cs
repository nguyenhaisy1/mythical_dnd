using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Infrastructure.SettlementContent;
namespace Game.Headless
{
    internal static class SettlementProbe
    {
        internal static int Run(string[] args)
        {
            if(args.Length!=2) { Console.Error.WriteLine("Usage: --settlement-compile <pack>"); return 2; }
            try
            {
                var registry=SettlementCompiler.Compile(SettlementJson.Read(args[1]));
                var kinds=new[]{"actor","community","fact","household","relationship","route"};
                Console.WriteLine(JsonSerializer.Serialize(new {passed=true,profile=SettlementCompiler.Profile,contentHash=registry.ContentHash,
                    counts=kinds.ToDictionary(k=>k,k=>registry.ByKind(k).Count),definitionIds=registry.Definitions.Select(d=>(string)d["definition_id"]!).ToArray(),
                    scope="authoring registry; no WorldState, player projection or executable routines"}));
                return 0;
            }
            catch(Exception e) when(e is InvalidDataException || e is JsonException || e is IOException || e is ArgumentException)
            { Console.Error.WriteLine(JsonSerializer.Serialize(new {passed=false,error=e.Message})); return 1; }
        }
    }
}
