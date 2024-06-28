namespace ImageBox.Elements.Elements.Directives;

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
    public AstValue<string> Let { get; set; } = new();
}
