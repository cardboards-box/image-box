namespace ImageBox.Core.SizeUnits;

/// <summary>
/// Represents the context for a size
/// </summary>
/// <param name="X">The X position of the context</param>
/// <param name="Y">The y position of the context</param>
/// <param name="Width">The width of the context</param>
/// <param name="Height">The height of the context</param>
/// <param name="FontSize">The font size of the context</param>
/// <param name="Parents">Any parent contexts (first will be root)</param>
public record class SizeContext(
    int X,
    int Y,
    int Width,
    int Height,
    int FontSize,
    params SizeContext[] Parents)
{
    /// <summary>
    /// The first context in the parent chain
    /// </summary>
    public SizeContext Root => Parents.Length == 0 ? this : Parents[0];

    /// <summary>
    /// Gets absolute point for the the given x and y offsets 
    /// </summary>
    /// <param name="xOffset">The offset to the point on the X axis</param>
    /// <param name="yOffset">The offset to the point on the Y axis</param>
    /// <returns>The relative X and Y points</returns>
    public (int x, int y) MakePointAbsolute(int xOffset, int yOffset)
    {
        return (X + xOffset, Y + yOffset);
    }

    /// <summary>
    /// Gets the current context with the given offsets
    /// </summary>
    /// <param name="xOffset">The X offset to start the new context at</param>
    /// <param name="yOffset">The y offset to start the new context at</param>
    /// <param name="width">The optional width of the new context</param>
    /// <param name="height">The optional height of the new context</param>
    /// <param name="fontSize">The font size for the new context</param>
    /// <returns>The new context</returns>
    /// <remarks>If <paramref name="width"/> or <paramref name="height"/> are not given, the parameter will be calculated from the given offsets</remarks>
    public SizeContext GetContext(int xOffset, int yOffset, int? width = null, int? height = null, int? fontSize = null)
    {
        var parents = Parents.Append(this).ToArray();

        var x = X + xOffset;
        var y = Y + yOffset;
        var w = width ?? Width - xOffset;
        var h = height ?? Height - yOffset;
        return new SizeContext(x, y, w, h, fontSize ?? FontSize, parents);
    }

    /// <summary>
    /// Creates the context for the first parent in the chain
    /// </summary>
    /// <param name="width">The width of the context</param>
    /// <param name="height">The height of the context</param>
    /// <param name="fontSize">The size of the font for the context</param>
    /// <returns>The root context size</returns>
    public static SizeContext ForRoot(int width, int height, int fontSize)
    {
        return new SizeContext(0, 0, width, height, fontSize);
    }
}