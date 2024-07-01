namespace ImageBox.Rendering.Animations;

/// <summary>
/// Represents a point in the render context
/// </summary>
[AstElement("point")]
public class PointElem : Element
{
    /// <summary>
    /// The X offset
    /// </summary>
    [AstAttribute("x")]
    public AstValue<SizeUnit?> X { get; set; } = new();

    /// <summary>
    /// The y offset
    /// </summary>
    [AstAttribute("y")]
    public AstValue<SizeUnit?> Y { get; set; } = new();
}
