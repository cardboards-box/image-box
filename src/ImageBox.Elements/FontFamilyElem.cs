namespace ImageBox.Elements;

/// <summary>
/// Element to allow for importing of custom fonts
/// </summary>
[AstElement("font-family", ScopeType.CustomParent, typeof(ResourcesElem))]
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
    [AstAttribute("src"), AstAttribute("source"), AstAttribute("path")]
    public IOPath? Source { get; set; }
}
