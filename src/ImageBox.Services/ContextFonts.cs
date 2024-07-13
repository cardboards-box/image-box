using SixLabors.Fonts;
using System.Collections.Concurrent;

namespace ImageBox.Services;

/// <summary>
/// Represents a collection of fonts for a rendering context
/// </summary>
public class ContextFonts
{
    /// <summary>
    /// The font collection for the context
    /// </summary>
    public FontCollection Collection { get; set; } = new();

    /// <summary>
    /// All of the font families in the collection
    /// </summary>
    public ConcurrentDictionary<string, LoadedFont> Families { get; } = [];

    /// <summary>
    /// Get the font family with the specified name
    /// </summary>
    /// <param name="name">The name of the font family</param>
    /// <param name="scope">The scope requesting the font</param>
    /// <param name="style">The style of the font</param>
    /// <returns>The loaded font</returns>
    /// <exception cref="RenderContextException">Thrown if the font doesn't exist</exception>
    public Font GetFont(string name, ContextScope scope, FontStyle style = FontStyle.Regular)
    {
        if (!Families.TryGetValue(name, out var font))
            throw new RenderContextException($"Font family '{name}' not found in context", scope.AstElement);

        return font.Get(scope.Size.FontSize, style);
    }
}

/// <summary>
/// Represents a loaded font
/// </summary>
public class LoadedFont
{
    /// <summary>
    /// The font family for the font
    /// </summary>
    public required FontFamily Family { get; init; }

    /// <summary>
    /// The element that the font was loaded from
    /// </summary>
    public required FontFamilyElem Element { get; init; }

    /// <summary>
    /// The name of the font family
    /// </summary>
    public string Name => Element.Name ?? Family.Name;

    /// <summary>
    /// Gets an instance of the font with the specified size and style
    /// </summary>
    /// <param name="size">The size of the font</param>
    /// <param name="style">The style of the font</param>
    /// <returns>The font</returns>
    public Font Get(int size, FontStyle style = FontStyle.Regular) => Family.CreateFont(size, style);
}