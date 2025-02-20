namespace ImageBox.Elements.Shapes;

using Drawing;

/// <summary>
/// Represents a rectangle that can be filled or bordered
/// </summary>
[AstElement("rectangle", ScopeType.Template), AstElement("rect", ScopeType.Template)]
public class RectangleElem : DrawPathElement
{
    /// <summary>
    /// The radius of the curved corners
    /// </summary>
    [AstAttribute("radius")]
    public AstValue<SizeUnit?> Radius { get; set; } = new();

    /// <summary>
    /// Get the path of the current element
    /// </summary>
    /// <param name="context">The size of the current context</param>
    /// <returns>The path</returns>
    public override IPath GetPath(SizeContext context)
    {
        var radius = (Radius.Value ?? SizeUnit.Zero).Pixels(context);
        return context.GetRectangle().Rounded(radius);
    }
}
