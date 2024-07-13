using IFontStyle = SixLabors.Fonts.FontStyle;

namespace ImageBox.Rendering.Base;

/// <summary>
/// Represents a GDI element that can be drawn with positional data
/// </summary>
public abstract class PositionalElement : RenderElement
{
    /// <summary>
    /// The X offset
    /// </summary>
    [AstAttribute("X")]
    public AstValue<SizeUnit?> X { get; set; } = new();

    /// <summary>
    /// The Y offset
    /// </summary>
    [AstAttribute("Y")]
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
    [AstAttribute("font-style")]
    public AstValue<string?> FontStyle { get; set; } = new();

    /// <summary>
    /// Gets the context from the positional data
    /// </summary>
    /// <param name="parent">The size context to bind from</param>
    /// <param name="fontSize">The size of the font in the context</param>
    /// <returns>The size context</returns>
    public SizeContext BoundContext(SizeContext parent, int? fontSize = null)
    {
        var x = X.Value?.Pixels(parent, true) ?? 0;
        var y = Y.Value?.Pixels(parent, false) ?? 0;
        var width = Width.Value?.Pixels(parent, true);
        var height = Height.Value?.Pixels(parent, false);

        return parent.GetContext(x, y, width, height, fontSize);
    }

    /// <summary>
    /// Gets the current scope from the context
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public ContextScope Scoped(ContextFrame context)
    {
        var previousScope = context.LastScope.Size;
        var fontSize = FontSize.Value?.Pixels(previousScope) ?? previousScope.FontSize;
        var current = BoundContext(previousScope, fontSize);
        return context.Scope(this, current);
    }

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