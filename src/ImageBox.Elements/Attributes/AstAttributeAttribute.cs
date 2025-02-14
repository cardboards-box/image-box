namespace ImageBox.Elements.Attributes;

/// <summary>
/// Indicates that the property can be bound to an attribute in the drawing abstract syntax tree
/// </summary>
/// <param name="name">The name of the attribute</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class AstAttributeAttribute(string name) : Attribute
{
    /// <summary>
    /// The name of the attribute
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Whether or not the attribute is required
    /// </summary>
    /// <remarks>This is mostly a documentation hint and isn't enforced in code</remarks>
    public bool Required { get; set; }

    /// <summary>
    /// The type of enum that the attribute is bound to
    /// </summary>
    public Type? EnumType { get; }

    /// <summary>
    /// Indicates that the property can be bound to an attribute in the drawing abstract syntax tree
    /// </summary>
    /// <param name="name">The name of the attribute</param>
    /// <param name="required">Whether or not the attribute is required</param>
    public AstAttributeAttribute(string name, bool required) : this(name)
    {
        Required = required;
    }

    /// <summary>
    /// Indicates that the property can be bound to an attribute in the drawing abstract syntax tree
    /// </summary>
    /// <param name="name">The name of the attribute</param>
    /// <param name="required">Whether or not the attribute is required</param>
    /// <param name="enumType">The type of enum that the attribute is bound to</param>
    public AstAttributeAttribute(string name, Type enumType, bool required = false) : this(name, required)
    {
        EnumType = enumType;
    }
}
