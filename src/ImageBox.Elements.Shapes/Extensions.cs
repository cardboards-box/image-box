namespace ImageBox.Elements.Shapes;

using Drawing.Models;

/// <summary>
/// Extension methods for the shapes namespace
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Gets all of the points from the <see cref="PointElem"/> children
    /// </summary>
    /// <param name="element">The element to get the points from</param>
    /// <returns>The points in the path</returns>
    public static IEnumerable<BoxUnitPoint> GetPoints(this IPathElement element)
    {
        var points = element.Children.OfType<PointElem>();
        foreach(var point in points)
        {
            var x = point.X?.Value ?? SizeUnit.Zero;
            var y = point.Y?.Value ?? SizeUnit.Zero;
            yield return new BoxUnitPoint(x, y);
        }
    }

    /// <summary>
    /// Gets all of the points from the <see cref="PointElem"/> children
    /// </summary>
    /// <param name="element">The element to get the points from</param>
    /// <param name="size">The current scope's size context</param>
    /// <returns>The points in the path</returns>
    public static IEnumerable<BoxPoint> GetPoints(this IPathElement element, SizeContext size)
    {
        return element.GetPoints().Select(p => p.Pixels(size));
    }
}
