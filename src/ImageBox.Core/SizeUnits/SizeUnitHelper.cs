namespace ImageBox.Core.SizeUnits;

/// <summary>
/// Helper utility for size unit operations
/// </summary>
public static class SizeUnitHelper
{
    private static SizeUnitMap[]? _units;
    private const string ERROR_PERCENT = "Cannot use percentage measurement as the context of the size is not known";

    /// <summary>
    /// All of the available units of measurement
    /// </summary>
    /// <returns></returns>
    public static SizeUnitMap[] Units()
    {
        return _units ??=
        [
            new SizeUnitMap(SizeUnitType.Centimeter, "cm"),
            new SizeUnitMap(SizeUnitType.Millimeter, "mm"),
            new SizeUnitMap(SizeUnitType.QuarterMillimeter, "q"),
            new SizeUnitMap(SizeUnitType.Inch, "in"),
            new SizeUnitMap(SizeUnitType.Pica, "pc"),
            new SizeUnitMap(SizeUnitType.Point, "pt"),
            new SizeUnitMap(SizeUnitType.Pixel, "px"),
            new SizeUnitMap(SizeUnitType.Percentage, "%"),
            new SizeUnitMap(SizeUnitType.Em, "em"),
            new SizeUnitMap(SizeUnitType.ViewHeight, "vh"),
            new SizeUnitMap(SizeUnitType.ViewWidth, "vw"),
            new SizeUnitMap(SizeUnitType.RelativePercentage, "rp")
        ];
    }

    /// <summary>
    /// Converts the given string to a <see cref="SizeUnit"/>
    /// </summary>
    /// <param name="input">The unit of measurement</param>
    /// <returns>The <see cref="SizeUnit"/> representing the unit of measurement</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="input"/> is null or empty</exception>
    /// <exception cref="UnitParserException">Thrown if the <paramref name="input"/> is not a valid unit of measurement</exception>
    public static SizeUnit ParseUnit(string input)
    {
        if (string.IsNullOrEmpty(input)) throw new ArgumentNullException(nameof(input));

        input = input.Trim().ToLower();

        if (input == "0") return SizeUnit.Zero;

        input = RegexUtility.UnitFilter(input);
        if (string.IsNullOrEmpty(input)) return SizeUnit.Zero;

        var (value, unit) = RegexUtility.UnitParse(input);
        if (string.IsNullOrEmpty(unit)) return new SizeUnit(SizeUnitType.Pixel, value);

        var unitMatch = Units().FirstOrDefault(u => u.Symbol == unit);
        if (unitMatch is null) return new SizeUnit(SizeUnitType.Pixel, value);

        return new SizeUnit(unitMatch.Type, value);
    }

    /// <summary>
    /// Serializes the given <see cref="SizeUnit"/> to a string
    /// </summary>
    /// <param name="size">The unit of measurement</param>
    /// <returns>The string representation of the size</returns>
    public static string SerializeUnit(SizeUnit size)
    {
        var unit = Units().FirstOrDefault(u => u.Type == size.Type);
        if (unit is null) return $"{size.Value}px";

        return $"{size.Value}{unit.Symbol}";
    }

    /// <summary>
    /// Converts the <see cref="SizeUnit"/> into the pixel equivalent
    /// </summary>
    /// <param name="size">The unit to convert</param>
    /// <param name="context">The context of the size request</param>
    /// <param name="isWidth">Whether or not the unit is for widths or heights (or null if the context isn't known)</param>
    /// <returns>The pixel equivalent of the given size</returns>
    /// <exception cref="NullReferenceException">Thrown if the context is missing a required value</exception>
    /// <exception cref="ArgumentException">Thrown if the size is a percentage and the context is not known</exception>
    public static int GetPixels(SizeUnit size, SizeContext? context, bool? isWidth)
    {
        if (size.Value == 0) return 0;

        SizeContext NotNullContext() => context ?? throw new NullReferenceException(ERROR_PERCENT);

        var value = size.Type switch
        {
            SizeUnitType.Pixel => size.Value,
            SizeUnitType.Centimeter => CentimeterToPixel(size.Value),
            SizeUnitType.Millimeter => MillimeterToPixel(size.Value),
            SizeUnitType.QuarterMillimeter => QuarterMillimeterToPixel(size.Value),
            SizeUnitType.Inch => InchToPixel(size.Value),
            SizeUnitType.Pica => PicaToPixel(size.Value),
            SizeUnitType.Point => PointToPixel(size.Value),
            SizeUnitType.RelativePercentage => RelativePercentageToPixel(size.Value, NotNullContext().Width, NotNullContext().Height),
            SizeUnitType.Em => EmToPixel(size.Value, NotNullContext().FontSize),
            SizeUnitType.ViewHeight => PercentToPixel(size.Value, null, NotNullContext().Root.Height, false),
            SizeUnitType.ViewWidth => PercentToPixel(size.Value, NotNullContext().Root.Width, null, true),
            SizeUnitType.Percentage => PercentToPixel(size.Value, NotNullContext().Width, NotNullContext().Height, isWidth),
            _ => 0,
        };

        return (int)Math.Round(value);
    }

