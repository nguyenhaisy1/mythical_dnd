using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Game.Foundation.Random
{
    /// <summary>mdc1 representation only. Schema validation and set ordering belong to the caller.</summary>
    public static class CanonicalCodec
    {
        private static readonly UTF8Encoding Utf8 = new UTF8Encoding(false, true);
        private static readonly Regex Key = new Regex(@"\A[a-z][a-z0-9_]*\z", RegexOptions.CultureInvariant);
        public static byte[] Encode(object? value)
        {
            var output = new List<byte>(Encoding.ASCII.GetBytes("MDC1"));
            Write(output, value);
            return output.ToArray();
        }
        public static string Hash(object? value)
        {
            using var sha = SHA256.Create();
            return string.Concat(sha.ComputeHash(Encode(value)).Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));
        }
        private static void Text(List<byte> output, string text) => output.AddRange(Utf8.GetBytes(text));
        private static void Write(List<byte> output, object? value)
        {
            switch (value)
            {
                case null: Text(output, "n"); return;
                case bool flag: Text(output, flag ? "t" : "f"); return;
                case string text:
                    var bytes = Utf8.GetBytes(text);
                    Text(output, "s" + bytes.Length.ToString(CultureInfo.InvariantCulture) + ":");
                    output.AddRange(bytes); return;
                case sbyte _: case byte _: case short _: case ushort _: case int _: case uint _: case long _: case ulong _:
                    Text(output, "i" + ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture) + ";"); return;
                case IEnumerable<KeyValuePair<string, object?>> pairs:
                    var fields = pairs.ToArray();
                    var seen = new HashSet<string>(StringComparer.Ordinal);
                    foreach (var pair in fields)
                        if (pair.Key == null || !Key.IsMatch(pair.Key) || !seen.Add(pair.Key))
                            throw new ArgumentException("Invalid or duplicate mdc1 object key");
                    Array.Sort(fields, (a, b) => StringComparer.Ordinal.Compare(a.Key, b.Key));
                    Text(output, "o" + fields.Length.ToString(CultureInfo.InvariantCulture) + ":");
                    foreach (var pair in fields) { Write(output, pair.Key); Write(output, pair.Value); }
                    return;
                case IReadOnlyList<object?> array:
                    Text(output, "a" + array.Count.ToString(CultureInfo.InvariantCulture) + ":");
                    foreach (var item in array) Write(output, item);
                    return;
                default: throw new ArgumentException("Unsupported mdc1 value type");
            }
        }
    }
}
