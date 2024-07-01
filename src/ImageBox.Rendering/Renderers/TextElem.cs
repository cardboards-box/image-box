using IColor = SixLabors.ImageSharp.Color;

namespace ImageBox.Rendering.Renderers;

/// <summary>
/// Represents text to be drawn to the image
/// </summary>
/// <param name="_execution">The script execution service</param>
[AstElement("text")]
public class TextElem(IScriptExecutionService _execution) : PositionalElement(_execution)
{
    /// <summary>
    /// The value of the text to draw to the image
    /// </summary>
    [AstAttribute("value")]
    public AstValue<string?> Value { get; set; } = new();

    /// <summary>
    /// The color to fill with
    /// </summary>
    [AstAttribute("color")]
    public AstValue<string?> Color { get; set; } = new();

    /// <summary>
    /// Where to align the text vertically in the rectangle
    /// </summary>
    [AstAttribute("align-vertical")]
    public AstValue<string?> AlignVertical { get; set; } = new();

    /// <summary>
    /// Where to align the text horizontally in the rectangle
    /// </summary>
    [AstAttribute("align-horizontal")]
    public AstValue<string?> AlignHorizontal { get; set; } = new();

    /// <summary>
    /// How to align the text within the rectangle
    /// </summary>
    [AstAttribute("align-text")]
    public AstValue<string?> AlignText { get; set; } = new();

    /// <summary>
    /// Applies the element to the render context
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    public override Task Render(RenderContext context)
    {
        if (string.IsNullOrWhiteSpace(Value.Value))
            return Task.CompletedTask;

        using var scope = Scoped(context);

        var rect = scope.Size.GetRectangle();
        var color = Color.Value.ParseColor(IColor.Black);

        if (!Enum.TryParse<VerticalAlignment>(AlignVertical.Value, true, out var vAlign))
            vAlign = VerticalAlignment.Center;

        if (!Enum.TryParse<HorizontalAlignment>(AlignHorizontal.Value, true, out var hAlign))
            hAlign = HorizontalAlignment.Center;

        if (!Enum.TryParse<TextAlignment>(AlignText.Value, true, out var tAlign))
            tAlign = TextAlignment.Center;

        var opts = new RichTextOptions(GetFont(scope))
        {
            HorizontalAlignment = hAlign,
            VerticalAlignment = vAlign,
            TextAlignment = tAlign,
            Origin = rect.Center(),
            WrappingLength = rect.Width,
        };
        context.Image.Mutate(i => i.DrawText(opts, Value.Value, color));
        return Task.CompletedTask;
    }
}