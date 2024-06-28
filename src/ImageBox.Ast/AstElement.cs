namespace ImageBox.Ast;

/// <summary>
/// Represents an element in the custom XML AST for image templates
/// </summary>
public class AstElement
{
    /// <summary>
    /// The position in the stream where this element occurs
    /// </summary>
    public required int StreamPosition { get; set; }

    /// <summary>
    /// The line number this element occurs on
    /// </summary>
    public required int Line { get; set; }

    /// <summary>
    /// The column number on the <see cref="Line"/> this element occurs on
    /// </summary>
    public required int Column { get; set; }

    /// <summary>
    /// The tag of the element
    /// </summary>
    public required string Tag { get; set; }

    /// <summary>
    /// Indicates the type of children on this element
    /// </summary>
    public required AstElementType Type { get; set; }

    /// <summary>
    /// Any attributes present on the element
    /// </summary>
    public AstAttribute[] Attributes { get; set; } = [];

    /// <summary>
    /// Any children element (only applicable if <see cref="Type"/> is <see cref="AstElementType.Children"/>)
    /// </summary>
    public AstElement[] Children { get; set; } = [];

    /// <summary>
    /// The text content of the element (only applicable if <see cref="Type"/> is <see cref="AstElementType.Text"/>)
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Prints out the elements position in the template
    /// </summary>
    /// <returns>The elements position</returns>
    public string ExceptionString()
    {
        return $"Tag: {Tag}. Pos: {StreamPosition}. Line: {Line}. Col: {Column}.";
    }
}
