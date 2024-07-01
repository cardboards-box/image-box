namespace ImageBox.Rendering.Directives;

/// <summary>
/// If directive for templates
/// </summary>
/// <param name="_execution">The script execution service</param>
[AstElement("if")]
public class IfDir(IScriptExecutionService _execution) : DirectiveElement
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
    public override async Task Render(RenderContext context)
    {
        if (!Condition.Value) return;

        using var scope = _execution.Scope(context, this);
        foreach (var child in Children)
            if (child is RenderElement render)
                await render.Render(context);
    }
}