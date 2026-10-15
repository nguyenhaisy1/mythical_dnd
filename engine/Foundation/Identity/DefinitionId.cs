using System;
using System.Text.RegularExpressions;

namespace Game.Foundation.Identity
{
    public sealed class DefinitionId : IEquatable<DefinitionId>
    {
        private static readonly Regex Wire = new Regex(@"\A[a-z][a-z0-9-]*\.[a-z][a-z0-9-]*@[1-9][0-9]*\z", RegexOptions.CultureInvariant);
        private readonly string value;
        private DefinitionId(string value) { this.value = value; }
        public static DefinitionId Parse(string wire)
        {
            if (wire == null) throw new ArgumentNullException(nameof(wire));
            if (!Wire.IsMatch(wire)) throw new FormatException("Noncanonical definition ID");
            return new DefinitionId(wire);
        }
        public override string ToString() => value;
        public bool Equals(DefinitionId? other) => other != null && StringComparer.Ordinal.Equals(value, other.value);
        public override bool Equals(object? obj) => obj is DefinitionId id && Equals(id);
        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(value);
    }
}
