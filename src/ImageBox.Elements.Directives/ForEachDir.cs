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
        int index = -1;
        foreach (var value in Each.Value ?? [])
        {
            index++;
            var vars = new Dictionary<string, object?>{};
            if (!string.IsNullOrWhiteSpace(Let))
                vars.Add(Let, value);

            if (!string.IsNullOrWhiteSpace(Index))
                vars.Add(Index, index);

            await this.RenderChildren(context, null, vars);
        }
    }
}
