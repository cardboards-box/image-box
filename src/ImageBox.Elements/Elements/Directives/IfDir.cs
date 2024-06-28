namespace ImageBox.Elements.Elements.Directives;

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
}