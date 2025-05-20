namespace ImageBox.Elements;

/// <summary>
/// Represents a GDI element that can be drawn with positional data
/// </summary>
public abstract class PositionalElement : RenderElement, IPositionElement
{
    /// <inheritdoc/>
    [AstAttribute("x")]
    public AstValue<SizeUnit?> X { get; set; } = new();

    /// <inheritdoc/>
    [AstAttribute("y")]
    public AstValue<SizeUnit?> Y { get; set; } = new();

    /// <inheritdoc/>
    [AstAttribute("width")]
    public AstValue<SizeUnit?> Width { get; set; } = new();

    /// <inheritdoc/>
    [AstAttribute("height")]
    public AstValue<SizeUnit?> Height { get; set; } = new();
}