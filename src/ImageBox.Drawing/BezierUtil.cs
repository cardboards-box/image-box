namespace ImageBox.Drawing;

using Models;
using Models.Bezier;

/// <summary>
/// Utility functions for Bezier curves
/// </summary>
public static class BezierUtil
{
    /// <summary>
    /// Calculate the point on the curve at the given time
    /// </summary>
    /// <param name="t">The time</param>
    /// <param name="controlPoints">The control points of the curve</param>
    /// <returns>The current point on the curve</returns>
    public static BoxPoint BezierPoint(double t, BoxPoint[] controlPoints)
    {
        int n = controlPoints.Length;
        BoxPoint[] points = new BoxPoint[n];

        for (int i = 0; i < n; i++)
        {
            points[i] = controlPoints[i];
        }

        for (int r = 1; r < n; r++)
        {
            for (int i = 0; i < n - r; i++)
            {
                points[i].X = (1 - t) * points[i].X + t * points[i + 1].X;
                points[i].Y = (1 - t) * points[i].Y + t * points[i + 1].Y;
            }
        }

        return points[0];
    }

    /// <summary>
    /// Applies the given easing function to the time
    /// </summary>
    /// <param name="t">The point in time</param>
    /// <param name="bezier">The type of bezier curve</param>
    /// <param name="easing">The type of easing function</param>
    /// <returns>The eased point in time</returns>
    public static double ApplyEasing(double t, BezierType bezier, EasingType easing)
    {
        double QuadEaseIn(double t) => t * t;
        double QuadEaseOut(double t) => t * (2 - t);
        double QuadEaseInOut(double t) => t < 0.5 ? 2 * t * t : -1 + (4 - 2 * t) * t;
        double CubicEaseIn(double t) => t * t * t;
        double CubicEaseOut(double t) => --t * t * t + 1;
        double CubicEaseInOut(double t) => t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;

        var values = new (BezierType, EasingType, Func<double, double>)[]
        {
            (BezierType.Quadratic, EasingType.In, QuadEaseIn),
            (BezierType.Quadratic, EasingType.Out, QuadEaseOut),
            (BezierType.Quadratic, EasingType.InOut, QuadEaseInOut),
            (BezierType.Cubic, EasingType.In, CubicEaseIn),
            (BezierType.Cubic, EasingType.Out, CubicEaseOut),
            (BezierType.Cubic, EasingType.InOut, CubicEaseInOut)
        };

        foreach (var (bezierType, easingType, func) in values)
        {
            if (bezierType == bezier && easingType == easing)
                return func(t);
        }

        return t;
    }

    /// <summary>
    /// Calculate the point on the curve at the given time
    /// </summary>
    /// <param name="t">The point in time (percentage)</param>
    /// <param name="controlPoints">The various control points to use for the curve</param>
    /// <param name="bezier">The type of bezier curve</param>
    /// <param name="easing">The easing function to apply to the curve</param>
    /// <returns>The point in the bezier curve</returns>
    public static BezierPoint Calculate(double t, BoxPoint[] controlPoints, BezierType bezier, EasingType easing)
    {
        var eased = ApplyEasing(t, bezier, easing);
        var point = BezierPoint(eased, controlPoints);
        return new BezierPoint(point, t, eased, bezier, easing);
    }
}
