using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game.Application.CommandPipeline;
using Game.Infrastructure.Persistence;

namespace Game.Infrastructure.Diagnostics
{
    public enum DiagnosticValueKind { Missing, Null, Boolean, Integer, String, Object, Array }
    public sealed class DiagnosticValue
    {
        public DiagnosticValueKind Kind { get; }
        public object? Value { get; }
        internal DiagnosticValue(DiagnosticValueKind kind, object? value = null) { Kind = kind; Value = value; }
    }
    public sealed class DiffEntry
    {
        public string Path { get; }
        public DiagnosticValue Before { get; }
        public DiagnosticValue After { get; }
        internal DiffEntry(string path, DiagnosticValue before, DiagnosticValue after) { Path = path; Before = before; After = after; }
    }
    public sealed class StateDiff
    {
        public ulong BeforeRevision { get; }
        public ulong AfterRevision { get; }
        public IReadOnlyList<DiffEntry> Entries { get; }
        public int TotalChanges { get; }
        public bool Truncated => TotalChanges > Entries.Count;
        internal StateDiff(ulong before, ulong after, IEnumerable<DiffEntry> entries, int total)
        { BeforeRevision = before; AfterRevision = after; Entries = Array.AsReadOnly(entries.ToArray()); TotalChanges = total; }
    }
    /// <summary>Developer-only full committed image diff. Array paths are positional JSON pointers, not entity identities.</summary>
    public static class StateDiffer
    {
        public static StateDiff Compare(CommittedWorld before, CommittedWorld after, int maxEntries = 1000)
        {
            if (before == null || after == null) throw new ArgumentNullException();
            if (maxEntries < 1 || maxEntries > 100000) throw new ArgumentOutOfRangeException(nameof(maxEntries));
            var left = new Dictionary<string, DiagnosticValue>(StringComparer.Ordinal);
            var right = new Dictionary<string, DiagnosticValue>(StringComparer.Ordinal);
            Flatten(MdcReader.Decode(CommittedWorldCodec.Encode(before)), "", left);
            Flatten(MdcReader.Decode(CommittedWorldCodec.Encode(after)), "", right);
            var entries = new List<DiffEntry>(); var total = 0;
            foreach (var path in left.Keys.Union(right.Keys, StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal))
            {
                if (!left.TryGetValue(path, out var a)) a = new DiagnosticValue(DiagnosticValueKind.Missing);
                if (!right.TryGetValue(path, out var b)) b = new DiagnosticValue(DiagnosticValueKind.Missing);
                if (a.Kind == b.Kind && Equals(a.Value, b.Value)) continue;
                total++;
                if (entries.Count < maxEntries) entries.Add(new DiffEntry(path, a, b));
            }
            return new StateDiff(before.World.Revision.Value, after.World.Revision.Value, entries, total);
        }
        private static void Flatten(object? value, string path, Dictionary<string, DiagnosticValue> output)
        {
            if (value is Dictionary<string, object?> map)
            {
                output.Add(path, new DiagnosticValue(DiagnosticValueKind.Object));
                foreach (var item in map) Flatten(item.Value, path + "/" + item.Key.Replace("~", "~0").Replace("/", "~1"), output);
            }
            else if (value is IList<object?> array)
            {
                output.Add(path, new DiagnosticValue(DiagnosticValueKind.Array, array.Count));
                for (var i = 0; i < array.Count; i++) Flatten(array[i], path + "/" + i.ToString(CultureInfo.InvariantCulture), output);
            }
            else output.Add(path, new DiagnosticValue(value == null ? DiagnosticValueKind.Null : value is bool ? DiagnosticValueKind.Boolean :
                value is string ? DiagnosticValueKind.String : DiagnosticValueKind.Integer, value));
        }
    }
}
