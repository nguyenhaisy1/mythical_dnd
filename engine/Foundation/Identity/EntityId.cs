using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Game.Foundation.Identity
{
    public sealed class IdentityException : InvalidOperationException
    {
        public string Code { get; }
        public IdentityException(string code) : base(code) { Code = code; }
    }

    // Closed v1 vocabulary: adding a kind requires a versioned contract change.
    public sealed class Actor { private Actor() { } }
    public sealed class Household { private Household() { } }
    public sealed class Item { private Item() { } }
    public sealed class Lot { private Lot() { } }
    public sealed class Activity { private Activity() { } }
    public sealed class Job { private Job() { } }
    public sealed class Event { private Event() { } }
    public sealed class Route { private Route() { } }
    public sealed class Settlement { private Settlement() { } }
    public sealed class StoryThread { private StoryThread() { } }

    public static class EntityKinds
    {
        private static readonly Dictionary<Type, string> Tags = new Dictionary<Type, string>
        {
            [typeof(Actor)] = "actor", [typeof(Household)] = "household",
            [typeof(Item)] = "item", [typeof(Lot)] = "lot", [typeof(Activity)] = "activity",
            [typeof(Job)] = "job", [typeof(Event)] = "event", [typeof(Route)] = "route",
            [typeof(Settlement)] = "settlement", [typeof(StoryThread)] = "thread"
        };
        public static IReadOnlyList<string> All { get; } = Array.AsReadOnly(new[]
            { "activity", "actor", "event", "household", "item", "job", "lot", "route", "settlement", "thread" });
        public static string For<T>() => Tags.TryGetValue(typeof(T), out var tag)
            ? tag : throw new IdentityException("REF_TYPE_MISMATCH");
        public static bool IsKnown(string tag) => tag != null && Tags.ContainsValue(tag);
    }

    public sealed class EntityId : IEquatable<EntityId>, IComparable<EntityId>
    {
        private static readonly Regex Wire = new Regex(@"\Aw([0-9a-f]{16})/([a-z]+)/([0-9a-f]{16})\z", RegexOptions.CultureInvariant);
        public ulong World { get; }
        public string Kind { get; }
        public ulong Counter { get; }
        public EntityId(ulong world, string kind, ulong counter)
        {
            if (world == 0 || counter == 0 || !EntityKinds.IsKnown(kind)) throw new FormatException("Invalid identity-v1 ID");
            World = world; Kind = kind; Counter = counter;
        }
        public static EntityId Parse(string wire)
        {
            if (wire == null) throw new ArgumentNullException(nameof(wire));
            var match = Wire.Match(wire);
            if (!match.Success) throw new FormatException("Noncanonical identity-v1 ID");
            return new EntityId(ulong.Parse(match.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture),
                match.Groups[2].Value, ulong.Parse(match.Groups[3].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
        }
        public override string ToString() => "w" + World.ToString("x16", CultureInfo.InvariantCulture) + "/" + Kind + "/" + Counter.ToString("x16", CultureInfo.InvariantCulture);
        public int CompareTo(EntityId? other)
        {
            if (other is null) return 1;
            var world = World.CompareTo(other.World);
            if (world != 0) return world;
            var kind = StringComparer.Ordinal.Compare(Kind, other.Kind);
            return kind != 0 ? kind : Counter.CompareTo(other.Counter);
        }
        public bool Equals(EntityId? other) => other != null && World == other.World && Kind == other.Kind && Counter == other.Counter;
        public override bool Equals(object? obj) => obj is EntityId id && Equals(id);
        public override int GetHashCode() => HashCode.Combine(World, Kind, Counter);
    }

    public sealed class EntityId<T> : IEquatable<EntityId<T>>
    {
        public EntityId Value { get; }
        public EntityId(EntityId value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Kind != EntityKinds.For<T>()) throw new IdentityException("REF_TYPE_MISMATCH");
            Value = value;
        }
        public static EntityId<T> Parse(string wire) => new EntityId<T>(EntityId.Parse(wire));
        public override string ToString() => Value.ToString();
        public bool Equals(EntityId<T>? other) => other != null && Value.Equals(other.Value);
        public override bool Equals(object? obj) => obj is EntityId<T> id && Equals(id);
        public override int GetHashCode() => Value.GetHashCode();
    }
}
