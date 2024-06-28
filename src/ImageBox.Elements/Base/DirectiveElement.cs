namespace ImageBox.Elements.Base;

/// <summary>
/// An element that can change the behavior of the syntax tree
/// </summary>
public abstract class DirectiveElement : Element, IParentElement
{
    /// <summary>
    /// The children elements of the directive
    /// </summary>
    public IElement[] Children { get; set; } = [];
}
