namespace ImageBox.Rendering.Animations;

using Bezier;

/// <summary>
/// Represents a point in the render context
/// </summary>
[AstElement("point", ScopeType.CustomParent, typeof(BezierAnimationElem))]
public class PointElem : Element
{
    /// <summary>
    /// The index of the point in the point list
    /// </summary>
    [AstAttribute("i"), AstAttribute("index")]
    public AstValue<int?> Index { get; set; } = new();

    /// <summary>
    /// The X offset
    /// </summary>
    [AstAttribute("x")]
    public AstValue<SizeUnit?> X { get; set; } = new();

    /// <summary>
    /// The Y offset
    /// </summary>
    [AstAttribute("y")]
    public AstValue<SizeUnit?> Y { get; set; } = new();
}
