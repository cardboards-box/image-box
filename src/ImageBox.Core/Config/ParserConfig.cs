namespace ImageBox.Core.Config;

/// <summary>
/// The configuration for the AST parser
/// </summary>
public class ParserConfig
{
    /// <summary>
    /// The character that appears before script bind attributes (single character only)
    /// </summary>
    /// <value>Default value is `:`, default config path is `ImageBox:Parser:Bind`.</value>
    public char Bind { get; set; } = ':';

    /// <summary>
    /// The character that indicates the start of a spread attribute (single character only)
    /// </summary>
    /// <value>Default value is `{`, default config path is `ImageBox:Parser:SpreadStart`.</value>
    public char SpreadStart { get; set; } = '{';

    /// <summary>
    /// The character that indicates the end of a spread attribute (single character only)
    /// </summary>
    /// <value>Default value is `}`, default config path is `ImageBox:Parser:SpreadEnd`.</value>
    public char SpreadEnd { get; set; } = '}';

    /// <summary>
    /// Configure error handling for AST parsing
    /// </summary>
    /// <value>Configuration paths can be found under `ImageBox:Parser:Errors`.</value>
    public ParserErrorConfig Errors { get; set; } = new();
}