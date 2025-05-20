namespace ImageBox.Elements.TopLevel;

/// <summary>
/// Represents a script that can be executed to calculate contexts
/// </summary>
[AstElement("script", ScopeType.TopLevel)]
public class ScriptElem : Element, IValueElement
{
    /// <summary>
    /// Whether or not the script is the entry point to the image
    /// </summary>
    [AstAttribute("setup")]
    public bool Setup { get; set; }

    /// <summary>
    /// Whether or not the script should be used for template initialization
    /// </summary>
    [AstAttribute("init")]
    public bool Init { get; set; }

    /// <summary>
    /// The name of the module to use when injecting into other scripts
    /// </summary>
    [AstAttribute("name"), AstAttribute("module")]
    public string? Module { get; set; }

    /// <summary>
    /// Where to look to populate this script
    /// </summary>
    [AstAttribute("src"), AstAttribute("source"), AstAttribute("path")]
    public IOPath? Source { get; set; }

    /// <summary>
    /// The script body
    /// </summary>
    public string? Value { get; set; }
}