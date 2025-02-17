using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using System.Numerics;

namespace ImageBox.Drawing;

/// <summary>
/// Rounded Rectangle stuff
/// </summary>
public static class RoundedRectangle
{
    /// <summary>
    /// Gets the rounded rectangle path
    /// </summary>
    /// <param name="rectangle">The original rectangle</param>
    /// <param name="cornerRadius">The corner radius</param>
    /// <returns>The path</returns>
    public static IPath Rounded(this Rectangle rectangle, float cornerRadius)
    {
        var pathBuilder = new PathBuilder();
        float width = rectangle.Width;
        float height = rectangle.Height;

        width--;
        height--;

        var radius = 2 * cornerRadius;

        // Make sure the rounded corners are no larger than half the size of the rectangle
        cornerRadius = Math.Min(width * 0.5f, Math.Min(height * 0.5f, cornerRadius));

        // Start drawing path
        pathBuilder.StartFigure();

        // upperBorder
        pathBuilder.AddLine(cornerRadius, 0, width - cornerRadius, 0);

        // Upper right rounded corner
        pathBuilder.AddArc(new RectangleF(width - radius, 0, radius, radius), 0, 270, 90);

        // right line
        pathBuilder.AddLine(width, cornerRadius, width, height - cornerRadius);

        // Lower right rounded corner
        pathBuilder.AddArc(new RectangleF(width - radius, height - radius, radius, radius), 0, 0, 90);

        // lower border
        pathBuilder.AddLine(width - cornerRadius, height, cornerRadius, height);

        // Lower left rounded corner
        pathBuilder.AddArc(new RectangleF(0, height - radius, radius, radius), 0, 90, 90);

        // left line
        pathBuilder.AddLine(0, height - cornerRadius, 0, cornerRadius);

        // Upper left rounded corner
        pathBuilder.AddArc(new RectangleF(0, 0, radius, radius), 0, 180, 90);

        // Close the path to form a complete rectangle
        pathBuilder.CloseFigure();

        return pathBuilder.Build().Transform(Matrix3x2.CreateTranslation(rectangle.X, rectangle.Y));
    }
}
