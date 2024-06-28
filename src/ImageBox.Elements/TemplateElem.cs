namespace ImageBox.Elements;

/// <summary>
/// Represents the root of a template
/// </summary>
[AstElement("template")]
public class TemplateElem : Element, IParentElement
{
    /// <summary>
    /// The width of the image
    /// </summary>
    [AstAttribute("width")]
    public SizeUnit? Width { get; set; }

    /// <summary>
    /// The height of the image
    /// </summary>
    [AstAttribute("height")]
    public SizeUnit? Height { get; set; }

    /// <summary>
    /// The default size of the font for the image
    /// </summary>
    [AstAttribute("font-size")]
    public SizeUnit? FontSize { get; set; }

    /// <summary>
    /// The default font family to use for the image
    /// </summary>
    [AstAttribute("font-family")]
    public string? FontFamily { get; set; }

    /// <summary>
    /// Whether or not to animate the image
    /// </summary>
    [AstAttribute("animate")]
    public bool Animate { get; set; } = false;

    /// <summary>
    /// The duration to animate the image
    /// </summary>
    [AstAttribute("animate-duration")]
    public TimeUnit? AnimateDuration { get; set; }

    /// <summary>
    /// The number of frames per second for the animation
    /// </summary>
    [AstAttribute("animate-fps")]
    public double? AnimateFps { get; set; }

    /// <summary>
    /// The children elements of the directive
    /// </summary>
    public IElement[] Children { get; set; } = [];
}