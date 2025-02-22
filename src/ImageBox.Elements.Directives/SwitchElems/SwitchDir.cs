namespace ImageBox.Elements.Directives.SwitchElems;

/// <summary>
/// Switch directive for templates
/// </summary>
[AstElement("switch", ScopeType.Template)]
public class SwitchDir : DirectiveElement
{
    /// <summary>
    /// The value to switch on
    /// </summary>
    [AstAttribute("value"), AstAttribute("target"), AstAttribute("condition"), AstAttribute("con")]
    public AstValue<object?> Value { get; set; } = new();

    /// <summary>
    /// All of the switch cases
    /// </summary>
    public IEnumerable<SwitchCaseDir> Cases => Children.OfType<SwitchCaseDir>();

    /// <summary>
    /// All of the default cases
    /// </summary>
    /// <remarks>Realistically, there should only be one, but... ya never know...</remarks>
    public IEnumerable<SwitchDefaultDir> Defaults => Children.OfType<SwitchDefaultDir>();

    /// <summary>
    /// Switches based on the <see cref="Value"/>
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException">Thrown if there are no cases or defaults</exception>
    public override async Task Render(ContextFrame context)
    {
        var cases = Cases;
        var defaults = Defaults;

        if (!cases.Any() && !defaults.Any())
            throw new RenderContextException("A switch directive must have at least one case or default directive",
                context.BoxContext.Ast, Context);

        var value = Value.Value?.ToString();
        foreach (var item in cases)
        {
            var caseValue = item.Value.Value?.ToString();
            var isMatch = (value is null && caseValue is null) ||
                (value is not null && value.Equals(caseValue));

            if (!isMatch) continue;

            await item.RenderChildren(context);
            return;
        }

        foreach (var item in defaults)
            await item.RenderChildren(context);
    }
}
