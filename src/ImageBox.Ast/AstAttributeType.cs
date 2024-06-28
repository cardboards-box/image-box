namespace ImageBox.Ast;

/// <summary>
/// Indicates the type of value an AST attribute can have
/// </summary>
public enum AstAttributeType
{
    /// <summary>
    /// Indicates that the attribute has a string value and requires no further processing
    /// </summary>
    Value = 0,

    /// <summary>
    /// Indicates that the value of the attribute comes from a script binding
    /// </summary>
    Bind = 1,

    /// <summary>
    /// Indicates that the attribute is actually a spread object and can resolve to multiple attributes
    /// </summary>
    Spread = 2,

    /// <summary>
    /// Indicates that the attribute has no value and is a boolean true
    /// </summary>
    BooleanTrue = 3,
}
