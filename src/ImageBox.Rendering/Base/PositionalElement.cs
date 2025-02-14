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
    /// Gets the font for the current element
    /// </summary>
    /// <param name="context">The font context</param>
    /// <returns>The font</returns>
    public Font GetFont(ContextScope context)
    {
        var fontName = FontFamily.Value ?? context.Size.FontFamily;
        if (string.IsNullOrEmpty(fontName))
            throw new RenderContextException(
                "Font family is required for this element", 
                context.Frame.BoxContext.Ast, Context);

        var style = IFontStyle.Regular;
        if (!string.IsNullOrEmpty(FontStyle.Value) &&
            Enum.TryParse<IFontStyle>(FontStyle.Value, true, out var parsed))
            style = parsed;

        return context.Frame.BoxContext.Fonts.GetFont(fontName, context, style);
    }
}