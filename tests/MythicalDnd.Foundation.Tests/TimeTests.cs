using System;
using Game.Foundation.Time;
using NUnit.Framework;

namespace MythicalDnd.Foundation.Tests
{
    [TestFixture]
    public sealed class TimeTests
    {
        private static void RangeError(TestDelegate action) =>
            Assert.That(Assert.Throws<TimeRangeException>(action)!.Code, Is.EqualTo("TIME_RANGE"));

        [TestCase(0L, 1L, 1, 1, 0, 0)]
        [TestCase(59L, 1L, 1, 1, 0, 59)]
        [TestCase(60L, 1L, 1, 1, 1, 0)]
        [TestCase(1439L, 1L, 1, 1, 23, 59)]
        [TestCase(1440L, 1L, 1, 2, 0, 0)]
        [TestCase(43199L, 1L, 1, 30, 23, 59)]
        [TestCase(43200L, 1L, 2, 1, 0, 0)]
        [TestCase(518399L, 1L, 12, 30, 23, 59)]
        [TestCase(518400L, 2L, 1, 1, 0, 0)]
        [TestCase(12960000L, 26L, 1, 1, 0, 0)]
        public void TEST_008_CalendarBoundaries(long tick, long year, int month, int day, int hour, int minute)
        {
            var date = SimulationCalendar.FromTick(new WorldTick(tick));
            Assert.That(new[] { date.Year, date.Month, date.Day, date.Hour, date.Minute },
                Is.EqualTo(new[] { year, month, day, hour, minute }));
            Assert.That(SimulationCalendar.ToTick(date).Value, Is.EqualTo(tick));
        }

        [Test]
        public void TEST_008_FullRangeAndMaximumDate()
        {
            var maximum = new WorldTick(long.MaxValue);
            var date = SimulationCalendar.FromTick(maximum);
            Assert.That(SimulationCalendar.ToTick(date), Is.EqualTo(maximum));
            Assert.That(date.Year, Is.GreaterThan(int.MaxValue));
            // Independently decompose using decimal arithmetic (no floating point precision loss).
            decimal minutes = (decimal)(date.Year - 1) * 518400 + (date.Month - 1) * 43200 +
                (date.Day - 1) * 1440 + date.Hour * 60 + date.Minute;
            Assert.That(minutes, Is.EqualTo((decimal)long.MaxValue));
            RangeError(() => maximum.Add(1));
            RangeError(() => SimulationCalendar.ToTick(date.Year, date.Month, date.Day, date.Hour, date.Minute + 1));
            RangeError(() => SimulationCalendar.ToTick(date.Year + 1, 1, 1));
            RangeError(() => SimulationCalendar.ToTick(long.MaxValue, 1, 1));
        }

        [Test]
        public void TEST_008_CheckedDurationAndInstantNoChange()
        {
            var start = new WorldTick(1439);
            Assert.That(start.Add(new WorkDuration(1)).Value, Is.EqualTo(1440));
            Assert.That(start.Add(0), Is.EqualTo(start));
            Assert.That(start.Value, Is.EqualTo(1439));
            Assert.That(default(WorldTick).Value, Is.Zero);
            Assert.That(default(WorldTick).Add(new WorkDuration(long.MaxValue)).Value, Is.EqualTo(long.MaxValue));
            Assert.That(new WorldTick(long.MaxValue - 1).Add(1).Value, Is.EqualTo(long.MaxValue));
            RangeError(() => new WorldTick(-1));
            RangeError(() => new WorldTick(long.MinValue));
            RangeError(() => start.Add(-1));
            RangeError(() => start.Add(long.MinValue));
            RangeError(() => start.Add(long.MaxValue));
            RangeError(() => new WorkDuration(0));
            RangeError(() => new WorkDuration(-1));
            RangeError(() => start.Add(default(WorkDuration)));
        }

        [TestCase(0L, 1, 1, 0, 0)]
        [TestCase(-1L, 1, 1, 0, 0)]
        [TestCase(1L, 0, 1, 0, 0)]
        [TestCase(1L, 13, 1, 0, 0)]
        [TestCase(1L, 1, 0, 0, 0)]
        [TestCase(1L, 1, 31, 0, 0)]
        [TestCase(1L, 1, 1, -1, 0)]
        [TestCase(1L, 1, 1, 24, 0)]
        [TestCase(1L, 1, 1, 0, -1)]
        [TestCase(1L, 1, 1, 0, 60)]
        public void TEST_008_RejectInvalidCalendar(long year, int month, int day, int hour, int minute)
        {
            RangeError(() => SimulationCalendar.ToTick(year, month, day, hour, minute));
            RangeError(() => new SimulationDateTime(year, month, day, hour, minute));
        }

        [Test]
        public void TEST_008_DefaultCalendarIsInvalid() => RangeError(() => SimulationCalendar.ToTick(default));

        [Test]
        public void TEST_008_OrderingAndEquality()
        {
            var a = new WorldTick(42);
            var b = new WorldTick(43);
            Assert.That(a == new WorldTick(42), Is.True);
            Assert.That(a != b, Is.True);
            Assert.That(a < b && a <= b && b > a && b >= a, Is.True);
            Assert.That(a.CompareTo(b), Is.LessThan(0));
            Assert.That(a.Equals((object)new WorldTick(42)), Is.True);
            Assert.That(a.Equals((object)42L), Is.False);
            Assert.That(a.GetHashCode(), Is.EqualTo(new WorldTick(42).GetHashCode()));
        }

        [Test]
        public void TEST_008_RoundTripAcrossLongCampaignAndRange()
        {
            for (long day = 0; day < 360 * 100; day++)
            {
                long tick = day * 1440 + 1439;
                Assert.That(SimulationCalendar.ToTick(SimulationCalendar.FromTick(new WorldTick(tick))).Value, Is.EqualTo(tick));
            }
            for (long divisor = 1; divisor <= 1000; divisor++)
            {
                long tick = long.MaxValue / divisor;
                Assert.That(SimulationCalendar.ToTick(SimulationCalendar.FromTick(new WorldTick(tick))).Value, Is.EqualTo(tick));
            }
        }
    }
}
