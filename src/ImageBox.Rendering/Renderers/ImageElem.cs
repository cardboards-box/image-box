namespace ImageBox.Rendering.Renderers;

/// <summary>
/// Represents an image that can be drawn to the image
/// </summary>
[AstElement("image")]
public class ImageElem : PositionalElement
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
    /// Applies the element to the render context
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    public override Task Render(RenderContext context)
    {
        return Task.CompletedTask;
    }
}