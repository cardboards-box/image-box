namespace ImageBox.Rendering.Base;

/// <summary>
/// Represents a GDI element that can be drawn with positional data
/// </summary>
public interface IPositionElement : IElement
{
    /// <summary>
    /// The X offset
    /// </summary>
    AstValue<SizeUnit?> X { get; }

    /// <summary>
    /// The Y offset
    /// </summary>
    AstValue<SizeUnit?> Y { get; }

    /// <summary>
    /// The width of the rectangle
    /// </summary>
    AstValue<SizeUnit?> Width { get; }

    /// <summary>
    /// The height of the rectangle
    /// </summary>
    AstValue<SizeUnit?> Height { get; }

    /// <summary>
    /// The font size
    /// </summary>
    AstValue<SizeUnit?> FontSize { get; }

    /// <summary>
    /// The font family to use for the text
    /// </summary>
    AstValue<string?> FontFamily { get; }

    /// <summary>
    /// The style of the font to use
    /// </summary>
    AstValue<string?> FontStyle { get; }
}
