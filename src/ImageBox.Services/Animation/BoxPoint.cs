using SixLabors.ImageSharp;

namespace ImageBox.Services.Animation;

/// <summary>
/// Represents a point in a render context
/// </summary>
/// <param name="x">The starting point on the x axis</param>
/// <param name="y">The starting point on the y axis</param>
public class BoxPoint(double x, double y)
{
    /// <summary>
    /// The point on the x axis
    /// </summary>
    public double X { get; set; } = x;

    /// <summary>
    /// The point on the y axis
    /// </summary>
    public double Y { get; set; } = y;

    /// <summary>
    /// Convert to an integer point
    /// </summary>
    /// <param name="p">The point to convert</param>
    public static implicit operator Point(BoxPoint p) => new((int)p.X, (int)p.Y);

    /// <summary>
    /// Convert from an integer point
    /// </summary>
    /// <param name="p">The point to convert</param>
    public static implicit operator BoxPoint(Point p) => new(p.X, p.Y);

    /// <summary>
    /// Converts to a float point
    /// </summary>
    /// <param name="p">The point to convert</param>
    public static implicit operator PointF(BoxPoint p) => new((float)p.X, (float)p.Y);

    /// <summary>
    /// Converts from a float point
    /// </summary>
    /// <param name="p">The point to convert</param>
    public static implicit operator BoxPoint(PointF p) => new(p.X, p.Y);
}
