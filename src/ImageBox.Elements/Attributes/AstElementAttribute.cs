namespace ImageBox.Elements.Attributes;

/// <summary>
/// Indicates that the class is available as a custom element in the drawing abstract syntax tree
/// </summary>
/// <param name="tag">The name of the tag this element represents</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class AstElementAttribute(string tag) : Attribute
{
    /// <summary>
    /// The name of the element in the AST
    /// </summary>
    public string Tag { get; } = tag;
}