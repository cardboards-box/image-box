namespace ImageBox.Elements.Directives;

/// <summary>
/// Represents a for-each directive
/// </summary>
[AstElement("foreach", ScopeType.Template)]
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
    /// What to name the index in the children template contexts
    /// </summary>
    [AstAttribute("index")]
    public string? Index { get; set; }

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

        int index = -1;
        foreach (var value in Each.Value ?? [])
        {
            index++;
            var vars = new Dictionary<string, object?> { [Let] = value };
            if (!string.IsNullOrEmpty(Index))
                vars.Add(Index, index);

            using var scope = context.Scope(this, null, vars);
            foreach (var child in Children)
                if (child is RenderElement render)
                    await render.Render(context);
        }
    }
}
