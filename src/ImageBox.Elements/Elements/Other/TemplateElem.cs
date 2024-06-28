namespace ImageBox.Elements.Elements.Other;

/// <summary>
/// Represents the root of a template
/// </summary>
[AstElement("template")]
public class TemplateElem : DirectiveElement
{
    /// <summary>
    /// The width of the card
    /// </summary>
    [AstAttribute("width")]
    public SizeUnit? Width { get; set; }

    /// <summary>
    /// The height of the card
    /// </summary>
    [AstAttribute("height")]
    public SizeUnit? Height { get; set; }

    /// <summary>
    /// The default size of the font for the card
    /// </summary>
    [AstAttribute("font-size")]
    public SizeUnit? FontSize { get; set; }

    /// <summary>
    /// The default font family to use for the card
    /// </summary>
    [AstAttribute("font-family")]
    public string? FontFamily { get; set; }

    /// <summary>
    /// Whether or not to animate the card
    /// </summary>
    [AstAttribute("animate")]
    public bool Animate { get; set; } = false;

    /// <summary>
    /// The duration to animate the card
    /// </summary>
    [AstAttribute("animate-duration")]
    public TimeUnit? AnimateDuration { get; set; }

    /// <summary>
    /// The number of frames per second for the animation
    /// </summary>
    [AstAttribute("animate-fps")]
    public double AnimateFps { get; set; } = 15;
}