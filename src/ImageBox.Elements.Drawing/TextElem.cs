using IColor = SixLabors.ImageSharp.Color;
using EOriginType = ImageBox.Drawing.OriginType;

namespace ImageBox.Elements.Drawing;

using Shapes;

/// <summary>
/// Represents text to be drawn to the image
/// </summary>
[AstElement("text", ScopeType.Template)]
public class TextElem : FontElement, IPathElement
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
    /// Whether or not to draw the text along the path
    /// </summary>
    /// <remarks>
    /// This requires that the child either be a collection of points or a path element.
    /// </remarks>
    [AstAttribute("draw-along-path")]
    public AstValue<bool?> DrawAlongPath { get; set; } = new();

    /// <summary>
    /// The children of the text element
    /// </summary>
    public IElement[] Children { get; set; } = [];

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

            if (Enum.TryParse<EOriginType>(RotateOriginType.Value, true, out var type))
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
    /// Gets the path to draw the text along
    /// </summary>
    /// <param name="scope">The current scope of the context</param>
    /// <param name="origin">The origin to use for the path</param>
    /// <returns>The path or null if <see cref="DrawAlongPath"/> is null</returns>
    /// <exception cref="RenderContextException">Thrown if there was no path provided</exception>
    public IPath? GetPath(ContextScope scope, Vector2 origin)
    {
        if (!(DrawAlongPath.Value ?? false) ||
            Children.Length <= 0)
            return null;

        var points = this.GetPoints(scope.Size).ToArray();
        if (points.Length > 0)
        {
            var path = new PathBuilder()
                .StartFigure()
                .SetOrigin(origin);

            Point previous = points[0];
            foreach (Point point in points.Skip(1))
            {
                path.AddLine(previous, point);
                previous = point;
            }

            return path.CloseFigure().Build();
        }

        var pathElem = Children.OfType<DrawPathElement>().FirstOrDefault()
            ?? throw new RenderContextException("The text element must have a path to draw along", Context);
        return pathElem.GetPath(scope.Size, origin);
    }

    /// <summary>
    /// Gets the text options for the current element
    /// </summary>
    /// <param name="scope">The current scope of the element</param>
    /// <param name="bounds">The bounds to draw the text in</param>
    /// <param name="text">The text that should be written</param>
    /// <returns>The text options to use for drawing the text</returns>
    public RichTextOptions GetTextOptions(ContextScope scope, Rectangle bounds, string text)
    {
        if (!Enum.TryParse<VerticalAlignment>(AlignVertical.Value, true, out var vAlign))
            vAlign = VerticalAlignment.Center;

        if (!Enum.TryParse<HorizontalAlignment>(AlignHorizontal.Value, true, out var hAlign))
            hAlign = HorizontalAlignment.Center;

        if (!Enum.TryParse<TextAlignment>(AlignText.Value, true, out var tAlign))
            tAlign = TextAlignment.Center;

        if (!Enum.TryParse<WordBreaking>(WordBreaking?.Value, true, out var wordBreaking))
            wordBreaking = SixLabors.Fonts.WordBreaking.Standard;

        var font = this.GetFont(text, scope);
        var origin = DetermineOrigin(scope, bounds);
        var path = GetPath(scope, origin);
        if (path is null)
            return new RichTextOptions(font)
            {
                HorizontalAlignment = hAlign,
                VerticalAlignment = vAlign,
                TextAlignment = tAlign,
                Origin = origin,
                WrappingLength = bounds.Width,
                WordBreaking = wordBreaking,
            };

        return new RichTextOptions(font)
        {
            HorizontalAlignment = hAlign,
            VerticalAlignment = vAlign,
            TextAlignment = tAlign,
            Path = path,
            WrappingLength = path.ComputeLength()
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

        var text = Value.Value!;

        using var scope = this.Scoped(context);

        var rect = scope.Size.GetRectangle();
        var color = Color.Value.ParseColor(IColor.Black);

        var drawing = GetDrawingOptions(scope, rect);
        var opts = GetTextOptions(scope, rect, text);
        var brush = new SolidBrush(color);
        context.Image.Mutate(i => i.DrawText(drawing, opts, text, brush, null));
        return Task.CompletedTask;
    }
}