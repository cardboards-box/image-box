namespace ImageBox;

using Configure = Action<IImageRendererEvents>;
using Variables = Dictionary<string, object?>;

/// <summary>
/// A short-cut utility for rendering images and ignoring dependency injection
/// </summary>
public class ImageBoxUtility
{
    private readonly IImageBox _image;
    private readonly IImageBoxService _service;
    private Variables? _variables;
    private Configure? _configure;

    internal ImageBoxUtility(IImageBox image, IImageBoxService service)
    {
        _image = image;
        _service = service;
    }

    /// <summary>
    /// Sets the configuration method for the image renderer
    /// </summary>
    /// <param name="configure">The configuration method</param>
    /// <returns></returns>
    public ImageBoxUtility Configure(Configure configure)
    {
        _configure = configure;
        return this;
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
        return _service.RenderToFile(to, _image, _variables, _configure);
    }

    /// <summary>
    /// Renders the image to the given path
    /// </summary>
    /// <param name="to">The path to write to</param>
    /// <returns></returns>
    public Task Render(string to)
    {
        return _service.RenderToFile(to, _image, _variables, _configure);
    }

    /// <summary>
    /// Renders the image to the given stream
    /// </summary>
    /// <param name="to">The stream to write to</param>
    /// <returns></returns>
    public Task Render(Stream to)
    {
        return _service.RenderToStream(to, _image, _variables, _configure);
    }

    /// <summary>
    /// Renders the image to a stream and returns it
    /// </summary>
    /// <returns>The stream to write to</returns>
    public Task<Stream> Render()
    {
        return _service.RenderToStream(_image, _variables, _configure);
    }

    /// <summary>
    /// Creates an image utility from the given path
    /// </summary>
    /// <param name="path">The path to read the template from</param>
    /// <param name="config">The configuration for rendering</param>
    /// <returns>The image utility scoped to the current image</returns>
    public static ImageBoxUtility From(IOPath path, ServiceConfig? config = null)
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
    public static ImageBoxUtility From(string path, ServiceConfig? config = null)
    {
        return From(new IOPath(path), config);
    }

    internal static IImageBoxService GetService(ServiceConfig config)
    {
        return new ServiceCollection()
            .AddImageBox(config)
            .BuildServiceProvider()
            .GetRequiredService<IImageBoxService>();
    }
}
