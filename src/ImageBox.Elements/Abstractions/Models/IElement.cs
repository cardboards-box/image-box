namespace ImageBox.Elements;

using Ast;

/// <summary>
/// Indicates that a class is a drawing abstract syntax tree element
/// </summary>
public interface IElement
{
    /// <summary>
    /// The index of this element within it's siblings
    /// </summary>
    int SiblingIndex { get; set; }

    /// <summary>
    /// The parent of the current element
    /// </summary>
    /// <remarks>Null if the element is a top-level element</remarks>
    IElement? ParentElement { get; set; }

    /// <summary>
    /// The <see cref="AstElement"/> that created this element
    /// </summary>
    /// <remarks>This can be used to find the original position of the element within AST</remarks>
    AstElement? Context { get; set; }

    /// <summary>
    /// The <see cref="ReflectedElement"/> information for this element
    /// </summary>
    ReflectedElement? Reflected { get; set; }
}