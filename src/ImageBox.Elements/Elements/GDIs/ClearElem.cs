namespace ImageBox.Elements.Elements.GDIs;

/// <summary>
/// The clear element
/// </summary>
[AstElement("clear")]
public class ClearElem : GDIElement
{
    /// <summary>
    /// The color to clear with
    /// </summary>
    [AstAttribute("color")]
    public AstValue<string> Color { get; set; } = new();
}
