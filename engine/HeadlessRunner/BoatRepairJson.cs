using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Game.Headless
{
    /// <summary>Bounded authoring/fixture adapter; portable compilers validate semantics separately.</summary>
    internal static class BoatRepairJson
    {
        internal static IReadOnlyDictionary<string, object?> Read(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (stream.Length > 65536) throw new InvalidDataException("REPAIR_JSON_SIZE");
            var bytes = new byte[checked((int)stream.Length)]; stream.ReadExactly(bytes);
            if (stream.ReadByte() != -1) throw new InvalidDataException("REPAIR_JSON_CHANGED");
            using var document = JsonDocument.Parse(bytes, new JsonDocumentOptions { MaxDepth = 8 });
            return Object(document.RootElement);
        }
        private static Dictionary<string, object?> Object(JsonElement value)
        {
            if (value.ValueKind != JsonValueKind.Object) throw new InvalidDataException("REPAIR_JSON_OBJECT");
            var result = new Dictionary<string, object?>(StringComparer.Ordinal);
            foreach (var property in value.EnumerateObject())
            {
                if (result.ContainsKey(property.Name)) throw new InvalidDataException("REPAIR_JSON_DUPLICATE");
                object? parsed;
                switch (property.Value.ValueKind)
                {
                    case JsonValueKind.Object: parsed = Object(property.Value); break;
                    case JsonValueKind.String: parsed = property.Value.GetString(); break;
                    case JsonValueKind.Number:
                        if (!property.Value.TryGetInt64(out var number)) throw new InvalidDataException("REPAIR_JSON_INTEGER");
                        parsed = number; break;
                    case JsonValueKind.True: parsed = true; break;
                    case JsonValueKind.False: parsed = false; break;
                    default: throw new InvalidDataException("REPAIR_JSON_SCALAR");
                }
                result.Add(property.Name, parsed);
            }
            return result;
        }
        internal static void Fields(IReadOnlyDictionary<string, object?> map, params string[] fields)
        {
            if (map.Count != fields.Length || map.Keys.Any(k => !fields.Contains(k, StringComparer.Ordinal)))
                throw new InvalidDataException("REPAIR_SCENARIO_FIELDS");
        }
        internal static long Number(IReadOnlyDictionary<string, object?> map, string name)
            => map.TryGetValue(name, out var value) && value is long n ? n : throw new InvalidDataException("REPAIR_SCENARIO_INTEGER:" + name);
        internal static string Text(IReadOnlyDictionary<string, object?> map, string name)
            => map.TryGetValue(name, out var value) && value is string s && !string.IsNullOrWhiteSpace(s) ? s : throw new InvalidDataException("REPAIR_SCENARIO_STRING:" + name);
    }
}
