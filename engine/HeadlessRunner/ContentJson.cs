using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Game.Infrastructure.Content;

namespace Game.Headless
{
    /// <summary>Host authoring adapter. Portable compiler revalidates all semantic data.</summary>
    internal static class ContentJson
    {
        private const int MaximumBytes = 1024 * 1024;
        internal static ContentPackInput Read(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            long length = stream.Length;
            if (length > MaximumBytes) throw new InvalidDataException("CONTENT_SIZE: " + path);
            var bytes = new byte[checked((int)length)];
            stream.ReadExactly(bytes);
            if (stream.ReadByte() != -1) throw new InvalidDataException("CONTENT_FILE_CHANGED: " + path);
            using var document = JsonDocument.Parse(bytes, new JsonDocumentOptions { MaxDepth = 16 });
            var root = document.RootElement;
            Fields(root, "$", "pack_id", "version", "schema_version", "rule_version", "dependencies", "definitions");
            var dependencies = Array(root, "dependencies", "$", 64).Select((x, i) =>
            {
                string p = "$.dependencies[" + i + "]";
                Fields(x, p, "pack_id", "version", "content_hash");
                return new ContentDependency(Text(x, "pack_id", p), Number(x, "version", p), Text(x, "content_hash", p));
            }).ToArray();
            var definitions = Array(root, "definitions", "$", 4096).Select((x, i) => Definition(x, i)).ToArray();
            return new ContentPackInput(Text(root, "pack_id", "$"), Number(root, "version", "$"),
                Number(root, "schema_version", "$"), Text(root, "rule_version", "$"), dependencies, definitions);
        }

        private static ContentDefinitionInput Definition(JsonElement x, int index)
        {
            string p = "$.definitions[" + index + "]";
            if (x.ValueKind == JsonValueKind.Object && x.TryGetProperty("definition_id", out var id) && id.ValueKind == JsonValueKind.String)
                p += "(" + id.GetString() + ")";
            string kind = Text(x, "kind", p);
            var common = new[] { "definition_id", "version", "kind", "schema_version", "source", "classification", "owner_domain", "display_key" };
            if (kind == "commodity") Fields(x, p, common.Concat(new[] { "unit" }).ToArray());
            else if (kind == "transfer-template") Fields(x, p, common.Concat(new[] { "commodity_id", "handler", "quantity" }).ToArray());
            else throw new InvalidDataException("CONTENT_KIND: " + p);
            return new ContentDefinitionInput(Text(x, "definition_id", p), Number(x, "version", p), kind,
                Number(x, "schema_version", p), Text(x, "source", p), Text(x, "classification", p),
                Text(x, "owner_domain", p), Text(x, "display_key", p),
                kind == "commodity" ? Text(x, "unit", p) : null,
                kind == "transfer-template" ? Text(x, "commodity_id", p) : null,
                kind == "transfer-template" ? Text(x, "handler", p) : null,
                kind == "transfer-template" ? Long(x, "quantity", p) : (long?)null);
        }

        private static void Fields(JsonElement value, string path, params string[] allowed)
        {
            if (value.ValueKind != JsonValueKind.Object) throw new InvalidDataException("CONTENT_OBJECT: " + path);
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var property in value.EnumerateObject())
            {
                if (!seen.Add(property.Name)) throw new InvalidDataException("CONTENT_DUPLICATE_FIELD: " + path + "." + property.Name);
                if (!allowed.Contains(property.Name, StringComparer.Ordinal)) throw new InvalidDataException("CONTENT_UNKNOWN_FIELD: " + path + "." + property.Name);
            }
            foreach (var name in allowed)
                if (!seen.Contains(name)) throw new InvalidDataException("CONTENT_REQUIRED_FIELD: " + path + "." + name);
        }
        private static string Text(JsonElement value, string name, string path)
        {
            if (value.ValueKind != JsonValueKind.Object || !value.TryGetProperty(name, out var field) || field.ValueKind != JsonValueKind.String)
                throw new InvalidDataException("CONTENT_STRING: " + path + "." + name);
            string text = field.GetString()!;
            if (text.Length == 0 || text.Length > 1024) throw new InvalidDataException("CONTENT_STRING_SIZE: " + path + "." + name);
            return text;
        }
        private static long Long(JsonElement value, string name, string path)
        {
            if (!value.TryGetProperty(name, out var field) || field.ValueKind != JsonValueKind.Number || !field.TryGetInt64(out var number))
                throw new InvalidDataException("CONTENT_INTEGER: " + path + "." + name);
            return number;
        }
        private static int Number(JsonElement value, string name, string path)
        {
            long number = Long(value, name, path);
            if (number < 1 || number > int.MaxValue) throw new InvalidDataException("CONTENT_VERSION: " + path + "." + name);
            return (int)number;
        }
        private static JsonElement[] Array(JsonElement value, string name, string path, int maximum)
        {
            if (!value.TryGetProperty(name, out var field) || field.ValueKind != JsonValueKind.Array || field.GetArrayLength() > maximum)
                throw new InvalidDataException("CONTENT_ARRAY: " + path + "." + name);
            return field.EnumerateArray().ToArray();
        }
    }
}
