namespace ImageBox.Services.Loading.SystemModules;

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
}