using IFontStyle = SixLabors.Fonts.FontStyle;

namespace ImageBox.Elements;

/// <summary>
/// Represents a GDI element that has text and font properties
/// </summary>
public abstract class FontElement : PositionalElement, IFontElement
{
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
    public AstValue<SizeUnit?> AutoFontSizePadding { get; set; } = new();

    /// <summary>
    /// Sets the word breaking to use when determining the font size
    /// </summary>
    [AstAttribute("auto-font-size-word-breaking", typeof(WordBreaking))]
    public AstValue<string?> WordBreaking { get; set; } = new();
}
