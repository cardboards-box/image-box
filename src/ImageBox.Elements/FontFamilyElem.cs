namespace ImageBox.Elements;

/// <summary>
/// Element to allow for importing of custom fonts
/// </summary>
[AstElement("font-family")]
public class FontFamilyElem : Element
{
    /// <summary>
    /// The name of the font family
    /// </summary>
    [AstAttribute("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Where to find the font file
    /// </summary>
    [AstAttribute("source"), AstAttribute("src"), AstAttribute("path")]
    public IOPath? Source { get; set; }
}