    /// <summary>
    /// Converts the given centimeters to pixels
    /// </summary>
    /// <param name="value">The number of centimeters</param>
    /// <returns>The equivalent number of pixels</returns>
    public static double CentimeterToPixel(double value) => value * 37.795275590551181102362204724409;

    /// <summary>
    /// Converts the given millimeters to pixels
    /// </summary>
    /// <param name="value">The number of millimeters</param>
    /// <returns>The equivalent number of pixels</returns>
    public static double MillimeterToPixel(double value) => CentimeterToPixel(value) / 10;

    /// <summary>
    /// Converts the given quarter millimeters to pixels
    /// </summary>
    /// <param name="value">The number of quarter millimeters</param>
    /// <returns>The equivalent number of pixels</returns>
    public static double QuarterMillimeterToPixel(double value) => MillimeterToPixel(value) / 4;

    /// <summary>
    /// Converts the given inches to pixels
    /// </summary>
    /// <param name="value">The number of inches</param>
    /// <returns>The equivalent number of pixels</returns>
    public static double InchToPixel(double value) => value * 96;

    /// <summary>
    /// Converts the given picas to pixels
    /// </summary>
    /// <param name="value">The number of picas</param>
    /// <returns>The equivalent number of pixels</returns>
    public static double PicaToPixel(double value) => InchToPixel(value) / 6;

    /// <summary>
    /// Converts the given points to pixels
    /// </summary>
    /// <param name="value">The number of points</param>
    /// <returns>The equivalent number of pixels</returns>
    public static double PointToPixel(double value) => value / (72.0 / 96.0);

    /// <summary>
    /// Converts the given percentage to pixels
    /// </summary>
    /// <param name="value">The percentage of the size</param>
    /// <param name="width">The width of the parent or image</param>
    /// <param name="height">The height of the parent or image</param>
    /// <param name="isWidth">Whether or not the size represents the width or height</param>
    /// <returns>The equivalent number of pixels</returns>
    /// <exception cref="ArgumentException">Thrown if the context of the size is not given correctly</exception>
    public static double PercentToPixel(double value, int? width, int? height, bool? isWidth)
    {
        if (isWidth is null) throw new ArgumentException(ERROR_PERCENT);

        if (isWidth.Value && width is null ||
            !isWidth.Value && height is null) throw new ArgumentException(ERROR_PERCENT);

        return value / 100 * (isWidth.Value ? width!.Value : height!.Value);
    }

    /// <summary>
    /// Converts the given relative percentage to pixels
    /// </summary>
    /// <param name="value">The percentage of the size</param>
    /// <param name="width">The width of the parent or image</param>
    /// <param name="height">The height of the parent or image</param>
    /// <returns>The equivalent number of pixels</returns>
    /// <exception cref="ArgumentException">Thrown if the context of the size is not given correctly</exception>
    public static double RelativePercentageToPixel(double value, int? width, int? height)
    {
        if (width is null || height is null) throw new ArgumentException(ERROR_PERCENT);
        return value / 100 * (width.Value + height.Value) / 2;
    }

    /// <summary>
    /// Converts the given em value to pixels relative to the context's font size
    /// </summary>
    /// <param name="value">The number of ems</param>
    /// <param name="fontSize">The size of the font in the current context</param>
    /// <returns>The equivalent number of pixels</returns>
    public static double EmToPixel(double value, int? fontSize)
    {
        if (fontSize is null) throw new ArgumentException("Cannot use em measurement without a font size");

        return value * fontSize.Value;
    }

    /// <summary>
    /// Represents an available unit of measurement
    /// </summary>
    /// <param name="Type">The type of unit</param>
    /// <param name="Symbol">The symbol that indicates the unit</param>
    public record class SizeUnitMap(
        SizeUnitType Type,
        string Symbol);
}
