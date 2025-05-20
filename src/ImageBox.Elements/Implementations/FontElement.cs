using IFontStyle = SixLabors.Fonts.FontStyle;

namespace ImageBox.Elements;

/// <inheritdoc/>
public abstract class FontElement : PositionalElement, IFontElement
{
    /// <inheritdoc/>
    [AstAttribute("font-size")]
    public AstValue<SizeUnit?> FontSize { get; set; } = new();

    /// <inheritdoc/>
    [AstAttribute("font-family")]
    public AstValue<string?> FontFamily { get; set; } = new();

    /// <inheritdoc/>
    [AstAttribute("font-style", typeof(IFontStyle))]
    public AstValue<string?> FontStyle { get; set; } = new();

    /// <inheritdoc/>
    [AstAttribute("auto-font-size")]
    public AstValue<bool?> AutoFontSize { get; set; } = new();

    /// <inheritdoc/>
    [AstAttribute("auto-font-size-padding")]
    public AstValue<SizeUnit?> AutoFontSizePadding { get; set; } = new();

    /// <inheritdoc/>
    [AstAttribute("auto-font-size-word-breaking", typeof(WordBreaking)), AstAttribute("word-breaking", typeof(WordBreaking))]
    public AstValue<string?> WordBreaking { get; set; } = new();
}
