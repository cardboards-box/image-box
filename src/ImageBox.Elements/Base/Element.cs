namespace ImageBox.Elements.Base;

using Ast;

/// <summary>
/// Represents an element that is a drawing abstract syntax tree element
/// </summary>
public abstract class Element : IElement
{
    private string? _elementName;

    /// <summary>
    /// The <see cref="AstElement"/> that created this element
    /// </summary>
    /// <remarks>This can be used to find the original position of the element within AST</remarks>
    public AstElement? Context { get; set; }

    /// <summary>
    /// The <see cref="ReflectedElement"/> information for this element
    /// </summary>
    public ReflectedElement? Reflected { get; set; }

    /// <summary>
    /// The tag name the element is represented by in the AST
    /// </summary>
    public string Tag => _elementName ??= GetElementName();

    /// <summary>
    /// Gets the name of the element from the <see cref="AstElementAttribute"/> attribute
    /// </summary>
    /// <returns>The tag from the <see cref="AstElementAttribute"/></returns>
    /// <exception cref="NullReferenceException">Thrown if the concrete implementation doesn't implement <see cref="AstElementAttribute"/></exception>
    private string GetElementName()
    {
        return GetType().GetCustomAttribute<AstElementAttribute>()?.Tag
            ?? throw new NullReferenceException(
                $"Class inherits from {nameof(Element)} but does not implement the {nameof(AstElementAttribute)} attribute");
    }
}
