namespace ImageBox.Rendering.Animations;

/// <summary>
/// Represents a point in the render context
/// </summary>
[AstElement("point")]
public class PointElem : Element
{
    /// <summary>
    /// The index of the point in the point list
    /// </summary>
    [AstAttribute("index"), AstAttribute("i")]
    public AstValue<int?> Index { get; set; } = new();

    /// <summary>
    /// The X offset
    /// </summary>
    [AstAttribute("X")]
    public AstValue<SizeUnit?> X { get; set; } = new();

    /// <summary>
    /// The Y offset
    /// </summary>
    [AstAttribute("Y")]
    public AstValue<SizeUnit?> Y { get; set; } = new();
}
