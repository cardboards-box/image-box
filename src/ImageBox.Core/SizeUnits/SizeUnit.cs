namespace ImageBox.Core.SizeUnits;

/// <summary>
/// Represents a unit of measurement of something on an image
/// </summary>
/// <param name="Type">The type of unit of measurement</param>
/// <param name="Value">The value of the measurement</param>
[JsonConverter(typeof(SizeUnitSerializer))]
public record struct SizeUnit(
    SizeUnitType Type,
    double Value)
{
    /// <summary>
    /// A size of zero
    /// </summary>
    public static SizeUnit Zero { get; } = new(SizeUnitType.Pixel, 0);

    /// <summary>
    /// 100% of the parent size
    /// </summary>
    public static SizeUnit Fill { get; } = new(SizeUnitType.Percentage, 100);

    /// <summary>
    /// 100% of the parent width
    /// </summary>
    public static SizeUnit FillWidth { get; } = new(SizeUnitType.ViewWidth, 100);

    /// <summary>
    /// 100% of the parent height
    /// </summary>
    public static SizeUnit FillHeight { get; } = new(SizeUnitType.ViewHeight, 100);

    /// <summary>
    /// Gets the equivalent number of pixels for this unit of measurement
    /// </summary>
    /// <param name="context">The context of the parent</param>
    /// <param name="isWidth">Whether or not the unit is for widths/x axis or heights/y axis (or null if the context isn't known)</param>
    /// <returns>The number of pixels</returns>
    public readonly int Pixels(SizeContext? context = null, bool? isWidth = null) => SizeUnitHelper.GetPixels(this, context, isWidth);

    /// <summary>
    /// Converts the unit of measurement to a string
    /// </summary>
    /// <returns>The string version of the measurement</returns>
    public readonly string Serialize() => SizeUnitHelper.SerializeUnit(this);

    /// <summary>
    /// Converts the unit of measurement to a string
    /// </summary>
    /// <returns>The serialized unit of measurement</returns>
    public override readonly string ToString() => Serialize();

    /// <summary>
    /// Converts the string to a unit of measurement
    /// </summary>
    /// <param name="input">The string version of the measurement</param>
    /// <returns>The parsed size unit</returns>
    public static SizeUnit Parse(string input) => SizeUnitHelper.ParseUnit(input);

    /// <summary>
    /// Converts the unit of measurement to a string
    /// </summary>
    /// <param name="size">The unit of measurement</param>
    public static implicit operator string(SizeUnit size) => size.Serialize();

    /// <summary>
    /// Converts the string to a unit of measurement
    /// </summary>
    /// <param name="input">The string version of the measurement</param>
    public static implicit operator SizeUnit(string input) => Parse(input);
}