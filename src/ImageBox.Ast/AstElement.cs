namespace ImageBox.Ast;

/// <summary>
/// Represents an element in the custom XML AST for image templates
/// </summary>
/// <param name="StreamPosition">The position in the stream where this element occurs</param>
/// <param name="Line">The line number this element occurs on</param>
/// <param name="Column">The column number on the <see cref="Line"/> this element occurs on</param>
/// <param name="Tag">The tag of the element</param>
/// <param name="Type">Indicates the type of children on this element</param>
/// <param name="Attributes">Any attributes present on the element</param>
/// <param name="Children">Any children element (only applicable if <see cref="Type"/> is <see cref="AstElementType.Children"/>)</param>
/// <param name="Value">The text content of the element (only applicable if <see cref="Type"/> is <see cref="AstElementType.Text"/>)</param>
public record class AstElement(
    int StreamPosition,
    int Line,
    int Column,
    string Tag,
    AstElementType Type,
    AstAttribute[] Attributes,
    AstElement[] Children,
    string? Value)
{
    /// <summary>
    /// Prints out the elements position in the template
    /// </summary>
    /// <returns>The elements position</returns>
    public string ExceptionString()
    {
        return $"Tag: {Tag}. Pos: {StreamPosition}. Line: {Line}. Col: {Column}.";
    }
};