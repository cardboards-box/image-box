namespace ImageBox.Rendering.Base;

/// <summary>
/// Represents a GDI element that can be drawn with positional data
/// </summary>
public abstract class PositionalElement : RenderElement
{
    /// <summary>
    /// The x offset
    /// </summary>
    [AstAttribute("x")]
    public AstValue<SizeUnit> X { get; set; } = new();

    /// <summary>
    /// The y offset
    /// </summary>
    [AstAttribute("y")]
    public AstValue<SizeUnit> Y { get; set; } = new();

    /// <summary>
    /// The width of the rectangle
    /// </summary>
    [AstAttribute("width")]
    public AstValue<SizeUnit> Width { get; set; } = new();

    /// <summary>
    /// The height of the rectangle
    /// </summary>
    [AstAttribute("height")]
    public AstValue<SizeUnit> Height { get; set; } = new();
}