using IFontStyle = SixLabors.Fonts.FontStyle;

namespace ImageBox.Elements;

/// <summary>
/// Represents properties 
/// </summary>
public interface IFontElement : IPositionElement
{
    /// <summary>
    /// The font size
    /// </summary>
    AstValue<SizeUnit?> FontSize { get; }

    /// <summary>
    /// The font family to use for the text
    /// </summary>
    [AstAttribute("font-family")]
    AstValue<string?> FontFamily { get; }

    /// <summary>
    /// The style of the font to use
    /// </summary>
    [AstAttribute("font-style", typeof(IFontStyle))]
    AstValue<string?> FontStyle { get; }

    /// <summary>
    /// Whether to automatically determine the font size based on the box size
    /// </summary>
    /// <remarks>
    /// If true, <see cref="FontSize"/> will be ignored. 
    /// This can be relatively expensive, so it should be avoided when possible.
    /// </remarks>
    [AstAttribute("auto-font-size")]
    AstValue<bool?> AutoFontSize { get; }

    /// <summary>
    /// Sets the padding to use when determining the font size
    /// </summary>
    /// <remarks>Only used when <see cref="AutoFontSize"/> is true</remarks>
    AstValue<SizeUnit?> AutoFontSizePadding { get; }

    /// <summary>
    /// Sets the word breaking to use when determining the font size
    /// </summary>
    AstValue<string?> WordBreaking { get; }
}
