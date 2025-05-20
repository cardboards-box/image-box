namespace ImageBox.Elements;

/// <summary>
/// An element that can change the behavior of the syntax tree
/// </summary>
public abstract class DirectiveElement : RenderElement, IParentElement
{
    /// <inheritdoc/>
    public IElement[] Children { get; set; } = [];
}
