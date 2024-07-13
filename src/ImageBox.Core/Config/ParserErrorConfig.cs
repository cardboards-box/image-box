namespace ImageBox.Core.Config;

/// <summary>
/// The configuration for error handling for AST parsing
/// </summary>
public class ParserErrorConfig
{
    /// <summary>
    /// Throw an exception when a matching attribute property is not found
    /// </summary>
    /// <value>Default value is true, default config path is `ImageBox:Parser:Errors:AttributeMoreThanOne`.</value>
    public bool AttributeMoreThanOne { get; set; } = true;

    /// <summary>
    /// Throw an exception when an AST attribute a binding expression, but the property doesn't support binding.
    /// </summary>
    /// <value>Default value is true, default config path is `ImageBox:Parser:Errors:AttributeBindInvalid`.</value>
    public bool AttributeBindInvalid { get; set; } = true;

    /// <summary>
    /// Throw an exception when an element has both children and text values
    /// </summary>
    /// <value>Default value is true, default config path is `ImageBox:Parser:Errors:ElementInvalidChild`.</value>
    public bool ElementInvalidChild { get; set; } = true;

    /// <summary>
    /// Throw an exception when a matching element class is not found
    /// </summary>
    /// <value>Default value is true, default config path is `ImageBox:Parser:Errors:ElementNotFound`.</value>
    public bool ElementNotFound { get; set; } = true;

    /// <summary>
    /// Throw an exception when more than one matching element class is found
    /// </summary>
    /// <value>Default value is true, default config path is `ImageBox:Parser:Errors:ElementMoreThanOne`.</value>
    public bool ElementMoreThanOne { get; set; } = true;

    /// <summary>
    /// Throw an exception when an instance of an element class cannot be created
    /// </summary>
    /// <value>Default value is true, default config path is `ImageBox:Parser:Errors:InvalidInstance`.</value>
    public bool InvalidInstance { get; set; } = true;
}
