namespace ImageBox.Ast;

/// <summary>
/// Represents an attribute on a <see cref="AstElement"/>
/// </summary>
public class AstAttribute
{
    /// <summary>
    /// The name of the attribute
    /// </summary>
    /// <remarks>Can be the name of the spread object in the case that <see cref="Type"/> is <see cref="AstAttributeType.Spread"/></remarks>
    public required string Name { get; set; }

    /// <summary>
    /// The type of ast attribute
    /// </summary>
    public required AstAttributeType Type { get; set; }

    /// <summary>
    /// The value of the attribute
    /// </summary>
    /// <remarks>Can be the bind type or script if the case that <see cref="Type"/> is <see cref="AstAttributeType.Bind"/></remarks>
    public string? Value { get; set; }

    /// <summary>
    /// Cache object for attributes
    /// </summary>
    public object? Cache { get; set; }
}
