namespace ImageBox.Rendering.Renderers;

/// <summary>
/// Represents a rectangle that can be filled or bordered
/// </summary>
[AstElement("rectangle")]
public class RectangleElem : PositionalElement, IParentElement
{
    /// <summary>
    /// The radius of the curved corners
    /// </summary>
    [AstAttribute("radius")]
    public AstValue<SizeUnit> Radius { get; set; } = new();

    /// <summary>
    /// The color to fill with
    /// </summary>
    [AstAttribute("color")]
    public AstValue<string> Color { get; set; } = new();

    /// <summary>
    /// The color of the border of the rectangle
    /// </summary>
    [AstAttribute("border-color")]
    public AstValue<string> BorderColor { get; set; } = new();

    /// <summary>
    /// The width of the border of the rectangle
    /// </summary>
    [AstAttribute("border-width")]
    public AstValue<SizeUnit> BorderWidth { get; set; } = new();

    /// <summary>
    /// All of the child elements on the parent element
    /// </summary>
    public IElement[] Children { get; set; } = [];

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