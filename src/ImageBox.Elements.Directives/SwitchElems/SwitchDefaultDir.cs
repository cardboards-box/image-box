namespace ImageBox.Elements.Directives.SwitchElems;

/// <summary>
/// The default case for a switch statement
/// </summary>
[AstElement("default", ScopeType.Template)]
public class SwitchDefaultDir : Element, IParentElement
{
    /// <summary>
    /// The children elements of the directive
    /// </summary>
    public IElement[] Children { get; set; } = [];
}
