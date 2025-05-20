namespace ImageBox.Elements;

/// <summary>
/// The settings for the template
/// </summary>
public class TemplateSettings
{
    /// <summary>
    /// The size context of the image
    /// </summary>
    public required SizeContext Size { get; init; }

    /// <summary>
    /// The total number of frames in the image
    /// </summary>
    public uint TotalFrames { get; set; } = 1;

    /// <summary>
    /// The delay between frames in milliseconds
    /// </summary>
    public uint FrameDelay { get; set; } = 100;

    /// <summary>
    /// How many times to repeat the gif
    /// </summary>
    /// <remarks>0 is repeat forever, x is repeat number of times</remarks>
    public ushort FrameRepeat { get; set; } = 0;

    /// <summary>
    /// Whether or not animation is enabled
    /// </summary>
    public bool Animate => TotalFrames > 1;
}
