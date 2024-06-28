namespace ImageBox.Rendering.Directives;

/// <summary>
/// If directive for templates
/// </summary>
[AstElement("if")]
public class IfDir : DirectiveElement
{
    /// <summary>
    /// The condition for the if statement
    /// </summary>
    [AstAttribute("con"), AstAttribute("condition")]
    public AstValue<bool> Condition { get; set; } = new();

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