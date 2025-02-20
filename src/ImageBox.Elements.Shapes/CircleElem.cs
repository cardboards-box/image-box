namespace ImageBox.Elements.Shapes;

using Drawing;

/// <summary>
/// Represents a circle that can be filled or bordered
/// </summary>
[AstElement("circle", ScopeType.Template)]
public class CircleElem : DrawPathElement
{
    /// <summary>
    /// Get the path of the current element
    /// </summary>
    /// <param name="context">The size of the current context</param>
    /// <returns>The path</returns>
    public override IPath GetPath(SizeContext context)
    {
        var rect = context.GetRectangle();
        return new EllipsePolygon(rect.X, rect.Y, rect.Width, rect.Height)
            .Translate(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
    }
}
