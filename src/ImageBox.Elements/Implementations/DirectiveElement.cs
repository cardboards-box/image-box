namespace ImageBox.Elements;

/// <summary>
/// An element that can change the behavior of the syntax tree
/// </summary>
public abstract class DirectiveElement : RenderElement, IParentElement
{
    /// <summary>
    /// The children elements of the directive
    /// </summary>
    public IElement[] Children { get; set; } = [];
}
