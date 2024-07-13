using SixLabors.ImageSharp;

namespace ImageBox;

using Services;
using Services.Loading;
using System.Diagnostics;
using Configure = Action<IImageRendererEvents>;
using Variables = Dictionary<string, object?>;

/// <summary>
/// Service for rendering image boxes
/// </summary>
public interface IImageBoxService
{
    /// <summary>
    /// Create an instance of an <see cref="IImageBox"/>
    /// </summary>
    /// <param name="path">The path to the file to file the image box</param>
    /// <returns>The image box instance</returns>
    IImageBox Create(string path);

    /// <summary>
    /// Create an instance of an <see cref="IImageBox"/>
    /// </summary>
    /// <param name="path">The path to the file to file the image box</param>
    /// <returns>The image box instance</returns>
    IImageBox Create(IOPath path);

    /// <summary>
    /// Get the (cached) data from the file path
    /// </summary>
    /// <param name="box">The box cache</param>
    /// <returns>The image data</returns>
    Task<LoadedAst> LoadData(IImageBox box);

    /// <summary>
    /// Get the (cached) render context from the image data
    /// </summary>
    /// <param name="box">The box cache</param>
    /// <returns>The render context</returns>
    Task<ContextBox> LoadContext(IImageBox box);

    /// <summary>
    /// Creates an image renderer
    /// </summary>
    /// <param name="box">The image to render</param>
    /// <param name="variables">The variables for the image template</param>
    /// <returns>The image renderer</returns>
    Task<IImageRenderer> CreateRender(IImageBox box, Variables variables);

    /// <summary>
    /// Render an image from the render instance
    /// </summary>
    /// <param name="box">The image renderer</param>
    /// <returns>The rendered image and whether it's a GIF (animated/true) or PNG (not animated/false)</returns>
    Task<(Image image, bool gif)> Render(IImageRenderer box);

    /// <summary>
    /// Render an image from the image box
    /// </summary>
    /// <param name="box">The image to render</param>
    /// <param name="variables">The variables for the image template</param>
    /// <param name="config">The configuration action for binding events</param>
    /// <returns>The rendered image and whether it's a GIF (animated/true) or PNG (not animated/false)</returns>
    Task<(Image image, bool gif)> Render(IImageBox box, Variables? variables = null, Configure? config = null);

    /// <summary>
    /// Renders the given image box to a stream
    /// </summary>
    /// <param name="stream">The stream to save to</param>
    /// <param name="box">The image box to render</param>
    /// <param name="variables">The variables to use to render</param>
    /// <param name="config">The configuration action for binding events</param>
    /// <returns></returns>
    Task RenderToStream(Stream stream, IImageBox box, Variables? variables = null, Configure? config = null);

    /// <summary>
    /// Renders the given image box to a stream
    /// </summary>
    /// <param name="box">The image box to render</param>
    /// <param name="variables">The variables to use to render</param>
    /// <param name="config">The configuration action for binding events</param>
    /// <returns>The stream the image was rendered to</returns>
    Task<Stream> RenderToStream(IImageBox box, Variables? variables = null, Configure? config = null);

    /// <summary>
    /// Renders the given image box to a file
    /// </summary>
    /// <param name="path">The path to save to</param>
    /// <param name="box">The image box to render</param>
    /// <param name="variables">The variables to use to render</param>
    /// <param name="config">The configuration action for binding events</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the file path isn't local</exception>
    Task RenderToFile(IOPath path, IImageBox box, Variables? variables = null, Configure? config = null);

    /// <summary>
    /// Renders the given image box to a file
    /// </summary>
    /// <param name="path">The path to save to</param>
    /// <param name="box">The image box to render</param>
    /// <param name="variables">The variables to use to render</param>
    /// <param name="config">The configuration action for binding events</param>
    /// <returns></returns>
    Task RenderToFile(string path, IImageBox box, Variables? variables = null, Configure? config = null);
}

internal class ImageBoxService(
    IAstLoaderService _templates,
    IScriptExecutionService _scripting,
    IElementReflectionService _elements,
    IServiceConfig _config,
    IContextGeneratorService _generator,
    ILogger<ImageBoxService> _logger) : IImageBoxService
{
    public IImageBox Create(IOPath path)
    {
        if (path.Local && !path.Exists)
            throw new FileNotFoundException($"The file '{path}' does not exist");

        return new ImageBox { Path = path };
    }

    public IImageBox Create(string path) => Create(new IOPath(path));

    public async Task<LoadedAst> LoadData(IImageBox box)
    {
        return box.Data ??= await _templates.Load(box.Path);
    }

    public async Task<ContextBox> LoadContext(IImageBox box)
    {
        return box.Context ??= await _generator.Generate(await LoadData(box));
    }

    public async Task<IImageRenderer> CreateRender(IImageBox box, Variables variables)
    {
        var ctx = await LoadContext(box);
        return new ImageRenderer(_scripting, _config, _elements, ctx, variables);
    }

    public async Task<(Image image, bool gif)> Render(IImageBox box, Variables? variables = null, Configure? config = null)
    {
        var watch = Stopwatch.StartNew();
        using var renderer = await CreateRender(box, variables ?? []);
        config?.Invoke(renderer);

        renderer.RenderStarted += (a) => _logger.LogInformation("Rendering image >> {image}", a.FileName);
        renderer.RenderFinished += (a) =>
        {
            watch.Stop();
            _logger.LogInformation("Rendered image in {Elapsed}ms >> {image}", watch.ElapsedMilliseconds, a.FileName);
        };
        renderer.FrameStarted += (p) => _logger.LogInformation(
            "[START] Rendering frame #{Frame} [{Rendered}/{Total} ({Progress:00.00}%)] ({Rendering} in-progress) >> {image}", 
            p.Frame, p.Rendered, p.Total, p.Progress, p.Rendering, p.Image.FileName);
        renderer.FrameFinished += (p) => _logger.LogInformation(
            "[END] Rendered frame #{Frame} [{Rendered}/{Total} ({Progress:00.00}%)] ({Rendering} in-progress) >> {image}",
            p.Frame, p.Rendered, p.Total, p.Progress, p.Rendering, p.Image.FileName);

        return await Render(renderer);
    }


    public async Task<(Image image, bool gif)> Render(IImageRenderer box)
    {
        return (await box.Render(), box.Box.Animate);
    }

    public async Task RenderToStream(Stream stream, IImageBox box, Variables? variables = null, Configure? config = null)
    {
        var (image, gif) = await Render(box, variables, config);
        if (gif) await image.SaveAsGifAsync(stream);
        else await image.SaveAsPngAsync(stream);
        image.Dispose();
    }

    public async Task<Stream> RenderToStream(IImageBox box, Variables? variables = null, Configure? config = null)
    {
        var ms = new MemoryStream();
        await RenderToStream(ms, box, variables, config);
        ms.Position = 0;
        return ms;
    }

    public async Task RenderToFile(IOPath path, IImageBox box, Variables? variables = null, Configure? config = null)
    {
        if (!path.Local)
            throw new InvalidOperationException("Cannot save to a non-local path");

        using var stream = File.Create(path.OSSafe);
        await RenderToStream(stream, box, variables, config);
    }

    public Task RenderToFile(string path, IImageBox box, Variables? variables = null, Configure? config = null)
    {
        return RenderToFile(new IOPath(path), box, variables, config);
    }
}