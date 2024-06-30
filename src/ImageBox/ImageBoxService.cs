using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using Variables = System.Collections.Generic.Dictionary<string, object?>;

namespace ImageBox;

using Rendering.Base;
using Services;
using Services.Loading;

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
    /// Render an image from the image box
    /// </summary>
    /// <param name="box">The image to render</param>
    /// <param name="variables">The variables for the image template</param>
    /// <returns>The rendered image and whether it's a GIF (animated/true) or PNG (not animated/false)</returns>
    Task<(Image image, bool gif)> Render(IImageBox box, Variables? variables);

    /// <summary>
    /// Renders the given image box to a stream
    /// </summary>
    /// <param name="stream">The stream to save to</param>
    /// <param name="box">The image box to render</param>
    /// <param name="variables">The variables to use to render</param>
    /// <returns></returns>
    Task RenderToStream(Stream stream, IImageBox box, Variables? variables = null);

    /// <summary>
    /// Renders the given image box to a stream
    /// </summary>
    /// <param name="box">The image box to render</param>
    /// <param name="variables">The variables to use to render</param>
    /// <returns>The stream the image was rendered to</returns>
    Task<Stream> RenderToStream(IImageBox box, Variables? variables = null);

    /// <summary>
    /// Renders the given image box to a file
    /// </summary>
    /// <param name="path">The path to save to</param>
    /// <param name="box">The image box to render</param>
    /// <param name="variables">The variables to use to render</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the file path isn't local</exception>
    Task RenderToFile(IOPath path, IImageBox box, Variables? variables = null);

    /// <summary>
    /// Renders the given image box to a file
    /// </summary>
    /// <param name="path">The path to save to</param>
    /// <param name="box">The image box to render</param>
    /// <param name="variables">The variables to use to render</param>
    /// <returns></returns>
    Task RenderToFile(string path, IImageBox box, Variables? variables = null);
}

