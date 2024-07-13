namespace ImageBox.Rendering.Directives;

/// <summary>
/// Represents a for-each directive
/// </summary>
[AstElement("foreach")]
public class ForEachDir : DirectiveElement
{
    /// <summary>
    /// Iterate through each of the values
    /// </summary>
    [AstAttribute("each")]
    public AstValue<object[]> Each { get; set; } = new();

    /// <summary>
    /// What to name the value in the children template contexts
    /// </summary>
    [AstAttribute("let")]
    public string? Let { get; set; }

    /// <summary>
    /// Renders each of the children for each value in the <see cref="Each"/>
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    public override async Task Render(ContextFrame context)
    {
        if (string.IsNullOrWhiteSpace(Let)) 
            throw new RenderContextException(
                "The 'let' attribute is required for the foreach directive", 
                context.BoxContext.Ast, Context);

        foreach(var value in Each.Value ?? [])
        {
            using var scope = context.Scope(this, null, new Dictionary<string, object?> { [Let] = value });
            foreach (var child in Children)
                if (child is RenderElement render)
                    await render.Render(context);
        }
    }
}
