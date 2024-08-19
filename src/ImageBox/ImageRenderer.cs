using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;

using Variables = System.Collections.Generic.Dictionary<string, object?>;

namespace ImageBox;

using Rendering.Base;
using Services;
using Services.Loading;

/// <summary>
/// The event handlers for rendering an image
/// </summary>
public interface IImageRendererEvents
{
    /// <summary>
    /// The event indicating a frame has started rendering
    /// </summary>
    event RenderProgressHandler FrameStarted;
    /// <summary>
    /// The event indicating a frame has finished rendering
    /// </summary>
    event RenderProgressHandler FrameFinished;
    /// <summary>
    /// The event indicating the rendering process has started
    /// </summary>
    event RenderVoid RenderStarted;
    /// <summary>
    /// The event indicating the rendering process has finished
    /// </summary>
    event RenderVoid RenderFinished;

    /// <summary>
    /// Configure the renderer to save individual frames
    /// </summary>
    /// <param name="save">Whether or not to save individual frames (default: true)</param>
    /// <param name="path">Where to save the individual frames (default: Context's working directory)</param>
    /// <returns>The current renderer for chaining</returns>
    IImageRenderer SetSaveFrames(bool save = true, IOPath? path = null);

    /// <summary>
    /// Configure the process to cancel when the given token is cancelled
    /// </summary>
    /// <param name="token">The token to watch cancellations from</param>
    /// <returns>The current renderer for chaining</returns>
    IImageRenderer CancelWith(CancellationToken token);
}

/// <summary>
/// An instance of the image renderer for a <see cref="ContextBox"/>
/// </summary>
public interface IImageRenderer : IImageRendererEvents, IAsyncDisposable, IDisposable
{
    /// <summary>
    /// The token for cancelling all the frame rendering tasks
    /// </summary>
    CancellationToken Token { get; }

    /// <summary>
    /// Whether the renderer is currently running
    /// </summary>
    bool Rendering { get; }

    /// <summary>
    /// Whether to save individual frames
    /// </summary>
    bool SaveFrames { get; }

    /// <summary>
    /// How many frames have been rendered so far
    /// </summary>
    int RenderedCount { get; }

    /// <summary>
    /// The number of frames currently being rendered
    /// </summary>
    int RenderingCount { get; }

    /// <summary>
    /// The percentage of frames that have been rendered (0-100)
    /// </summary>
    double Progress { get; }

    /// <summary>
    /// Where to save the individual frames (if <see cref="SaveFrames"/> is true)
    /// </summary>
    IOPath? SaveFramesPath { get; }

    /// <summary>
    /// The context box being rendered
    /// </summary>
    ContextBox Box { get; }

    /// <summary>
    /// Render the image and return it
    /// </summary>
    /// <returns>The image to return</returns>
    Task<Image> Render();

    /// <summary>
    /// Requests that all of the renderers be cancelled
    /// </summary>
    Task Cancel();
}

