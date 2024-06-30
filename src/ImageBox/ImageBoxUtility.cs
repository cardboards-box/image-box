using Variables = System.Collections.Generic.Dictionary<string, object?>;

namespace ImageBox;

/// <summary>
/// A short-cut utility for rendering images and ignoring dependency injection
/// </summary>
public class ImageBoxUtility : IDisposable
{
    private readonly IImageBox _image;
    private readonly IImageBoxService _service;
    private Variables? _variables;

    internal ImageBoxUtility(IImageBox image, IImageBoxService service)
    {
        _image = image;
        _service = service;
    }

    /// <summary>
    /// Set the root-scope variables of the template
    /// </summary>
    /// <param name="vars"></param>
    /// <returns></returns>
    public ImageBoxUtility SetVars(Variables vars)
    {
        _variables = vars;
        return this;
    }

    /// <summary>
    /// Clear the root-scope variables of the template
    /// </summary>
    /// <returns></returns>
    public ImageBoxUtility ClearVars()
    {
        _variables = null;
        return this;
    }

    /// <summary>
    /// Renders the image to the given path
    /// </summary>
    /// <param name="to">The path to write to</param>
    /// <returns></returns>
    public Task Render(IOPath to)
    {
        return _service.RenderToFile(to, _image, _variables);
    }

    /// <summary>
    /// Renders the image to the given path
    /// </summary>
    /// <param name="to">The path to write to</param>
    /// <returns></returns>
    public Task Render(string to)
    {
        return _service.RenderToFile(to, _image, _variables);
    }

    /// <summary>
    /// Renders the image to the given stream
    /// </summary>
    /// <param name="to">The stream to write to</param>
    /// <returns></returns>
    public Task Render(Stream to)
    {
        return _service.RenderToStream(to, _image, _variables);
    }

    /// <summary>
    /// Renders the image to a stream and returns it
    /// </summary>
    /// <returns>The stream to write to</returns>
    public Task<Stream> Render()
    {
        return _service.RenderToStream(_image, _variables);
    }

    /// <summary>
    /// Disposes of the image utility and the underlying image
    /// </summary>
    public void Dispose()
    {
        _image.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Creates an image utility from the given path
    /// </summary>
    /// <param name="path">The path to read the template from</param>
    /// <param name="config">The configuration for rendering</param>
    /// <returns>The image utility scoped to the current image</returns>
    public static ImageBoxUtility From(IOPath path, ImageBoxConfig? config = null)
    {
        var service = GetService(config ?? new());
        var box = service.Create(path);
        return new ImageBoxUtility(box, service);
    }

    /// <summary>
    /// Creates an image utility from the given path
    /// </summary>
    /// <param name="path">The path to read the template from</param>
    /// <param name="config">The configuration for rendering</param>
    /// <returns>The image utility scoped to the current image</returns>
    public static ImageBoxUtility From(string path, ImageBoxConfig? config = null)
    {
        return From(new IOPath(path), config);
    }

    internal static IImageBoxService GetService(ImageBoxConfig config)
    {
        return new ServiceCollection()
            .AddImageBox(config)
            .BuildServiceProvider()
            .GetRequiredService<IImageBoxService>();
    }
}
