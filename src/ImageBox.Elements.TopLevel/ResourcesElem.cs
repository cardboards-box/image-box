namespace ImageBox.Elements.TopLevel;

/// <summary>
/// Element to allow for caching resources to use across multiple renders
/// </summary>
[AstElement("cache", ScopeType.TopLevel), AstElement("resources", ScopeType.TopLevel)]
public class ResourcesElem : Element, IParentElement
{
    private FontFamilyElem[]? _fonts;
    private RemoteResourceElem[]? _remotes;

    /// <summary>
    /// All of the resources to cache
    /// </summary>
    public IElement[] Children { get; set; } = [];

    /// <summary>
    /// All of the font elements to cache
    /// </summary>
    public FontFamilyElem[] Fonts => _fonts ??= Children.OfType<FontFamilyElem>().ToArray();

    /// <summary>
    /// All of the remote resources to cache
    /// </summary>
    public RemoteResourceElem[] Remotes => _remotes ??= Children.OfType<RemoteResourceElem>().ToArray();
}
