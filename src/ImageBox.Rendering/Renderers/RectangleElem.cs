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
    public AstValue<SizeUnit?> Radius { get; set; } = new();

    /// <summary>
    /// The color to fill with
    /// </summary>
    [AstAttribute("color")]
    public AstValue<string?> Color { get; set; } = new();

    /// <summary>
    /// The color of the border of the rectangle
    /// </summary>
    [AstAttribute("border-color")]
    public AstValue<string?> BorderColor { get; set; } = new();

    /// <summary>
    /// The width of the border of the rectangle
    /// </summary>
    [AstAttribute("border-width")]
    public AstValue<SizeUnit?> BorderWidth { get; set; } = new();

    /// <summary>
    /// All of the child elements on the parent element
    /// </summary>
    public IElement[] Children { get; set; } = [];

    /// <summary>
    /// Applies the element to the render context
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    public override async Task Render(ContextFrame context)
    {
        var scope = context.LastScope;
        var current = BoundContext(scope.Size);
        var radius = (Radius.Value ?? SizeUnit.Zero).Pixels(current);
        var rect = current.GetRectangle().Rounded(radius);

        if (!string.IsNullOrWhiteSpace(Color.Value))
        {
            var color = Color.Value.ParseColor();
            context.Image.Mutate(x => x.Fill(color, rect));
        }

        if (!string.IsNullOrWhiteSpace(BorderColor.Value) && BorderWidth.Value is not null)
        {
            var color = BorderColor.Value.ParseColor();
            var width = BorderWidth.Value.Value.Pixels(current);
            context.Image.Mutate(x => x.Draw(color, width, rect));
        }

        if (Children.Length == 0) return;

        using var childScope = context.Scope(this, current);
        foreach (var child in Children)
            if (child is RenderElement render)
                await render.Render(context);
    }
}