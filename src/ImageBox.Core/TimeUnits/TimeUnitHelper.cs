namespace ImageBox.Core.TimeUnits;

/// <summary>
/// Helper utility for time units
/// </summary>
/// <remarks>Months are approximated to 30 days. Years do not account for leap years</remarks>
public static class TimeUnitHelper
{
    private static TimeUnitMap[]? _units;

    /// <summary>
    /// All of the available time units
    /// </summary>
    /// <returns></returns>
    public static TimeUnitMap[] Units()
    {
        return _units ??=
        [
            new TimeUnitMap(TimeUnitType.Millisecond, "ms"),
            new TimeUnitMap(TimeUnitType.Millisecond, "milli"),
            new TimeUnitMap(TimeUnitType.Millisecond, "millis"),
            new TimeUnitMap(TimeUnitType.Millisecond, "millisecond"),
            new TimeUnitMap(TimeUnitType.Millisecond, "milliseconds"),
            new TimeUnitMap(TimeUnitType.Second, "s"),
            new TimeUnitMap(TimeUnitType.Second, "sec"),
            new TimeUnitMap(TimeUnitType.Second, "secs"),
            new TimeUnitMap(TimeUnitType.Second, "second"),
            new TimeUnitMap(TimeUnitType.Second, "seconds"),
            new TimeUnitMap(TimeUnitType.Minute, "m"),
            new TimeUnitMap(TimeUnitType.Minute, "min"),
            new TimeUnitMap(TimeUnitType.Minute, "mins"),
            new TimeUnitMap(TimeUnitType.Minute, "minute"),
            new TimeUnitMap(TimeUnitType.Minute, "minutes"),
            new TimeUnitMap(TimeUnitType.Hour, "h"),
            new TimeUnitMap(TimeUnitType.Hour, "hr"),
            new TimeUnitMap(TimeUnitType.Hour, "hour"),
            new TimeUnitMap(TimeUnitType.Hour, "hours"),
            new TimeUnitMap(TimeUnitType.Day, "d"),
            new TimeUnitMap(TimeUnitType.Day, "day"),
            new TimeUnitMap(TimeUnitType.Day, "days"),
            new TimeUnitMap(TimeUnitType.Week, "w"),
            new TimeUnitMap(TimeUnitType.Week, "week"),
            new TimeUnitMap(TimeUnitType.Week, "weeks"),
            new TimeUnitMap(TimeUnitType.Month, "mo"),
            new TimeUnitMap(TimeUnitType.Month, "mon"),
            new TimeUnitMap(TimeUnitType.Month, "mons"),
            new TimeUnitMap(TimeUnitType.Month, "month"),
            new TimeUnitMap(TimeUnitType.Month, "months"),
            new TimeUnitMap(TimeUnitType.Quarter, "q"),
            new TimeUnitMap(TimeUnitType.Quarter, "qu"),
            new TimeUnitMap(TimeUnitType.Quarter, "quart"),
            new TimeUnitMap(TimeUnitType.Quarter, "quarter"),
            new TimeUnitMap(TimeUnitType.Quarter, "quarters"),
            new TimeUnitMap(TimeUnitType.Year, "y"),
            new TimeUnitMap(TimeUnitType.Year, "yr"),
            new TimeUnitMap(TimeUnitType.Year, "year"),
            new TimeUnitMap(TimeUnitType.Year, "years"),
            new TimeUnitMap(TimeUnitType.Decade, "de"),
            new TimeUnitMap(TimeUnitType.Decade, "dec"),
            new TimeUnitMap(TimeUnitType.Decade, "decade"),
            new TimeUnitMap(TimeUnitType.Decade, "decades"),
            new TimeUnitMap(TimeUnitType.Century, "c"),
            new TimeUnitMap(TimeUnitType.Century, "cent"),
            new TimeUnitMap(TimeUnitType.Century, "cents"),
            new TimeUnitMap(TimeUnitType.Century, "century"),
            new TimeUnitMap(TimeUnitType.Century, "centuries"),
            new TimeUnitMap(TimeUnitType.Millennium, "mi"),
            new TimeUnitMap(TimeUnitType.Millennium, "mill"),
            new TimeUnitMap(TimeUnitType.Millennium, "millennia"),
            new TimeUnitMap(TimeUnitType.Millennium, "millennium"),
            new TimeUnitMap(TimeUnitType.Millennium, "millenniums")
        ];
    }

    /// <summary>
    /// Gets the value and unit of a unit
    /// </summary>
    /// <param name="input">The given unit</param>
    /// <returns>The parsed value and unit</returns>
    public static TimeUnit ParseUnit(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));

        input = input.Trim().ToLower();

        if (input == "0") return TimeUnit.Zero;

        input = RegexUtility.UnitFilter(input);
        if (string.IsNullOrWhiteSpace(input)) return TimeUnit.Zero;

        var (value, unit) = RegexUtility.UnitParse(input);
        if (string.IsNullOrWhiteSpace(unit)) return new TimeUnit(TimeUnitType.Millisecond, value);

        var unitMatch = Units().FirstOrDefault(u => u.Symbol == unit);
        if (unitMatch is null) return new TimeUnit(TimeUnitType.Millisecond, value);

        return new TimeUnit(unitMatch.Type, value);
    }

    /// <summary>
    /// Serializes a unit into a string
    /// </summary>
    /// <param name="unit">The unit of time</param>
    /// <returns>The string representation of the time unit</returns>
    public static string SerializeUnit(TimeUnit unit)
    {
        if (unit == TimeUnit.Zero) return "0";

        var unitMatch = Units().FirstOrDefault(u => u.Type == unit.Type);
        if (unitMatch is null) return $"{unit.Value}ms";

        return $"{unit.Value}{unitMatch.Symbol}";
    }

    /// <summary>
    /// Converts the <see cref="TimeUnit"/> into the milliseconds equivalent
    /// </summary>
    /// <param name="unit">The unit to convert</param>
    /// <returns>The milliseconds equivalent of the given time</returns>
    public static double GetMilliseconds(TimeUnit unit)
    {
        if (unit.Value == 0) return 0;

        return unit.Type switch
        {
            TimeUnitType.Millisecond => unit.Value,
            TimeUnitType.Second => unit.Value * 1000,
            TimeUnitType.Minute => unit.Value * 60 * 1000,
            TimeUnitType.Hour => unit.Value * 60 * 60 * 1000,
            TimeUnitType.Day => unit.Value * 24 * 60 * 60 * 1000,
            TimeUnitType.Week => unit.Value * 7 * 24 * 60 * 60 * 1000,
            TimeUnitType.Month => unit.Value * 30 * 24 * 60 * 60 * 1000,
            TimeUnitType.Quarter => unit.Value * 4 * 30 * 24 * 60 * 60 * 1000,
            TimeUnitType.Year => unit.Value * 365 * 24 * 60 * 60 * 1000,
            TimeUnitType.Decade => unit.Value * 10 * 365 * 24 * 60 * 60 * 1000,
            TimeUnitType.Century => unit.Value * 100 * 365 * 24 * 60 * 60 * 1000,
            TimeUnitType.Millennium => unit.Value * 1000 * 365 * 24 * 60 * 60 * 1000,
            _ => 0
        };
    }

    /// <summary>
    /// Represents an available unit of time
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="Symbol"></param>
    public record class TimeUnitMap(
        TimeUnitType Type,
        string Symbol);
}
