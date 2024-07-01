namespace ImageBox.Rendering.Directives;

/// <summary>
/// Represents a for directive
/// </summary>
/// <param name="_execution">The script execution service</param>
[AstElement("range")]
public class RangeDir(IScriptExecutionService _execution) : DirectiveElement
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
    public override async Task Render(RenderContext context)
    {
        var start = Start.Value ?? 0;
        var step = Step.Value ?? 1;
        var end = End.Value;

        for(var i = start; i < end; i += step)
        {
            using var scope = _execution.Scope(context, this, c =>
            {
                if (!string.IsNullOrWhiteSpace(Let))
                    c.SetVar(Let, i);
            });
            foreach (var child in Children)
                if (child is RenderElement render)
                    await render.Render(context);
        }
    }
}