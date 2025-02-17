using IFontStyle = SixLabors.Fonts.FontStyle;

namespace ImageBox.Rendering.Base;

/// <summary>
/// Represents a GDI element that can be drawn with positional data
/// </summary>
public abstract class PositionalElement : RenderElement, IPositionElement
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

    /// <summary>
    /// The width of the rectangle
    /// </summary>
    [AstAttribute("width")]
    public AstValue<SizeUnit?> Width { get; set; } = new();

    /// <summary>
    /// The height of the rectangle
    /// </summary>
    [AstAttribute("height")]
    public AstValue<SizeUnit?> Height { get; set; } = new();

    /// <summary>
    /// The font size
    /// </summary>
    [AstAttribute("font-size")]
    public AstValue<SizeUnit?> FontSize { get; set; } = new();

    /// <summary>
    /// The font family to use for the text
    /// </summary>
    [AstAttribute("font-family")]
    public AstValue<string?> FontFamily { get; set; } = new();

    /// <summary>
    /// The style of the font to use
    /// </summary>
    [AstAttribute("font-style", typeof(IFontStyle))]
    public AstValue<string?> FontStyle { get; set; } = new();

    /// <summary>
    /// Whether to automatically determine the font size based on the box size
    /// </summary>
    /// <remarks>
    /// If true, <see cref="FontSize"/> will be ignored. 
    /// This can be relatively expensive, so it should be avoided when possible.
    /// </remarks>
    [AstAttribute("auto-font-size")]
    public AstValue<bool?> AutoFontSize { get; set; } = new();

    /// <summary>
    /// Sets the padding to use when determining the font size
    /// </summary>
    /// <remarks>Only used when <see cref="AutoFontSize"/> is true</remarks>
    [AstAttribute("auto-font-size-padding")]
    public AstValue<int?> AutoFontSizePadding { get; set; } = new();
}