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
    /// The settings for the template
    /// </summary>
    public required TemplateSettings Settings { get; init; }

    /// <summary>
    /// The script runner for the setup module in the template
    /// </summary>
    public required ScriptRunner? Runner { get; init; }

    /// <inheritdoc cref="TemplateSettings.Size" />
    public SizeContext Size => Settings.Size;

    /// <inheritdoc cref="TemplateSettings.TotalFrames" />
    public uint TotalFrames => Settings.TotalFrames;

    /// <inheritdoc cref="TemplateSettings.FrameDelay" />
    public uint FrameDelay => Settings.FrameDelay;

    /// <inheritdoc cref="TemplateSettings.FrameRepeat" />
    public ushort FrameRepeat => Settings.FrameRepeat;

    /// <inheritdoc cref="TemplateSettings.Animate" />
    public bool Animate => Settings.Animate;
}