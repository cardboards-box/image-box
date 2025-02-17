namespace ImageBox.Cli.Documentation;

/// <summary>
/// Indicates the different types of values an element can have
/// </summary>
public enum ElementType
{
    /// <summary>
    /// The element has child elements
    /// </summary>
    Children,
    /// <summary>
    /// The element has a text value
    /// </summary>
    Value,
    /// <summary>
    /// The element is self closing and has no children or value
    /// </summary>
    SelfClosing,
}
