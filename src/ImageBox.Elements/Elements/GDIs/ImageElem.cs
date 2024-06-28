namespace ImageBox.Elements.Elements.GDIs;

/// <summary>
/// Represents an image that can be drawn to the card
/// </summary>
[AstElement("image")]
public class ImageElem : PositionalGDIElement
{
    /// <summary>
    /// The images source
    /// </summary>
    [AstAttribute("src"), AstAttribute("source")]
    public AstValue<IOPath> Source { get; set; } = new();

    /// <summary>
    /// The position of the image within the bounds of the rectangle
    /// </summary>
    [AstAttribute("position")]
    public AstValue<string> Position { get; set; } = new();
}