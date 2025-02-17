namespace ImageBox.Rendering.Animations.Bezier;

using Services.Animation;
using Services.Animation.Bezier;

/// <summary>
/// Renders the children of the element with a Bezier animation
/// </summary>
[AstElement("animation-bezier", ScopeType.Template)]
public class BezierAnimationElem : PositionalElement, IParentElement
{
    /// <summary>
    /// The type of Bezier curve to use
    /// </summary>
    [AstAttribute("type", typeof(BezierType)), AstAttribute("interpolation", typeof(BezierType))]
    public AstValue<string?> Interpolation { get; set; } = new();

    /// <summary>
    /// The easing function to use
    /// </summary>
    [AstAttribute("easing", typeof(EasingType)), AstAttribute("timing", typeof(EasingType))]
    public AstValue<string?> Timing { get; set; } = new();

    /// <summary>
    /// The points of the curve to animate between
    /// </summary>
    public PointElem[] Points => Children.OfType<PointElem>().ToArray();

    /// <summary>
    /// All of the child elements on the parent element
    /// </summary>
    public IElement[] Children { get; set; } = [];

    internal BoxUnitPoint[] GetControlPoints()
    {
        var points = new (SizeUnit x, SizeUnit y, int i)[Points.Length];
        for(var i = 0; i < Points.Length; i++)
        {
            var index = Points[i].Index?.Value ?? i;
            var x = Points[i].X?.Value ?? SizeUnit.Zero;
            var y = Points[i].Y?.Value ?? SizeUnit.Zero;
            points[i] = (x, y, index);
        }

        return points.OrderBy(p => p.i).Select(p => new BoxUnitPoint(p.x, p.y)).ToArray();
    }

    /// <summary>
    /// Renders the element to the render context
    /// </summary>
    /// <param name="context">The context to render</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override async Task Render(ContextFrame context)
    {
        using var fullScope = this.Scoped(context);

        if (Points.Length < 2) return;

        if (!Enum.TryParse<BezierType>(Interpolation.Value, true, out var bezierType))
            bezierType = BezierType.Linear;

        if (!Enum.TryParse<EasingType>(Timing.Value, true, out var easingType))
            easingType = EasingType.InOut;

        var controlPoints = GetControlPoints();
        var point = BezierUtil.Calculate(fullScope, controlPoints, bezierType, easingType);
        var newSize = point.Contextualize(fullScope.Size);
        var vars = new Dictionary<string, object?>
        {
            ["width"] = newSize.Width,
            ["height"] = newSize.Height,
        };
        point.ApplyToScope(vars);
        using var scope = context.Scope(this, newSize, vars);

        foreach (var child in Children)
        {
            if (child is not RenderElement render) continue;

            await render.Render(context);
        }
    }
}