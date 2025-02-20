namespace ImageBox.Elements.Shapes;

/// <summary>
/// Represents a line that can be filled or bordered
/// </summary>
[AstElement("line", ScopeType.Template)]
public class LineElem : DrawPathElement, IPathElement
{
    /// <summary>
    /// Get the path of the current element
    /// </summary>
    /// <param name="context">The size of the current context</param>
    /// <returns>The path</returns>
    public override IPath GetPath(SizeContext context)
    {
        var points = this.GetPoints(context).ToArray();
        if (points.Length < 2)
            throw new RenderContextException("A line must have at least 2 points", Context);

        var path = new PathBuilder()
            .StartFigure();

        Point previous = points[0];
        foreach(Point point in points.Skip(1))
        {
            path.AddLine(previous, point);
            previous = point;
        }

        return path
            .CloseFigure()
            .Build();
    }
}
