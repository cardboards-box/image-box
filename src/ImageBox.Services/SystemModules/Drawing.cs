using Jint;
using Jint.Native;

namespace ImageBox.Services.SystemModules;

using ImageBox.Drawing.Models;

/// <summary>
/// Provides a set of functions related to image drawing and units
/// </summary>
/// <param name="_context"></param>
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = JUSTIFICATION)]
public class Drawing(
    ContextFrame _context) : IScriptItem
{
    private const string JUSTIFICATION = "Meant to be used within JavaScript modules where lowercase naming is the standard";

    /// <summary>
    /// Converts a unit to pixels
    /// </summary>
    /// <param name="value">The value of the unit</param>
    /// <param name="context">The size context provided to the unit</param>
    /// <param name="isWidth">Whether or not the unit is supposed to represent a width or x-axis value</param>
    /// <returns>The value of the unit</returns>
    public double UnitContext(string value, SizeContext? context, bool? isWidth)
    {
        context ??= _context.LastScope.Size;
        return SizeUnit.Parse(value).Pixels(context, isWidth);
    }

    /// <summary>
    /// Converts a unit to pixels
    /// </summary>
    /// <param name="value">The value of the unit</param>
    /// <param name="isWidth">Whether or not the unit is supposed to represent a width or x-axis value</param>
    /// <returns>The value of the unit</returns>
    public double unit(string value, bool? isWidth = null) => UnitContext(value, null, isWidth);

    /// <summary>
    /// Gets the value of the unit from the right side of the context
    /// </summary>
    /// <param name="value">The value of the unit</param>
    /// <returns>The value of the unit</returns>
    public double right(string value)
    {
        var ctx = _context.LastScope.Size;
        var size = UnitContext(value, ctx, true);
        return ctx.Root.Width - size;
    }

    /// <summary>
    /// Gets the value of the unit from the left side of the context
    /// </summary>
    /// <param name="value">The value of the unit</param>
    /// <returns>The value of the unit</returns>
    public double left(string value) => UnitContext(value, null, true);

    /// <summary>
    /// Gets the value of the unit from the top side of the context
    /// </summary>
    /// <param name="value">The value of the unit</param>
    /// <returns>The value of the unit</returns>
    public double top(string value) => UnitContext(value, null, false);

    /// <summary>
    /// Gets the value of the unit from the bottom side of the context
    /// </summary>
    /// <param name="value">The value of the unit</param>
    /// <returns>The value of the unit</returns>
    public double bottom(string value)
    {
        var ctx = _context.LastScope.Size;
        var size = UnitContext(value, ctx, false);
        return ctx.Root.Height - size;
    }

    /// <summary>
    /// Returns the given value as the unit value
    /// </summary>
    /// <param name="item">The item to process</param>
    /// <param name="width">Whether or not the value represents a width or x axis value (null if you don't know)</param>
    /// <returns>The number representation of the item</returns>
    /// <exception cref="InvalidOperationException">Thrown if the value isn't valid</exception>
    public double fromUnitString(JsValue item, bool? width = null)
    {
        if (item.Type == Jint.Runtime.Types.Number)
            return item.AsNumber();
        if (item.Type != Jint.Runtime.Types.String)
            throw new InvalidOperationException("Input value needs to be a number or size unit");

        return UnitContext(item.AsString(), null, width);
    }

    /// <summary>
    /// Gets the rectangle bounds from the given coordinates
    /// </summary>
    /// <param name="x1">The X value of the first coordinate</param>
    /// <param name="y1">The Y value of the first coordinate</param>
    /// <param name="x2">The X value of the second coordinate</param>
    /// <param name="y2">The Y value of the second coordinate</param>
    /// <returns>The x, y, width, and height of the bounds calculated from the coordinates</returns>
    /// <exception cref="InvalidOperationException">Thrown if any of the values aren't valid</exception>
    public object bounds(JsValue x1, JsValue y1, JsValue x2, JsValue y2)
    {
        var a = point(x1, y1);
        var b = point(x2, y2);
        return bounds(a, b);
    }

    /// <summary>
    /// Gets the rectangle bounds from the given coordinates
    /// </summary>
    /// <param name="a">The first coordinate</param>
    /// <param name="b">The second coordinate</param>
    /// <returns>The x, y, width, and height of the bounds calculated from the coordinates</returns>
    public object bounds(PointF a, PointF b)
    {
        var width = b.X - a.X;
        var height = b.Y - a.Y;
        return new { x = a.X, y = a.Y, width, height };
    }

    /// <summary>
    /// Gets the point from the given coordinates
    /// </summary>
    /// <param name="x">The point on the x axis</param>
    /// <param name="y">The point on the y axis</param>
    /// <returns>The point</returns>
    public PointF point(JsValue x, JsValue y)
    {
        var xValue = fromUnitString(x, true);
        var yValue = fromUnitString(y, false);
        return new BoxPoint(xValue, yValue);
    }

    /// <summary>
    /// Measures the bounding box of the given text
    /// </summary>
    /// <param name="text">The text to measure</param>
    /// <param name="options">
    /// <para>The text options. The following properties are supported:</para>
    /// <para>fontFamily - string</para>
    /// <para>fontSize - number / <see cref="SizeUnit"/></para>
    /// <para>fontStyle - string / <see cref="FontStyle"/> enum</para>
    /// <para>width - number / <see cref="SizeUnit"/></para>
    /// <para>lineSpacing - number / <see cref="SizeUnit"/></para>
    /// <para>wordBreaking - string / <see cref="WordBreaking"/> enum</para>
    /// <para>textDirection - string / <see cref="TextDirection"/> enum</para>
    /// <para>textAlignment - string / <see cref="TextAlignment"/> enum</para>
    /// <para>horizontalAlignment - string / <see cref="HorizontalAlignment"/> enum</para>
    /// <para>verticalAlignment - string / <see cref="VerticalAlignment"/> enum</para>
    /// <para>layoutMode - string / <see cref="LayoutMode"/> enum</para>
    /// <para>textJustification - string / <see cref="TextJustification"/> enum</para>
    /// </param>
    /// <returns>The x, y, width and height of the bounds that were measured</returns>
    /// <exception cref="RenderContextException">Thrown if the <paramref name="options"/> are invalid</exception>
    public object measureText(string text, JsValue? options = null)
    {
        Font GetFont()
        {
            string? family;
            if (options is null)
            {
                family = _context.LastScope.Size.FontFamily;
                return _context.BoxContext.Fonts.GetFont(family, _context.LastScope, FontStyle.Regular);
            }

            var obj = options.AsObject();

            var font = obj.AsInstance<Font>();
            if (font is not null) return font;

            var size = DetermineFloat("fontSize", _context.LastScope.Size.FontSize, true);
            var style = DetermineEnum("fontStyle", FontStyle.Regular);

            family = obj.Get("fontFamily").AsString();
            if (string.IsNullOrEmpty(family))
                family = _context.LastScope.Size.FontFamily;

            if (!_context.BoxContext.Fonts.Families.TryGetValue(family, out var fontFamily))
                throw new RenderContextException($"Font family '{family}' not found in context", _context.LastScope.AstElement);

            return fontFamily.Get((int)size, style);
        }

        float DetermineFloat(string name, float defaultValue, bool isWidth = true)
        {
            if (options is null) return defaultValue;
            var value = options.AsObject().Get(name);
            if (value.IsUndefined() || value.IsNull())
                return defaultValue;
            return (float)fromUnitString(value, isWidth);
        }

        T DetermineEnum<T>(string name, T defaultValue) where T : struct, Enum
        {
            if (options is null) return defaultValue;
            var value = options.AsObject().Get(name);
            if (value.IsUndefined() || value.IsNull()) return defaultValue;
            if (!Enum.TryParse<T>(value.AsString(), true, out var result))
                throw new RenderContextException($"Invalid enum value '{value}' for {name}", _context.LastScope.AstElement);
            return result;
        }

        var textOptions = new TextOptions(GetFont())
        {
            WrappingLength = DetermineFloat("width", _context.LastScope.Size.Width),
            TabWidth = DetermineFloat("tabWidth", -1F, true),
            LineSpacing = DetermineFloat("lineSpacing", 1F, false),
            WordBreaking = DetermineEnum("wordBreaking", WordBreaking.Standard),
            TextDirection = DetermineEnum("textDirection", TextDirection.Auto),
            TextAlignment = DetermineEnum("textAlignment", TextAlignment.Center),
            HorizontalAlignment = DetermineEnum("horizontalAlignment", HorizontalAlignment.Center),
            VerticalAlignment = DetermineEnum("verticalAlignment", VerticalAlignment.Center),
            LayoutMode = DetermineEnum("layoutMode", LayoutMode.HorizontalTopBottom),
            TextJustification = DetermineEnum("textJustification", TextJustification.None),
        };
        var rect = TextMeasurer.MeasureAdvance(text, textOptions);
        return new
        {
            x = rect.X,
            y = rect.Y,
            width = rect.Width,
            height = rect.Height,
        };
    }
}