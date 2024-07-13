namespace ImageBox.Rendering.Directives;

/// <summary>
/// Represents a for directive
/// </summary>
[AstElement("range")]
public class RangeDir : DirectiveElement
{
    /// <summary>
    /// The start of value
    /// </summary>
    [AstAttribute("start")]
    public AstValue<double?> Start { get; set; } = new();

    /// <summary>
    /// The end value of the loop
    /// </summary>
    [AstAttribute("end")]
    public AstValue<double> End { get; set; } = new();

    /// <summary>
    /// The step to increment each iteration by
    /// </summary>
    [AstAttribute("step")]
    public AstValue<double?> Step { get; set; } = new();

    /// <summary>
    /// What to name the value in the children template contexts
    /// </summary>
    [AstAttribute("let")]
    public string? Let { get; set; }

    /// <summary>
    /// Iterate through each of the values and render the children for each iteration
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    public override async Task Render(ContextFrame context)
    {
        var start = Start.Value ?? 0;
        var step = Step.Value ?? 1;
        var end = End.Value;

        for(var i = start; i < end; i += step)
        {
            var vars = new Dictionary<string, object?>();
            if (!string.IsNullOrWhiteSpace(Let))
                vars.Add(Let, i);
            using var scope = context.Scope(this, null, vars);
            foreach (var child in Children)
                if (child is RenderElement render)
                    await render.Render(context);
        }
    }
}