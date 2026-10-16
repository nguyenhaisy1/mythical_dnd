using System;

namespace Game.Foundation.Time
{
    /// <summary>Fictional 360-day campaign calendar, not a historical calendar or civil date.</summary>
    public static class SimulationCalendar
    {
        public const string Version = "clock-v1";
        public const long MinutesPerHour = 60;
        public const long MinutesPerDay = 1440;
        public const long MinutesPerMonth = 43200;
        public const long MinutesPerYear = 518400;

        public static SimulationDateTime FromTick(WorldTick tick)
        {
            long value = tick.Value;
            return new SimulationDateTime(value / MinutesPerYear + 1,
                (int)((value / MinutesPerMonth) % 12) + 1,
                (int)((value / MinutesPerDay) % 30) + 1,
                (int)((value / MinutesPerHour) % 24), (int)(value % 60));
        }

        public static WorldTick ToTick(SimulationDateTime date)
        {
            // Also validates default(SimulationDateTime), which bypasses its constructor.
            return ToTick(date.Year, date.Month, date.Day, date.Hour, date.Minute);
        }

        public static WorldTick ToTick(long year, int month, int day, int hour = 0, int minute = 0)
        {
            if (year < 1 || month < 1 || month > 12 || day < 1 || day > 30 ||
                hour < 0 || hour > 23 || minute < 0 || minute > 59)
                throw new TimeRangeException("date");
            try
            {
                return new WorldTick(checked((year - 1) * MinutesPerYear +
                    (month - 1) * MinutesPerMonth + (day - 1) * MinutesPerDay + hour * MinutesPerHour + minute));
            }
            catch (OverflowException) { throw new TimeRangeException("date"); }
        }
    }

    public readonly struct SimulationDateTime
    {
        public long Year { get; }
        public int Month { get; }
        public int Day { get; }
        public int Hour { get; }
        public int Minute { get; }
        public SimulationDateTime(long year, int month, int day, int hour = 0, int minute = 0)
        {
            SimulationCalendar.ToTick(year, month, day, hour, minute);
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
        }
    }
}
