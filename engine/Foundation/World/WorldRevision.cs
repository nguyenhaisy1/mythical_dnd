using System;
using System.Globalization;

namespace Game.Foundation.World
{
    public readonly struct WorldRevision : IEquatable<WorldRevision>, IComparable<WorldRevision>
    {
        public ulong Value { get; }
        public WorldRevision(ulong value) { Value = value; }
        public WorldRevision Next() => new WorldRevision(checked(Value + 1));
        public int CompareTo(WorldRevision other) => Value.CompareTo(other.Value);
        public bool Equals(WorldRevision other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is WorldRevision other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
        public static bool operator ==(WorldRevision a, WorldRevision b) => a.Equals(b);
        public static bool operator !=(WorldRevision a, WorldRevision b) => !a.Equals(b);
    }
}
