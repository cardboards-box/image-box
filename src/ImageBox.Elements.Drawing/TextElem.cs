using IColor = SixLabors.ImageSharp.Color;
using EOriginType = ImageBox.Drawing.OriginType;

namespace ImageBox.Elements.Drawing;

/// <summary>
/// Represents text to be drawn to the image
/// </summary>
[AstElement("text", ScopeType.Template)]
public class TextElem : FontElement
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
    [AstAttribute("align-vertical", typeof(VerticalAlignment))]
    public AstValue<string?> AlignVertical { get; set; } = new();

    /// <summary>
    /// Where to align the text horizontally in the rectangle
    /// </summary>
    [AstAttribute("align-horizontal", typeof(HorizontalAlignment))]
    public AstValue<string?> AlignHorizontal { get; set; } = new();

    /// <summary>
    /// How to align the text within the rectangle
    /// </summary>
    [AstAttribute("align-text", typeof(TextAlignment))]
    public AstValue<string?> AlignText { get; set; } = new();

    /// <summary>
    /// The number of degrees to rotate the image before rendering
    /// </summary>
    [AstAttribute("rotate")]
    public AstValue<double?> Rotate { get; set; } = new();

    /// <summary>
    /// How to determine the origin point of the text rotation within the current box
    /// </summary>
    /// <remarks>Ignored if both <see cref="RotateOriginX"/> and <see cref="RotateOriginY"/> are set</remarks>
    [AstAttribute("rotate-origin-type", typeof(EOriginType))]
    public AstValue<string?> RotateOriginType { get; set; } = new();

    /// <summary>
    /// The x coordinate of the origin point of the text rotation
    /// </summary>
    /// <remarks>Requires <see cref="RotateOriginY"/> to be set as well</remarks>
    [AstAttribute("rotate-origin-x")]
    public AstValue<SizeUnit?> RotateOriginX { get; set; } = new();

    /// <summary>
    /// The y coordinate of the origin point of the text rotation
    /// </summary>
    /// <remarks>Requires <see cref="RotateOriginX"/> to be set as well</remarks>
    [AstAttribute("rotate-origin-y")]
    public AstValue<SizeUnit?> RotateOriginY { get; set; } = new();

    /// <summary>
    /// How to determine the origin point of the text within the current box
    /// </summary>
    /// <remarks>Ignored if both <see cref="OriginX"/> and <see cref="OriginY"/> are set</remarks>
    [AstAttribute("origin-type", typeof(EOriginType))]
    public AstValue<string?> OriginType { get; set; } = new();

    /// <summary>
    /// The x coordinate of the origin point
    /// </summary>
    /// <remarks>Requires <see cref="OriginY"/> to be set as well</remarks>
    [AstAttribute("origin-x")]
    public AstValue<SizeUnit?> OriginX { get; set; } = new();

    /// <summary>
    /// The y coordinate of the origin point
    /// </summary>
    /// <remarks>Requires <see cref="OriginX"/> to be set as well</remarks>
    [AstAttribute("origin-y")]
    public AstValue<SizeUnit?> OriginY { get; set; } = new();

    /// <summary>
    /// Determines the origin point of the text
    /// </summary>
    /// <param name="context">The current scope's context</param>
    /// <param name="bounds">The parent's bounding rectangle</param>
    /// <returns>The origin point based on the configuration</returns>
    public Vector2 DetermineOrigin(ContextScope context, Rectangle bounds)
    {
        if (OriginX.Value is not null && OriginY.Value is not null)
        {
            var x = OriginX.Value.Value.Pixels(context.Size, true);
            var y = OriginY.Value.Value.Pixels(context.Size, false);
            return new(x, y);
        }

        if (!Enum.TryParse<EOriginType>(OriginType.Value, true, out var type))
            type = EOriginType.Center;

        return bounds.Origin(type);
    }

    /// <summary>
    /// Gets the drawing options with the appropriate transforms for the text rendering
    /// </summary>
    /// <param name="context">The current scope's context</param>
    /// <param name="bounds">The parent's bounding rectangle</param>
    /// <returns>The drawing options for the current text element</returns>
    public DrawingOptions GetDrawingOptions(ContextScope context, Rectangle bounds)
    {
        Vector2 DetermineRotationOrigin()
        {
            if (RotateOriginX.Value.HasValue && RotateOriginY.Value.HasValue)
            {
                var x = RotateOriginX.Value.Value.Pixels(context.Size, true);
                var y = RotateOriginY.Value.Value.Pixels(context.Size, false);
                return new(x, y);
            }

            if (Enum.TryParse<OriginType>(RotateOriginType.Value, true, out var type))
                return bounds.Origin(type);

            return DetermineOrigin(context, bounds);
        }

        if (!Rotate.Value.HasValue) return new DrawingOptions();

        var point = DetermineRotationOrigin();
        var rotation = (float)Rotate.Value.Value;
        var transform = Matrix3x2Extensions.CreateRotationDegrees(rotation, point);
        return new DrawingOptions
        {
            Transform = transform
        };
    }

    /// <summary>
    /// Applies the element to the render context
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    public override Task Render(ContextFrame context)
    {
        if (string.IsNullOrWhiteSpace(Value.Value))
            return Task.CompletedTask;

        using var scope = this.Scoped(context);

        var rect = scope.Size.GetRectangle();
        var color = Color.Value.ParseColor(IColor.Black);

        if (!Enum.TryParse<VerticalAlignment>(AlignVertical.Value, true, out var vAlign))
            vAlign = VerticalAlignment.Center;

        if (!Enum.TryParse<HorizontalAlignment>(AlignHorizontal.Value, true, out var hAlign))
            hAlign = HorizontalAlignment.Center;

        if (!Enum.TryParse<TextAlignment>(AlignText.Value, true, out var tAlign))
            tAlign = TextAlignment.Center;

        if (!Enum.TryParse<WordBreaking>(WordBreaking?.Value, true, out var workBreaking))
            workBreaking = SixLabors.Fonts.WordBreaking.Standard;

        var text = Value.Value;
        var drawing = GetDrawingOptions(scope, rect);
        var opts = new RichTextOptions(this.GetFont(text, scope))
        {
            HorizontalAlignment = hAlign,
            VerticalAlignment = vAlign,
            TextAlignment = tAlign,
            Origin = DetermineOrigin(scope, rect),
            WrappingLength = rect.Width,
            WordBreaking = workBreaking
        };
        var brush = new SolidBrush(color);
        context.Image.Mutate(i => i.DrawText(drawing, opts, text, brush, null));
        return Task.CompletedTask;
    }
}