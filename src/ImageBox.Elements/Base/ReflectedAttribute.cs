namespace ImageBox.Elements.Base;

/// <summary>
/// Represents the attribute binds to POCO properties
/// </summary>
/// <param name="Type">The property info</param>
/// <param name="Attributes">The attributes on the property</param>
/// <param name="IsBindable">Whether or not the value is a <see cref="AstValue{T}"/></param>
/// <param name="BindType">The generic type of the <see cref="AstValue{T}"/> if <paramref name="IsBindable"/> is true</param>
public record class ReflectedAttribute(
    PropertyInfo Type,
    AstAttributeAttribute[] Attributes,
    bool IsBindable,
    Type? BindType);