internal class ImageBoxService(
    ITemplateLoaderService _templates,
    IScriptExecutionService _execution,
    IContextGeneratorService _generator) : IImageBoxService
{
    /// <summary>
    /// Create an instance of an <see cref="IImageBox"/>
    /// </summary>
    /// <param name="path">The path to the file to file the image box</param>
    /// <returns>The image box instance</returns>
    public IImageBox Create(IOPath path)
    {
        if (path.Local && !path.Exists)
            throw new FileNotFoundException($"The file '{path}' does not exist");

        return new ImageBox { Path = path };
    }

    /// <summary>
    /// Create an instance of an <see cref="IImageBox"/>
    /// </summary>
    /// <param name="path">The path to the file to file the image box</param>
    /// <returns>The image box instance</returns>
    public IImageBox Create(string path) => Create(new IOPath(path));

    /// <summary>
    /// Get the (cached) data from the file path
    /// </summary>
    /// <param name="box">The box cache</param>
    /// <returns>The image data</returns>
    public async Task<BoxedImageData> LoadData(IImageBox box)
    {
        return box.Data ??= await _templates.Load(box.Path);
    }

    /// <summary>
    /// Get the (cached) render context from the image data
    /// </summary>
    /// <param name="box">The box cache</param>
    /// <returns>The render context</returns>
    public async Task<RenderContext> LoadContext(IImageBox box)
    {
        return box.Context ??= await _generator.Generate(await LoadData(box));
    }

    /// <summary>
    /// Render a single image from the context
    /// </summary>
    /// <param name="context">The context to render</param>
    /// <param name="variables">The variables to use for the root scope</param>
    /// <param name="frame">The current image frame (if it's a GIF, if not it's null)</param>
    /// <returns>The rendered image</returns>
    public async Task<Image> RenderSingle(RenderContext context, Variables variables, int? frame)
    {
        //Set the global scope of the context
        context.Frame = frame;
        //Force global scope to re-compute
        context.ClearGlobalScope();
        //Set the root scope of the context with the variables
        context.SetRootScope(variables);
        //Execute the script and bind the properties
        await _execution.Execute(context);
        //Create the image for the context
        var image = new Image<Rgba32>(context.Width, context.Height);
        //Set the context for the image
        context.Image = image;
        //Iterate through each element in the template
        foreach(var element in context.Template.Children)
        {
            //Element isn't a render-able element, skip it
            if (element is not RenderElement render) continue;
            //Render the element to the image
            await render.Render(context);
        }
        //Return the rendered image
        return image;
    }

    /// <summary>
    /// Render an image from the image box
    /// </summary>
    /// <param name="box">The image to render</param>
    /// <param name="variables">The variables for the image template</param>
    /// <returns>The rendered image and whether it's a GIF (animated/true) or PNG (not animated/false)</returns>
    public async Task<(Image image, bool gif)> Render(IImageBox box, Variables? variables)
    {
        variables ??= [];
        //Load the context from the cache
        var context = await LoadContext(box);
        //If it's not an animation, render a single frame and return
        if (!context.Animate) return (await RenderSingle(context, variables, null), false);
        //Create the gif image to apply mutation to
        var gif = new Image<Rgba32>(context.Width, context.Height);
        //Get the meta data and set the repeat count
        var meta = gif.Metadata.GetGifMetadata();
        meta.RepeatCount = context.FrameRepeat;
        //Get the root frame to set the frame delay
        var frame = gif.Frames.RootFrame.Metadata.GetGifMetadata();
        frame.FrameDelay = context.FrameDelay;
        //Render each frame and add it to the gif
        for(var i = 0; i < context.TotalFrames; i++)
        {
            //Get the frame
            using var image = await RenderSingle(context, variables, i + 1);
            //Set the frame delay
            frame = image.Frames.RootFrame.Metadata.GetGifMetadata();
            frame.FrameDelay = context.FrameDelay;
            //Add the frame to the gif
            gif.Frames.AddFrame(image.Frames.RootFrame);
        }
        //Return the gif
        return (gif, true);
    }

    /// <summary>
    /// Renders the given image box to a stream
    /// </summary>
    /// <param name="stream">The stream to save to</param>
    /// <param name="box">The image box to render</param>
    /// <param name="variables">The variables to use to render</param>
    /// <returns></returns>
    public async Task RenderToStream(Stream stream, IImageBox box, Variables? variables = null)
    {
        var (image, gif) = await Render(box, variables);
        if (gif) await image.SaveAsGifAsync(stream);
        else await image.SaveAsPngAsync(stream);
        image.Dispose();
    }

    /// <summary>
    /// Renders the given image box to a stream
    /// </summary>
    /// <param name="box">The image box to render</param>
    /// <param name="variables">The variables to use to render</param>
    /// <returns>The stream the image was rendered to</returns>
    public async Task<Stream> RenderToStream(IImageBox box, Variables? variables = null)
    {
        var ms = new MemoryStream();
        await RenderToStream(ms, box, variables);
        ms.Position = 0;
        return ms;
    }

    /// <summary>
    /// Renders the given image box to a file
    /// </summary>
    /// <param name="path">The path to save to</param>
    /// <param name="box">The image box to render</param>
    /// <param name="variables">The variables to use to render</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the file path isn't local</exception>
    public async Task RenderToFile(IOPath path, IImageBox box, Variables? variables = null)
    {
        if (!path.Local)
            throw new InvalidOperationException("Cannot save to a non-local path");

        using var stream = File.Create(path.OSSafe);
        await RenderToStream(stream, box, variables);
    }

    /// <summary>
    /// Renders the given image box to a file
    /// </summary>
    /// <param name="path">The path to save to</param>
    /// <param name="box">The image box to render</param>
    /// <param name="variables">The variables to use to render</param>
    /// <returns></returns>
    public Task RenderToFile(string path, IImageBox box, Variables? variables = null)
    {
        return RenderToFile(new IOPath(path), box, variables);
    }
}
