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
    /// <param name="origin">Sets the origin for the circle</param>
    /// <returns>The path</returns>
    public override IPath GetPath(SizeContext context, Vector2? origin = null)
    {
        var rect = context.GetRectangle();
        var center = origin ?? new Vector2(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
        return new EllipsePolygon(center.X, center.Y, rect.Width, rect.Height);
    }
}
