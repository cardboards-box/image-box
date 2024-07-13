namespace ImageBox.Core.SizeUnits;

/// <summary>
/// Represents a unit of measurement. (css-units)
/// </summary>
public enum SizeUnitType
{
    /// <summary>
    /// Pixel on the screen
    /// </summary>
    Pixel = 0,
    /// <summary>
    /// Pixel = Centimeter * 37.795275591
    /// </summary>
    Centimeter = 1,
    /// <summary>
    /// Centimeter / 10
    /// </summary>
    Millimeter = 2,
    /// <summary>
    /// Millimeter / 4
    /// </summary>
    QuarterMillimeter = 3,
    /// <summary>
    /// Pixel * 96
    /// </summary>
    Inch = 4,
    /// <summary>
    /// Inch / 6
    /// </summary>
    Pica = 5,
    /// <summary>
    /// Pixel / 72
    /// </summary>
    Point = 6,
    /// <summary>
    /// Percentage of the total width or height
    /// </summary>
    Percentage = 7,
    /// <summary>
    /// Pixel * FontSizeUnit
    /// </summary>
    Em = 8,
    /// <summary>
    /// Percentage of the total height
    /// </summary>
    ViewHeight = 9,
    /// <summary>
    /// Percentage of the total width
    /// </summary>
    ViewWidth = 10,
    /// <summary>
    /// Percentage of (width + height) / 2
    /// </summary>
    RelativePercentage = 11
}
