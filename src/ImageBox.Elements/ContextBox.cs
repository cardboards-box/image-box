namespace ImageBox.Elements;

using Ast;

/// <summary>
/// Represents a context for rendering an entire image
/// </summary>
public class ContextBox : IScriptItem
{
    /// <summary>
    /// The entire abstract syntax tree for the template
    /// </summary>
    public required LoadedAst Ast { get; init; }

    /// <summary>
    /// The abstract syntax tree from the underlying template
    /// </summary>
    public AstElement Template => TemplateElement.Context!;

    /// <summary>
    /// The underlying template for the context
    /// </summary>
    public required IElement TemplateElement { get; init; }

    /// <summary>
    /// The elements in the template
    /// </summary>
    public IElement[] Elements => (TemplateElement as IParentElement)?.Children ?? [];

    /// <summary>
    /// The cached fonts for the image
    /// </summary>
    public required ContextFonts Fonts { get; init; }

    /// <summary>
    /// The size of the image
    /// </summary>
    public required SizeContext Size { get; init; }

    /// <summary>
    /// The script runner for the setup module in the template
    /// </summary>
    public required ScriptRunner? Runner { get; init; }

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