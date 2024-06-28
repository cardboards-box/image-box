namespace ImageBox.Ast;

/// <summary>
/// Indicates the type of children an element can have
/// </summary>
public enum AstElementType
{
    /// <summary>
    /// Element has text as children
    /// </summary>
    Text = 0,

    /// <summary>
    /// Element has other elements as children
    /// </summary>
    Children = 1,

    /// <summary>
    /// Element has no children, but isn't self closing
    /// </summary>
    Empty = 2,
}
