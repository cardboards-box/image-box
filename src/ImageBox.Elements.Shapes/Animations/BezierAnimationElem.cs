namespace ImageBox.Elements.Shapes.Animations;

using Drawing;
using Drawing.Models.Bezier;

/// <summary>
/// Renders the children of the element with a Bezier animation
/// </summary>
[AstElement("animation-bezier", ScopeType.Template)]
public class BezierAnimationElem : PositionalElement, IPathElement
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

    /// <summary>
    /// Calculate the current point in the bezier curve based on the current frame
    /// </summary>
    /// <param name="scope">The context's scope</param>
    /// <returns>The point in the bezier curve</returns>
    internal BezierPoint Calculate(ContextScope scope)
    {
        if (!Enum.TryParse<BezierType>(Interpolation.Value, true, out var bezierType))
            bezierType = BezierType.Linear;

        if (!Enum.TryParse<EasingType>(Timing.Value, true, out var easingType))
            easingType = EasingType.InOut;

        var controlPoints = this.GetPoints(scope.Size).ToArray();
        double t = scope.Frame.Frame / (double)scope.Frame.TotalFrames;
        return BezierUtil.Calculate(t, controlPoints, bezierType, easingType);
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

        var point = Calculate(fullScope);
        var newSize = point.Contextualize(fullScope.Size);
        var vars = new Dictionary<string, object?>
        {
            ["width"] = newSize.Width,
            ["height"] = newSize.Height,
        };
        point.ApplyToScope(vars);

        await this.RenderChildren(context, newSize, vars);
    }
}