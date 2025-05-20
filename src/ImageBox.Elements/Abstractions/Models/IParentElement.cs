namespace ImageBox.Elements;

/// <summary>
/// Indicates that an element is expecting children
/// </summary>
public interface IParentElement : IElement
{
    /// <summary>
    /// All of the child elements on the this element
    /// </summary>
    IElement[] Children { get; set; }
}