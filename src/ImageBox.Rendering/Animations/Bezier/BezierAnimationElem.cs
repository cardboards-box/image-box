namespace ImageBox.Rendering.Animations.Bezier;

/// <summary>
/// Renders the children of the element with a Bezier animation
/// </summary>
[AstElement("animation-bezier")]
public class BezierAnimationElem : PositionalElement, IParentElement
{
    /// <summary>
    /// The type of Bezier curve to use
    /// </summary>
    [AstAttribute("type"), AstAttribute("interpolation")]
    public AstValue<string?> Interpolation { get; set; } = new();

    /// <summary>
    /// The easing function to use
    /// </summary>
    [AstAttribute("easing"), AstAttribute("timing")]
    public AstValue<string?> Timing { get; set; } = new();

    /// <summary>
    /// The points of the curve to animate between
    /// </summary>
    public PointElem[] Points => Children.OfType<PointElem>().ToArray();

    /// <summary>
    /// All of the child elements on the parent element
    /// </summary>
    public IElement[] Children { get; set; } = [];

    internal static Point BezierPoint(double t, Point[] controlPoints)
    {
        int n = controlPoints.Length - 1;
        Point[] points = new Point[n + 1];

        for (int i = 0; i <= n; i++)
        {
            points[i] = controlPoints[i];
        }

        for (int r = 1; r <= n; r++)
        {
            for (int i = 0; i <= n - r; i++)
            {
                points[i].X = (1 - t) * points[i].X + t * points[i + 1].X;
                points[i].Y = (1 - t) * points[i].Y + t * points[i + 1].Y;
            }
        }

        return points[0];
    }

    internal Point[] GetControlPoints(SizeContext size)
    {
        var points = new (double x, double y, int i)[Points.Length];
        for(var i = 0; i < Points.Length; i++)
        {
            var index = Points[i].Index?.Value ?? i;
            var x = (double)(Points[i].X?.Value?.Pixels(size, true) ?? 0);
            var y = (double)(Points[i].Y?.Value?.Pixels(size, false) ?? 0);
            points[i] = (x, y, index);
        }

        return points.OrderBy(p => p.i).Select(p => new Point(p.x, p.y)).ToArray();
    }

    internal double Easing(double t, BezierType bezier, EasingType easing)
    {
        double QuadEaseIn(double t) => t * t;
        double QuadEaseOut(double t) => t * (2 - t);
        double QuadEaseInOut(double t) => t < 0.5 ? 2 * t * t : -1 + (4 - 2 * t) * t;
        double CubicEaseIn(double t) => t * t * t;
        double CubicEaseOut(double t) => (--t) * t * t + 1;
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

        foreach(var (bezierType, easingType, func) in values)
        {
            if (bezierType == bezier && easingType == easing)
                return func(t);
        }

        return t;
    }

    /// <summary>
    /// Renders the element to the render context
    /// </summary>
    /// <param name="context">The context to render</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override async Task Render(ContextFrame context)
    {
        using var fullScope = Scoped(context);

        if (Points.Length < 2) return;

        var controlPoints = GetControlPoints(fullScope.Size);

        if (!Enum.TryParse<BezierType>(Interpolation.Value, true, out var bezierType))
            bezierType = BezierType.Linear;

        if (!Enum.TryParse<EasingType>(Timing.Value, true, out var easingType))
            easingType = EasingType.InOut;

        double t = context.Frame / (double)context.TotalFrames;
        double eased = Easing(t, bezierType, easingType);
        var point = BezierPoint(eased, controlPoints);
        var newSize = fullScope.Size.GetContext((int)point.X, (int)point.Y);
        var vars = new Dictionary<string, object?>
        {
            ["x"] = point.X,
            ["y"] = point.Y,
            ["width"] = newSize.Width,
            ["height"] = newSize.Height,
            ["t"] = t,
            ["eased"] = eased,
        };
        using var scope = context.Scope(this, newSize, vars);

        foreach (var child in Children)
        {
            if (child is not RenderElement render) continue;

            await render.Render(context);
        }
    }

    /// <summary>
    /// The type of Bezier curve to use
    /// </summary>
    public enum BezierType
    {
        /// <summary>
        /// Linear interpolation
        /// </summary>
        Linear,
        /// <summary>
        /// Quadratic interpolation
        /// </summary>
        Quadratic,
        /// <summary>
        /// Cubic interpolation
        /// </summary>
        Cubic,
    }

    /// <summary>
    /// The type of easing function to use
    /// </summary>
    public enum EasingType
    {
        /// <summary>
        /// Ease-in
        /// </summary>
        In,
        /// <summary>
        /// Ease-out
        /// </summary>
        Out,
        /// <summary>
        /// Ease-in-out
        /// </summary>
        InOut,
    }

    internal class Point(double x, double y)
    {
        public double X { get; set; } = x;
        public double Y { get; set; } = y;
    }
}