namespace ImageBox;

using Elements;

/// <summary>
/// Represents a render-able image box
/// </summary>
/// <remarks>You should cache this and reuse if possible, then dispose when you don't need it anymore</remarks>
public interface IImageBox
{
    /// <summary>
    /// The path to the image box file
    /// </summary>
    IOPath Path { get; }

    /// <summary>
    /// The data loaded from the image box file
    /// </summary>
    LoadedAst? Data { get; set; }

    /// <summary>
    /// The render box context for the image box
    /// </summary>
    ContextBox? Context { get; set;  }
}

/// <inheritdoc/>
public class ImageBox : IImageBox
{
    /// <inheritdoc/>
    public required IOPath Path { get; init; }

    /// <inheritdoc/>
    public LoadedAst? Data { get; set; }

    /// <inheritdoc/>
    public ContextBox? Context { get; set; }
}