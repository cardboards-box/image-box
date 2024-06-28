namespace ImageBox.Elements.Elements.Other;

/// <summary>
/// Represents a script that can be executed to calculate contexts
/// </summary>
[AstElement("script")]
public class ScriptElem : Element, IValueElement
{
    /// <summary>
    /// Whether or not the script is the entry point to the card
    /// </summary>
    [AstAttribute("setup")]
    public bool Setup { get; set; }

    /// <summary>
    /// The name of the module to use when injecting into other scripts
    /// </summary>
    [AstAttribute("module"), AstAttribute("name")]
    public string? Module { get; set; }

    /// <summary>
    /// Where to look to populate this script
    /// </summary>
    [AstAttribute("source"), AstAttribute("src"), AstAttribute("path")]
    public IOPath? Source { get; set; }

    /// <summary>
    /// The script body
    /// </summary>
    public string? Value { get; set; }
}