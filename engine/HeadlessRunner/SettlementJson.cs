using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
namespace Game.Headless
{
    internal static class SettlementJson
    {
        internal static Dictionary<string,object?> Read(string path)
        {
            using var f=new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read);
            if(f.Length>1024*1024) throw new InvalidDataException("SETTLEMENT_FILE_SIZE");
            var bytes=new byte[checked((int)f.Length)]; f.ReadExactly(bytes);
            if(f.ReadByte()!=-1) throw new InvalidDataException("SETTLEMENT_FILE_CHANGED");
            using var doc=JsonDocument.Parse(bytes,new JsonDocumentOptions{MaxDepth=20});
            if(Convert(doc.RootElement) is not Dictionary<string,object?> root) throw new InvalidDataException("SETTLEMENT_ROOT");
            return root;
        }
        private static object? Convert(JsonElement node)
        {
            switch(node.ValueKind)
            {
                case JsonValueKind.Object:
                    var map=new Dictionary<string,object?>(StringComparer.Ordinal);
                    foreach(var p in node.EnumerateObject())
                        if(!map.TryAdd(p.Name,Convert(p.Value))) throw new InvalidDataException("SETTLEMENT_DUPLICATE_FIELD");
                    return map;
                case JsonValueKind.Array:return node.EnumerateArray().Select(Convert).ToArray();
                case JsonValueKind.String:return node.GetString();
                case JsonValueKind.Number:
                    if(node.TryGetInt64(out var n)) return n;
                    throw new InvalidDataException("SETTLEMENT_INTEGER");
                case JsonValueKind.True:return true;
                case JsonValueKind.False:return false;
                case JsonValueKind.Null:return null;
                default:throw new InvalidDataException("SETTLEMENT_JSON");
            }
        }
    }
}
