namespace ImageBox.Elements.Drawing;

/// <summary>
/// Represents an image that can be drawn to the image
/// </summary>
/// <param name="_resolver">The file resolution service</param>
[AstElement("image", ScopeType.Template), AstElement("img", ScopeType.Template)]
public class ImageElem(IFileResolverService _resolver) : PositionalElement, IFileElement
{
    /// <summary>
    /// The images source
    /// </summary>
    [AstAttribute("src", true), AstAttribute("source", true)]
    public AstValue<IOPath> Source { get; set; } = new();

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
    /// The optional User-Agent header for fetching the file
    /// </summary>
    [AstAttribute("user-agent")]
    public AstValue<string?> UserAgent { get; set; } = new();

    /// <summary>
    /// The optional Accepts header for fetching the file
    /// </summary>
    [AstAttribute("accepts")]
    public AstValue<string?> Accepts { get; set; } = new();

    /// <summary>
    /// Indicates whether or not the file should be cached locally
    /// </summary>
    [AstAttribute("should-cache")]
    public AstValue<bool?> ShouldCache { get; set; } = new();

    /// <summary>
    /// Gets the image stream from the path
    /// </summary>
    /// <param name="context">The scoped context</param>
    /// <param name="path">The path to fetch the image from</param>
    /// <returns>The stream for the image</returns>
    public async Task<Stream> HandleImage(ContextScope context, IOPath path)
    {
        var wrkDir = context.Frame.BoxContext.Ast.WorkingDirectory;
        var props = this.Properties(context.Size, wrkDir);
        var (stream, _, _, _) = await _resolver.Fetch(props);
        return stream;
    }

    /// <summary>
    /// Applies the element to the render context
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    public override async Task Render(ContextFrame context)
    {
        using var scope = this.Scoped(context);

        var rect = scope.Size.GetRectangle();
        using var imageStream = await HandleImage(scope, Source.Value);
        using var image = Image.Load(imageStream);
        image.Mutate(i => i.Resize(rect.Width, rect.Height));

        if (FlipVertical.Value || FlipHorizontal.Value)
            image.Mutate(i =>
            {
                if (FlipVertical.Value) i.Flip(FlipMode.Vertical);
                if (FlipHorizontal.Value) i.Flip(FlipMode.Horizontal);
            });

        var output = new Vector2();
        if (Rotate.Value.HasValue)
        {
            var centerPoint = rect.Center();
            image.Mutate(i => i.Rotate((float)Rotate.Value.Value));
            var rotated = new Point(rect.X + image.Width / 2, rect.Y + image.Height / 2);
            output = new Vector2(rotated.X - centerPoint.X, rotated.Y - centerPoint.Y);
        }

        context.Image.Mutate(i => i.DrawImage(image, new Point(rect.X - (int)output.X, rect.Y - (int)output.Y), 1));
    }
}