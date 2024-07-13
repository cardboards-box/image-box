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
    /// Checks if the <see cref="Condition"/> is true and renders the children
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    public override async Task Render(ContextFrame context)
    {
        if (!Condition.Value) return;

        using var scope = context.Scope(this);
        foreach (var child in Children)
            if (child is RenderElement render)
                await render.Render(context);
    }
}