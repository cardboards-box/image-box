namespace ImageBox.Elements.Base;

/// <summary>
/// Indicates that an element is expecting children
/// </summary>
public interface IParentElement : IElement
{
    /// <summary>
    /// All of the child elements on the parent element
    /// </summary>
    IElement[] Children { get; set; }
}