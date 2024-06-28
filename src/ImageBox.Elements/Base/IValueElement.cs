namespace ImageBox.Elements.Base;

/// <summary>
/// Indicates that an element has a string value
/// </summary>
public interface IValueElement : IElement
{
    /// <summary>
    /// The value of the element
    /// </summary>
    string? Value { get; set; }
}