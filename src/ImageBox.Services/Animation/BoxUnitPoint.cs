namespace ImageBox.Services.Animation;

/// <summary>
/// Represents a point in a render context that uses computed units
/// </summary>
/// <param name="X">The point on the x axis</param>
/// <param name="Y">The point on the y axis</param>
public record class BoxUnitPoint(
    SizeUnit X,
    SizeUnit Y)
{
    /// <summary>
    /// Converts the point to pixels
    /// </summary>
    /// <param name="context">The context to render in</param>
    /// <returns>The point in pixels</returns>
    public BoxPoint Pixels(SizeContext? context = null)
    {
        return new BoxPoint(
            X.Pixels(context, true),
            Y.Pixels(context, false));
    }
}
