using ImageBox.Core.SizeUnits;

namespace ImageBox.Elements.Elements.GDIs;

/// <summary>
/// Represents text to be drawn to the card
/// </summary>
[AstElement("text")]
public class TextElem : PositionalGDIElement
{
    /// <summary>
    /// The value of the text to draw to the card
    /// </summary>
    [AstAttribute("value")]
    public AstValue<string> Value { get; set; } = new();

    /// <summary>
    /// The font size
    /// </summary>
    [AstAttribute("font-size")]
    public AstValue<SizeUnit> FontSize { get; set; } = new();

    /// <summary>
    /// The font family to use for the text
    /// </summary>
    [AstAttribute("font-family")]
    public AstValue<string> FontFamily { get; set; } = new();

    /// <summary>
    /// The color to fill with
    /// </summary>
    [AstAttribute("color")]
    public AstValue<string> Color { get; set; } = new();

    /// <summary>
    /// Where to align the text vertically in the rectangle
    /// </summary>
    [AstAttribute("align-vertical")]
    public AstValue<string> AlignVertical { get; set; } = new();

    /// <summary>
    /// Where to align the text horizontally in the rectangle
    /// </summary>
    [AstAttribute("align-horizontal")]
    public AstValue<string> AlignHorizontal { get; set; } = new();
}