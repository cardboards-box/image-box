namespace ImageBox.Ast;

/// <summary>
/// Represents an attribute on a <see cref="AstElement"/>
/// </summary>
/// <param name="Name">The name of the attribute (Can be the name of the spread object in the case that <see cref="Type"/> is <see cref="AstAttributeType.Spread"/>)</param>
/// <param name="Type">The type of ast attribute</param>
/// <param name="Value">The value of the attribute (Can be the bind type or script if the case that <see cref="Type"/> is <see cref="AstAttributeType.Bind"/>)</param>
public record class AstAttribute(
    string Name,
    AstAttributeType Type,
    string? Value)
{
    /// <summary>
    /// Cache object for attributes
    /// </summary>
    public object? Cache { get; set; }

    /// <summary>
    /// Indicates that the value of the attribute comes from a script binding
    /// </summary>
    /// <param name="name">The attribute name</param>
    /// <param name="value">The attribute value</param>
    /// <returns>The created AST attribute</returns>
    public static AstAttribute Bind(string name, string value) => new(name, AstAttributeType.Bind, value);

    /// <summary>
    /// Indicates that the attribute is actually a spread object and can resolve to multiple attributes
    /// </summary>
    /// <param name="name">The expression to resolve the spread value</param>
    /// <returns>The created AST attribute</returns>
    public static AstAttribute Spread(string name) => new(name, AstAttributeType.Spread, null);

    /// <summary>
    /// Indicates that the attribute has no value and is a boolean true
    /// </summary>
    /// <param name="name">The attribute name</param>
    /// <returns>The created AST attribute</returns>
    public static AstAttribute BooleanTrue(string name) => new(name, AstAttributeType.BooleanTrue, null);

    /// <summary>
    /// Indicates that the attribute has a string value and requires no further processing
    /// </summary>
    /// <param name="name">The attribute name</param>
    /// <param name="value">The attribute value</param>
    /// <returns>The created AST attribute</returns>
    public static AstAttribute Text(string name, string? value) => new(name, AstAttributeType.Value, value);
}