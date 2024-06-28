namespace ImageBox.Elements.Attributes;

/// <summary>
/// Indicates that the property can be bound to an attribute in the drawing abstract syntax tree
/// </summary>
/// <param name="name">The name of the attribute</param>
/// <param name="required">Whether or not the attribute is required</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class AstAttributeAttribute(string name, bool required = false) : Attribute
{
    /// <summary>
    /// The name of the attribute
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Whether or not the attribute is required
    /// </summary>
    public bool Required { get; } = required;
}
