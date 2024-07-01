using IFontStyle = SixLabors.Fonts.FontStyle;

namespace ImageBox.Rendering.Base;

/// <summary>
/// Represents a GDI element that can be drawn with positional data
/// </summary>
/// <param name="_execution">The script execution service</param>
public abstract class PositionalElement(IScriptExecutionService _execution) : RenderElement
{
    /// <summary>
    /// The script execution service
    /// </summary>
    public IScriptExecutionService Executor { get; } = _execution;

    /// <summary>
    /// The x offset
    /// </summary>
    [AstAttribute("x")]
    public AstValue<SizeUnit?> X { get; set; } = new();

    /// <summary>
    /// The y offset
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
    public ScopeContext Scoped(RenderContext context)
    {
        var previousScope = context.CurrentScope.Size;
        var fontSize = FontSize.Value?.Pixels(previousScope) ?? previousScope.FontSize;
        var current = BoundContext(previousScope, fontSize);
        return Executor.Scope(context, this, c => c.SetSize(current));
    }

    /// <summary>
    /// Gets the font for the current element
    /// </summary>
    /// <param name="context">The font context</param>
    /// <returns>The font</returns>
    public Font GetFont(ScopeContext context)
    {
        var fontName = FontFamily.Value;
        if (string.IsNullOrEmpty(fontName))
            throw new RenderContextException(
                "Font family is required for this element", 
                context.Context.Context, Context);

        var style = IFontStyle.Regular;
        if (!string.IsNullOrEmpty(FontStyle.Value) &&
            Enum.TryParse<IFontStyle>(FontStyle.Value, true, out var parsed))
            style = parsed;

        return context.Context.Fonts.GetFont(fontName, context.Scope, style);
    }
}