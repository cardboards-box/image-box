using Jint;
using Jint.Native;
using SixLabors.ImageSharp;

namespace ImageBox.Services.SystemModules;

using ImageBox.Drawing.Models;

/// <summary>
/// Provides a set of functions related to image drawing and units
/// </summary>
/// <param name="_context"></param>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = JUSTIFICATION)]
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
}