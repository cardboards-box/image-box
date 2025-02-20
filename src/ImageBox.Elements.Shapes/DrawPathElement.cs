namespace ImageBox.Elements.Shapes;

using Drawing;

/// <summary>
/// Represents an element that can be drawn or filled based on <see cref="IPath"/>s
/// </summary>
public abstract class DrawPathElement : PositionalElement, IParentElement
{
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
    /// Get the path of the current element
    /// </summary>
    /// <param name="context">The size of the current context</param>
    /// <returns>The path</returns>
    public abstract IPath GetPath(SizeContext context);

    /// <summary>
    /// Applies the element to the render context
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    public override async Task Render(ContextFrame context)
    {
        var scope = context.LastScope;
        var current = this.BoundContext(scope.Size);
        var path = GetPath(current);

        if (!string.IsNullOrWhiteSpace(Color.Value))
        {
            var color = Color.Value.ParseColor();
            context.Image.Mutate(x => x.Fill(color, path));
        }

        if (!string.IsNullOrWhiteSpace(BorderColor.Value) && BorderWidth.Value is not null)
        {
            var color = BorderColor.Value.ParseColor();
            var width = BorderWidth.Value.Value.Pixels(current);
            context.Image.Mutate(x => x.Draw(color, width, path));
        }

        if (Children.Length == 0) return;

        using var childScope = context.Scope(this, current);
        foreach (var child in Children)
            if (child is RenderElement render)
                await render.Render(context);
    }
}
