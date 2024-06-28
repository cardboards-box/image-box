namespace ImageBox.Elements.Attributes;

/// <summary>
/// Constraints what types of child elements can be used with the current element
/// </summary>
/// <param name="types">The child element types allowed on this element (can be interfaces or abstract types)</param>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class AstConstraintAttribute(params Type[] types) : Attribute
{
    /// <summary>
    /// The child element types allowed on this element
    /// </summary>
    public Type[] ConstrainedTypes { get; } = types;
}
