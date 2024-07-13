namespace ImageBox.Rendering.Renderers;

/// <summary>
/// The clear element
/// </summary>
[AstElement("clear")]
public class ClearElem : RenderElement
{
    /// <summary>
    /// The color to clear with
    /// </summary>
    [AstAttribute("color")]
    public AstValue<string> Color { get; set; } = new();

    /// <summary>
    /// Applies the element to the render context
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    public override Task Render(ContextFrame context)
    {
        context.Image.Mutate(i => i.Clear(Color.Value.ParseColor()));
        return Task.CompletedTask;
    }
}
