namespace ImageBox.Core.TimeUnits;

/// <summary>
/// Represents a unit of time.
/// </summary>
public enum TimeUnitType
{
    /// <summary>
    /// Millisecond
    /// </summary>
    Millisecond = 0,
    /// <summary>
    /// Millisecond * 1000
    /// </summary>
    Second = 1,
    /// <summary>
    /// Second * 60
    /// </summary>
    Minute = 2,
    /// <summary>
    /// Minute * 60
    /// </summary>
    Hour = 3,
    /// <summary>
    /// Hour * 24
    /// </summary>
    Day = 4,
    /// <summary>
    /// Day * 7
    /// </summary>
    Week = 5,
    /// <summary>
    /// Day * 30
    /// </summary>
    Month = 6,
    /// <summary>
    /// Month * 3
    /// </summary>
    Quarter = 7,
    /// <summary>
    /// Month * 12
    /// </summary>
    Year = 8,
    /// <summary>
    /// Year * 10
    /// </summary>
    Decade = 9,
    /// <summary>
    /// Decade * 10
    /// </summary>
    Century = 10,
    /// <summary>
    /// Century * 10
    /// </summary>
    Millennium = 11
}
