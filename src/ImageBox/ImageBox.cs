using ImageBox.Services;

namespace ImageBox;

/// <summary>
/// Represents a render-able image box
/// </summary>
/// <remarks>You should cache this and reuse if possible, then dispose when you don't need it anymore</remarks>
public interface IImageBox : IDisposable
{
    /// <summary>
    /// The path to the image box file
    /// </summary>
    IOPath Path { get; }

    /// <summary>
    /// The data loaded from the image box file
    /// </summary>
    BoxedImageData? Data { get; set; }

    /// <summary>
    /// The render box context for the image box
    /// </summary>
    RenderContext? Context { get; set;  }
}

/// <summary>
/// Represents an instance of an image box and its render context
/// </summary>
public class ImageBox : IImageBox
{
    /// <summary>
    /// The path to the image box file
    /// </summary>
    public required IOPath Path { get; init; }

    /// <summary>
    /// The data loaded from the image box file
    /// </summary>
    public BoxedImageData? Data { get; set; }

    /// <summary>
    /// The render box context for the image box
    /// </summary>
    public RenderContext? Context { get; set; }

    /// <summary>
    /// Dispose of the render context and data
    /// </summary>
    public void Dispose()
    {
        Context?.Dispose();
        Data = null;
        Context = null;
        GC.SuppressFinalize(this);
    }
}
