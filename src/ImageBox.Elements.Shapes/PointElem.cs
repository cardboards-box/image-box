namespace ImageBox.Elements.Shapes;

/// <summary>
/// Represents a point in the render context
/// </summary>
[AstElement("point", ScopeType.CustomParent)]
public class PointElem : Element
{
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
