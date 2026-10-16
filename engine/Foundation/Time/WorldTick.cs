using System;
using System.Globalization;

namespace Game.Foundation.Time
{
    public sealed class TimeRangeException : ArgumentOutOfRangeException
    {
        public string Code => "TIME_RANGE";
        public TimeRangeException(string parameter) : base(parameter, "Time must be within the clock-v1 range.") { }
    }

    /// <summary>Immutable simulation minute. This value never reads or advances with wall-clock time.</summary>
    public readonly struct WorldTick : IEquatable<WorldTick>, IComparable<WorldTick>
    {
        public long Value { get; }
        public WorldTick(long value)
        {
            if (value < 0) throw new TimeRangeException(nameof(value));
            Value = value;
        }
        public WorldTick Add(long minutes)
        {
            if (minutes < 0 || minutes > long.MaxValue - Value) throw new TimeRangeException(nameof(minutes));
            return new WorldTick(checked(Value + minutes));
        }
        public WorldTick Add(WorkDuration duration)
        {
            // default(WorkDuration) must not bypass the positive-duration invariant.
            if (duration.Minutes < 1) throw new TimeRangeException(nameof(duration));
            return Add(duration.Minutes);
        }
        public int CompareTo(WorldTick other) => Value.CompareTo(other.Value);
        public bool Equals(WorldTick other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is WorldTick other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
        public static bool operator ==(WorldTick left, WorldTick right) => left.Equals(right);
        public static bool operator !=(WorldTick left, WorldTick right) => !left.Equals(right);
        public static bool operator <(WorldTick left, WorldTick right) => left.Value < right.Value;
        public static bool operator >(WorldTick left, WorldTick right) => left.Value > right.Value;
        public static bool operator <=(WorldTick left, WorldTick right) => left.Value <= right.Value;
        public static bool operator >=(WorldTick left, WorldTick right) => left.Value >= right.Value;
    }

    /// <summary>Scheduled work takes at least one simulation minute; instant commands use no duration.</summary>
    public readonly struct WorkDuration
    {
        public long Minutes { get; }
        public WorkDuration(long minutes)
        {
            if (minutes < 1) throw new TimeRangeException(nameof(minutes));
            Minutes = minutes;
        }
    }
}
