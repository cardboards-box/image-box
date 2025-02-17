namespace ImageBox.Elements;

/// <summary>
/// Represents a remote resource that should be cached
/// </summary>
/// <remarks>Not implemented yet</remarks>
[AstElement("remote-resource", ScopeType.CustomParent, typeof(ResourcesElem))]
public class RemoteResourceElem : Element
{
    /// <summary>
    /// The key to cache with for retrieval
    /// </summary>
    [AstAttribute("key")]
    public string? Key { get; set; }

    /// <summary>
    /// The remote source
    /// </summary>
    [AstAttribute("src"), AstAttribute("source"), AstAttribute("path")]
    public IOPath? Source { get; set; }

    /// <summary>
    /// The width to cache the value as (if it's an image)
    /// </summary>
    [AstAttribute("width")]
    public SizeUnit? Width { get; set; }

    /// <summary>
    /// The height to cache the value as (if it's an image)
    /// </summary>
    [AstAttribute("height")]
    public SizeUnit? Height { get; set; }
}
