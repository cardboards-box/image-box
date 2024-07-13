namespace ImageBox.Core.TimeUnits;

/// <summary>
/// Represents a unit of time.
/// </summary>
/// <param name="Type">The type of unit of time</param>
/// <param name="Value">The value of the time unit</param>
[JsonConverter(typeof(TimeUnitSerializer))]
public record struct TimeUnit(
    TimeUnitType Type,
    double Value)
{
    /// <summary>
    /// A time of zero
    /// </summary>
    public static TimeUnit Zero { get; } = new(TimeUnitType.Millisecond, 0);

    /// <summary>
    /// The number of milliseconds in this time unit
    /// </summary>
    public readonly double Milliseconds => TimeUnitHelper.GetMilliseconds(this);

    /// <summary>
    /// Converts the unit of measurement to a string
    /// </summary>
    /// <returns>The string version of the measurement</returns>
    public readonly string Serialize() => TimeUnitHelper.SerializeUnit(this);

    /// <summary>
    /// Converts the unit of measurement to a string
    /// </summary>
    /// <returns>The serialized unit of measurement</returns>
    public override readonly string ToString() => Serialize();

    /// <summary>
    /// Converts the string to a unit of measurement
    /// </summary>
    /// <param name="input">The string version of the measurement</param>
    /// <returns>The parsed time unit</returns>
    public static TimeUnit Parse(string input) => TimeUnitHelper.ParseUnit(input);

    /// <summary>
    /// Converts the unit of measurement to a string
    /// </summary>
    /// <param name="size">The unit of measurement</param>
    public static implicit operator string(TimeUnit size) => size.Serialize();

    /// <summary>
    /// Converts the string to a unit of measurement
    /// </summary>
    /// <param name="input">The string version of the measurement</param>
    public static implicit operator TimeUnit(string input) => Parse(input);
}
