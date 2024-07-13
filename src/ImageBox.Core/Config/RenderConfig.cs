namespace ImageBox.Core.Config;

using IOPath;

/// <summary>
/// The configuration for rendering images
/// </summary>
public class RenderConfig
{
    /// <summary>
    /// Configuration option bind for <see cref="FontSizeUnit"/>
    /// </summary>
    public string FontSize
    {
        get => FontSizeUnit;
        set => FontSizeUnit = value;
    }

    /// <summary>
    /// The default font size for text rendering
    /// </summary>
    /// <value>Default value is 16px, default config path is `ImageBox:Render:FontSizeUnit`.</value>
    public SizeUnit FontSizeUnit { get; set; } = "16px";

    /// <summary>
    /// The default font family for text rendering
    /// </summary>
    /// <value>No default value, default config path is `ImageBox:Render:FontFamily`.</value>
    public string? FontFamily { get; set; }

    /// <summary>
    /// Configuration option bind for <see cref="FontFamilySrc"/>
    /// </summary>
    public string? FontFamilySrc
    {
        get => FontFamilySrcValue;
        set => FontFamilySrcValue = !string.IsNullOrEmpty(value) ? new IOPath(value) : null!;
    }

    /// <summary>
    /// The default font family source for text rendering
    /// </summary>
    public IOPath? FontFamilySrcValue { get; set; }

    /// <summary>
    /// Configuration option bind for <see cref="WidthUnit"/>
    /// </summary>
    public string Width
    {
        get => WidthUnit;
        set => WidthUnit = value;
    }

    /// <summary>
    /// The default width of the image
    /// </summary>
    /// <value>Default value is 500px, default config path is `ImageBox:Render:WidthUnit`.</value>
    public SizeUnit WidthUnit { get; set; } = "500px";

    /// <summary>
    /// Configuration option bind for <see cref="HeightUnit"/>
    /// </summary>
    public string Height
    {
        get => HeightUnit;
        set => HeightUnit = value;
    }

    /// <summary>
    /// The default height of the image
    /// </summary>
    /// <value>Default value is 500px, default config path is `ImageBox:Render:HeightUnit`.</value>
    public SizeUnit HeightUnit { get; set; } = "500px";

    /// <summary>
    /// Whether or not to animate images by default
    /// </summary>
    /// <value>Default value is false, default config path is `ImageBox:Render:Animate`.</value>
    public bool Animate { get; set; } = false;

    /// <summary>
    /// The default animation duration of the image
    /// </summary>
    /// <value>Default value is 3.5 seconds, default config path is `ImageBox:Render:AnimateDuration`.</value>
    public string AnimateDuration 
    { 
        get => AnimateDurationUnit; 
        set => AnimateDurationUnit = value; 
    }

    /// <summary>
    /// The default duration to animate images
    /// </summary>
    public TimeUnit AnimateDurationUnit { get; set; } = "3.5s";

    /// <summary>
    /// The default FPS for animated images
    /// </summary>
    /// <value>Default value is 15 fps, default config path is `ImageBox:Render:AnimateFps`.</value>
    public double AnimateFps { get; set; } = 15;

    /// <summary>
    /// The default number of times to repeat the animation (0 is repeat forever)
    /// </summary>
    /// <value>Default value is 0, default config path is `ImageBox:Render:AnimateRepeat`.</value>
    public ushort AnimateRepeat { get; set; } = 0;

    /// <summary>
    /// The max number of image frames to render at once
    /// </summary>
    /// <value>Default value is 5, default config path is `ImageBox:Render:AnimateParallelism`.</value>
    public ushort AnimateParallelism { get; set; } = 5;
}