internal class ImageRenderer(
    IScriptExecutionService _scripting,
    IServiceConfig _config,
    IElementReflectionService _elements,
    ContextBox _box,
    Variables _variables) : IImageRenderer
{
    private IOPath? _saveFramesPath = null;
    private string? _saveFramesDir = null;
    private readonly List<CancellationTokenSource> _sources = [];
    private CancellationTokenSource _tokenSource = new();

    /// <summary>
    /// The event indicating a frame has started rendering
    /// </summary>
    public event RenderProgressHandler FrameStarted = delegate { };
    /// <summary>
    /// The event indicating a frame has finished rendering
    /// </summary>
    public event RenderProgressHandler FrameFinished = delegate { };
    /// <summary>
    /// The event indicating the rendering process has started
    /// </summary>
    public event RenderVoid RenderStarted = delegate { };
    /// <summary>
    /// The event indicating the rendering process has finished
    /// </summary>
    public event RenderVoid RenderFinished = delegate { };

    /// <summary>
    /// All of the frames currently being rendered
    /// </summary>
    public ConcurrentDictionary<int, Image> RenderFrames { get; } = [];

    /// <summary>
    /// The token for cancelling all the frame rendering tasks
    /// </summary>
    public CancellationToken Token => _tokenSource.Token;

    /// <summary>
    /// Whether the renderer is currently running
    /// </summary>
    public bool Rendering { get; private set; } = false;

    /// <summary>
    /// Whether to save individual frames
    /// </summary>
    public bool SaveFrames { get; private set; } = false;

    /// <summary>
    /// How many frames have been rendered so far
    /// </summary>
    public int RenderedCount { get; private set; } = 0;

    /// <summary>
    /// The number of frames currently being rendered
    /// </summary>
    public int RenderingCount => RenderFrames.Count;

    /// <summary>
    /// The percentage of frames that have been rendered (0-100)
    /// </summary>
    public double Progress => RenderedCount / (double)_box.TotalFrames * 100;

    /// <summary>
    /// What directory to save the frames to
    /// </summary>
    public IOPath? SaveFramesPath => _saveFramesPath ?? _box.Ast.WorkingDirectory;

    /// <summary>
    /// The context box being rendered
    /// </summary>
    public ContextBox Box => _box;

    /// <summary>
    /// Get the directory to save the frames to
    /// </summary>
    public string SaveFrameDir()
    {
        if (_saveFramesPath is null) return _box.Ast.WorkingDirectory;

        return _saveFramesDir ??= _saveFramesPath.Value.GetAbsolute(_box.Ast.WorkingDirectory).OSSafe;
    }

    /// <summary>
    /// Configure the renderer to save individual frames
    /// </summary>
    /// <param name="save">Whether or not to save individual frames (default: true)</param>
    /// <param name="path">Where to save the individual frames (default: Context's working directory)</param>
    /// <returns>The current renderer for chaining</returns>
    public IImageRenderer SetSaveFrames(bool save = true, IOPath? path = null)
    {
        ThrowIfRendering();
        _saveFramesPath = path;
        _saveFramesDir = null;
        SaveFrames = save;
        return this;
    }

    /// <summary>
    /// Configure the process to cancel when the given token is cancelled
    /// </summary>
    /// <param name="token">The token to watch cancellations from</param>
    /// <returns>The current renderer for chaining</returns>
    public IImageRenderer CancelWith(CancellationToken token)
    {
        ThrowIfRendering();
        _sources.Add(_tokenSource);
        _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(_tokenSource.Token, token);
        return this;
    }

    /// <summary>
    /// Creates a progress record for the given frame
    /// </summary>
    /// <param name="frame">The frame for the progress record</param>
    /// <returns>The progress record</returns>
    public RenderProgress CurrentProgress(int frame)
    {
        return new RenderProgress(frame, RenderedCount, RenderingCount, (int)_box.TotalFrames, Progress, _box.Ast);
    }

    /// <summary>
    /// Renders a single frame of the image
    /// </summary>
    /// <param name="frameNum">The frame number</param>
    /// <param name="token">The cancellation token for cancelling the rendering</param>
    /// <param name="variables">The variables to render</param>
    /// <param name="image">The image to use for rendering (if is the first frame of the gif)</param>
    /// <returns>The image that was rendered</returns>
    public async Task<Image> RenderFrame(int frameNum, CancellationToken token, Variables variables, Image? image = null)
    {
        FrameStarted(CurrentProgress(frameNum));
        //Create the image if it doesn't exist
        image ??= new Image<Rgba32>(_box.Size.Width, _box.Size.Height);
        //Create the frame to render
        using var frame = new ContextFrame(frameNum, image, _box, variables, _scripting, Token)
        {
            //Get the elements to render
            Elements = _elements.BindTemplates(_box.Template.Children, false).ToArray()
        };
        //Execute and bind the script
        await _scripting.Execute(frame);
        //Render each element in the template
        foreach(var element in frame.Elements)
        {
            //If the token is cancelled, break out of the loop
            if (token.IsCancellationRequested) break;
            //If the element isn't a render element, skip it
            if (element is not RenderElement render) continue;
            //Render the element to the image
            await render.Render(frame);
        }
        RenderedCount++;
        FrameFinished(CurrentProgress(frameNum));
        //return the rendered image
        return image;
    }

    /// <summary>
    /// Attempt to append the latest frame to the gif
    /// </summary>
    /// <param name="gif">The gif to append to</param>
    public void AppendFrames(Image<Rgba32> gif)
    {
        //Always skip the first frame since it's already rendered
        var totalFrames = gif.Frames.Count + 1;
        if (!RenderFrames.TryGetValue(totalFrames, out Image? frame)) return;

        gif.Frames.AddFrame(frame.Frames.RootFrame);
        if (RenderFrames.TryRemove(totalFrames, out _))
            frame.Dispose();
        AppendFrames(gif);
    }

    /// <summary>
    /// Render the image and return it
    /// </summary>
    /// <returns>The image to return</returns>
    public async Task<Image> Render()
    {
        //Cannot render multiple times at once
        ThrowIfRendering();
        RenderStarted(_box.Ast);
        //Set the rendering flag
        Rendering = true;
        RenderedCount = 0;
        //If we're not animating, just render a single frame
        if (!_box.Animate)
        {
            var single = await RenderFrame(1, Token, _variables);
            Rendering = false;
            RenderFinished(_box.Ast);
            return single;
        }
        //Create the gif to be rendered
        var gif = new Image<Rgba32>(_box.Size.Width, _box.Size.Height);
        //Get the meta data and set the repeat count
        var meta = gif.Metadata.GetGifMetadata();
        meta.RepeatCount = _box.FrameRepeat;
        //Get the root frame to set the frame delay
        var frame = gif.Frames.RootFrame.Metadata.GetGifMetadata();
        frame.FrameDelay = 0;
        //Render the first frame of the gif
        await RenderFrame(1, Token, _variables, gif);
        //Get the frames to render
        var frameCounts = Enumerable.Range(2, (int)_box.TotalFrames - 1);
        //The options for parallel rendering
        var opts = new ParallelOptions
        {
            MaxDegreeOfParallelism = _config.Render.AnimateParallelism,
            CancellationToken = Token
        };
        //Lock object for adding frames to the gif
        var obj = new object();
        //Render the frames in parallel
        await Parallel.ForEachAsync(frameCounts, opts, async (frameNum, token) =>
        {
            //Render the frame
            var image = await RenderFrame(frameNum, token, _variables);
            //Set the meta data for the frame
            var nf = image.Frames.RootFrame.Metadata.GetGifMetadata();
            nf.FrameDelay = (int)_box.FrameDelay / 10;
            nf.DisposalMethod = GifDisposalMethod.RestoreToBackground;

            if (SaveFrames)
            {
                var dir = SaveFrameDir();
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, $"{frameNum}.png");
                await using var fs = File.Create(path);
                image.SaveAsPng(fs);
            }

            //Lock the gif to add the frame
            lock (obj)
            {
                RenderFrames.AddOrUpdate(frameNum, image, (_, _) => image);
                AppendFrames(gif);
            }
        });
        Rendering = false;
        RenderFinished(_box.Ast);
        return gif;
    }

    /// <summary>
    /// Throws an exception if the renderer is currently running
    /// </summary>
    /// <exception cref="RenderContextException">The exception thrown</exception>
    public void ThrowIfRendering()
    {
        if (!Rendering) return;

        throw new RenderContextException("The renderer is currently running", _box.Ast);
    }

    /// <summary>
    /// Requests that all of the renderers be cancelled
    /// </summary>
    public async Task Cancel()
    {
        if (!Rendering) return;

        await _tokenSource.CancelAsync();
        Rendering = false;
    }

    public void Dispose()
    {
        DisposeAsync().AsTask().Wait();
    }

    public async ValueTask DisposeAsync()
    {
        await Cancel();
        RenderFrames
            .Where(t => t.Key != 1)
            .Each(t => t.Value.Dispose());
        RenderFrames.Clear();
        _sources.ForEach(s => s.Dispose());
        _tokenSource.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// The handler for rendering progress
/// </summary>
/// <param name="progress">The progress of the renderer</param>
public delegate void RenderProgressHandler(RenderProgress progress);

/// <summary>
/// The handler for indicating various rendering events
/// </summary>
/// <param name="Image">The image event</param>
public delegate void RenderVoid(LoadedAst Image);

/// <summary>
/// Represents a progress report for rendering
/// </summary>
/// <param name="Frame">The frame number</param>
/// <param name="Rendered">The number of frames rendered</param>
/// <param name="Rendering">The number of frames currently rendering</param>
/// <param name="Total">The total number of frames to render</param>
/// <param name="Progress">The total progress percentage of the rendering</param>
/// <param name="Image">The image being rendered</param>
public record class RenderProgress(
    int Frame,
    int Rendered,
    int Rendering,
    int Total,
    double Progress,
    LoadedAst Image);