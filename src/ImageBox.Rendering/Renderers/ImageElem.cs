using System.Numerics;

namespace ImageBox.Rendering.Renderers;

/// <summary>
/// Represents an image that can be drawn to the image
/// </summary>
/// <param name="_execution">The script execution service</param>
/// <param name="_resolver">The file resolution service</param>
/// <param name="_svg">The SVG renderer service</param>
[AstElement("image")]
public class ImageElem(
    IScriptExecutionService _execution,
    IFileResolverService _resolver,
    ISvgService _svg) : PositionalElement(_execution)
{
    /// <summary>
    /// The images source
    /// </summary>
    [AstAttribute("src"), AstAttribute("source")]
    public AstValue<IOPath> Source { get; set; } = new();

    /// <summary>
    /// The position of the image within the bounds of the rectangle
    /// </summary>
    [AstAttribute("position")]
    public AstValue<string> Position { get; set; } = new();

    /// <summary>
    /// The number of degrees to rotate the image before rendering
    /// </summary>
    [AstAttribute("rotate")]
    public AstValue<double?> Rotate { get; set; } = new();

    /// <summary>
    /// Whether to flip the image vertically or not
    /// </summary>
    [AstAttribute("flip-vertical")]
    public AstValue<bool> FlipVertical { get; set; } = new();

    /// <summary>
    /// Whether to flip the image horizontally or not
    /// </summary>
    [AstAttribute("flip-horizontal")]
    public AstValue<bool> FlipHorizontal { get; set; } = new();

    /// <summary>
    /// Gets the image stream from the path
    /// </summary>
    /// <param name="context">The scoped context</param>
    /// <param name="path">The path to fetch the image from</param>
    /// <returns>The stream for the image</returns>
    public async Task<Stream> HandleImage(ScopeContext context, IOPath path)
    {
        var wrkDir = context.Context.Context.WorkingDirectory;
        var imgPath = path.GetAbsolute(wrkDir);
        var (stream, _, type) = await _resolver.Fetch(imgPath);
        if (type == "image/svg+xml")
            return _svg.GetStream(stream, new RenderOptions
            {
                Width = context.Size.Width,
                Height = context.Size.Height
            });

        return stream;
    }

    /// <summary>
    /// Applies the element to the render context
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    public override async Task Render(RenderContext context)
    {
        using var scope = Scoped(context);

        var rect = scope.Size.GetRectangle();
        using var imageStream = await HandleImage(scope, Source.Value);
        using var image = Image.Load(imageStream);
        image.Mutate(i => i.Resize(rect.Width, rect.Height));

        if (FlipVertical.Value)
            image.Mutate(i => i.Flip(FlipMode.Vertical));
        if (FlipHorizontal.Value)
            image.Mutate(i => i.Flip(FlipMode.Horizontal));

        var output = new Vector2();
        if (Rotate.Value.HasValue)
        {
            var centerPoint = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            image.Mutate(i => i.Rotate((float)Rotate.Value.Value));
            var rotated = new Point(rect.X + image.Width / 2, rect.Y + image.Height / 2);
            output = new Vector2(rotated.X - centerPoint.X, rotated.Y - centerPoint.Y);
        }

        context.Image.Mutate(i => i.DrawImage(image, new Point(rect.X - (int)output.X, rect.Y - (int)output.Y), 1));
    }
}