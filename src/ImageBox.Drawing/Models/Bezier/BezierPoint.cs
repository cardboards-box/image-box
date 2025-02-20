namespace ImageBox.Drawing.Models.Bezier;

/// <summary>
/// The output of a Bezier curve calculation
/// </summary>
/// <param name="Point">The output point based on the current time</param>
/// <param name="PointInTime">The point in time</param>
/// <param name="EasedPointInTime">The point in time with the easing applied</param>
/// <param name="Type">The type of the bezier curve applied</param>
/// <param name="Easing">The type of easing function applied</param>
public record class BezierPoint(
    BoxPoint Point,
    double PointInTime,
    double EasedPointInTime,
    BezierType Type,
    EasingType Easing)
{
    /// <summary>
    /// The point on the x axis
    /// </summary>
    public double X => Point.X;

    /// <summary>
    /// The point on the y axis
    /// </summary>
    public double Y => Point.Y;

    /// <summary>
    /// Converts the Bezier point to a JavaScript object
    /// </summary>
    /// <returns></returns>
    public object ToJsObj()
    {
        return new
        {
            x = X,
            y = Y,
            t = PointInTime,
            eased = EasedPointInTime
        };
    }

    /// <summary>
    /// Converts the Bezier point to a point in the current context
    /// </summary>
    /// <param name="context">The context of the point</param>
    /// <returns>The new size context</returns>
    public SizeContext Contextualize(SizeContext context)
    {
        return context.GetContext((int)X, (int)Y);
    }

    /// <summary>
    /// Applies the current point to the given scope
    /// </summary>
    /// <param name="scope">The scope to apply to</param>
    public void ApplyToScope(Dictionary<string, object?> scope)
    {
        scope["x"] = X;
        scope["y"] = Y;
        scope["t"] = PointInTime;
        scope["eased"] = EasedPointInTime;
    }
}
