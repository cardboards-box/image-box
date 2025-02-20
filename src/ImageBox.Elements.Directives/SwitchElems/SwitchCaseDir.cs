namespace ImageBox.Elements.Directives.SwitchElems;

/// <summary>
/// The case statement for the <see cref="SwitchDir"/> directive
/// </summary>
[AstElement("case", ScopeType.Template)]
public class SwitchCaseDir : Element, IParentElement
{
    /// <summary>
    /// The value to compare against
    /// </summary>
    [AstAttribute("when"), AstAttribute("con"), AstAttribute("condition"), AstAttribute("value")]
    public AstValue<object> Value { get; set; } = new();

    /// <summary>
    /// The children elements of the directive
    /// </summary>
    public IElement[] Children { get; set; } = [];
}
