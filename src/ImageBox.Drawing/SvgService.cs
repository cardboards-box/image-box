using Svg;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageBox.Drawing;

/// <summary>
/// Service for interfacing with SVG files
/// </summary>
public interface ISvgService
{
    /// <summary>
    /// Gets the image from the given SVG stream
    /// </summary>
    /// <param name="svg">The SVG document</param>
    /// <param name="options">The render options</param>
    /// <returns>The bitmap of the SVG</returns>
    Bitmap GetBitmap(Stream svg, RenderOptions? options = null);

    /// <summary>
    /// Gets the image from the given file path
    /// </summary>
    /// <param name="path">The path to get the SVG document from</param>
    /// <param name="options">The options to render with</param>
    /// <returns>The bitmap from of the SVG</returns>
    Task<Bitmap> GetBitmap(IOPath path, RenderOptions? options = null);

    /// <summary>
    /// Gets the image stream from the given SVG stream
    /// </summary>
    /// <param name="svg">The SVG document</param>
    /// <param name="options">The render options</param>
    /// <returns>The bitmap as a stream</returns>
    Stream GetStream(Stream svg, RenderOptions? options = null);

    /// <summary>
    /// Gets the image stream from the given file path
    /// </summary>
    /// <param name="path">The path to get the SVG document from</param>
    /// <param name="options">The options to render with</param>
    /// <returns>The bitmap as a stream</returns>
    Task<Stream> GetStream(IOPath path, RenderOptions? options = null);

    /// <summary>
    /// Create and save a bitmap from the given SVG stream
    /// </summary>
    /// <param name="output">The stream to write the image to</param>
    /// <param name="svg">The SVG document</param>
    /// <param name="options">The render options</param>
    /// <returns></returns>
    Task SaveBitmap(Stream output, Stream svg, RenderOptions? options = null);

    /// <summary>
    /// Create and save a bitmap from the given file path
    /// </summary>
    /// <param name="output">The stream to write the image to</param>
    /// <param name="path">The path to get the SVG document from</param>
    /// <param name="options">The options to render with</param>
    /// <returns></returns>
    Task SaveBitmap(Stream output, IOPath path, RenderOptions? options = null);
}

internal class SvgService(
    IFileResolverService _resolver) : ISvgService
{
    public static readonly ImageFormat _defaultFormat = ImageFormat.Png;

    /// <summary>
    /// Gets the image from the given file path
    /// </summary>
    /// <param name="path">The path to get the SVG document from</param>
    /// <param name="options">The options to render with</param>
    /// <returns>The bitmap from of the SVG</returns>
    public async Task<Bitmap> GetBitmap(IOPath path, RenderOptions? options = null)
    {
        var (stream, _, _) = await _resolver.Fetch(path);
        var svg = OpenSvg(stream);
        await stream.DisposeAsync();
        return DrawSvg(svg, options);
    }

    /// <summary>
    /// Gets the image from the given SVG stream
    /// </summary>
    /// <param name="svg">The SVG document</param>
    /// <param name="options">The render options</param>
    /// <returns>The bitmap of the SVG</returns>
    public Bitmap GetBitmap(Stream svg, RenderOptions? options = null)
    {
        var input = OpenSvg(svg);
        return DrawSvg(input, options);
    }

    /// <summary>
    /// Gets the image stream from the given file path
    /// </summary>
    /// <param name="path">The path to get the SVG document from</param>
    /// <param name="options">The options to render with</param>
    /// <returns>The bitmap as a stream</returns>
    public async Task<Stream> GetStream(IOPath path, RenderOptions? options = null)
    {
        using var bitmap = await GetBitmap(path, options);
        return ToStream(bitmap, options);
    }

    /// <summary>
    /// Gets the image stream from the given SVG stream
    /// </summary>
    /// <param name="svg">The SVG document</param>
    /// <param name="options">The render options</param>
    /// <returns>The bitmap as a stream</returns>
    public Stream GetStream(Stream svg, RenderOptions? options = null)
    {
        using var bitmap = GetBitmap(svg, options);
        return ToStream(bitmap, options);
    }

    /// <summary>
    /// Create and save a bitmap from the given file path
    /// </summary>
    /// <param name="output">The stream to write the image to</param>
    /// <param name="path">The path to get the SVG document from</param>
    /// <param name="options">The options to render with</param>
    /// <returns></returns>
    public async Task SaveBitmap(Stream output, IOPath path, RenderOptions? options = null)
    {
        var format = options?.Format ?? _defaultFormat;
        using var bitmap = await GetBitmap(path, options);
        bitmap.Save(output, format);
        await output.FlushAsync();
    }

    /// <summary>
    /// Create and save a bitmap from the given SVG stream
    /// </summary>
    /// <param name="output">The stream to write the image to</param>
    /// <param name="svg">The SVG document</param>
    /// <param name="options">The render options</param>
    /// <returns></returns>
    public async Task SaveBitmap(Stream output, Stream svg, RenderOptions? options = null)
    {
        var format = options?.Format ?? _defaultFormat;
        using var bitmap = GetBitmap(svg, options);
        bitmap.Save(output, format);
        await output.FlushAsync();
    }

    /// <summary>
    /// Converts the given bitmap to a stream with the given render options
    /// </summary>
    /// <param name="map">The bitmap to turn into a stream</param>
    /// <param name="options">The options to save the bitmap with</param>
    /// <returns>The stream representation of the bitmap</returns>
    public static Stream ToStream(Bitmap map, RenderOptions? options)
    {
        var format = options?.Format ?? _defaultFormat;
        var output = new MemoryStream();
        map.Save(output, format);
        output.Position = 0;
        return output;
    }

    /// <summary>
    /// Gets the SVG document from the stream
    /// </summary>
    /// <param name="svg">The contents of the SVG file</param>
    /// <returns>The SVG document from the stream</returns>
    public static SvgDocument OpenSvg(Stream svg) => SvgDocument.Open<SvgDocument>(svg);

    /// <summary>
    /// Draw the SVG document to a bitmap with the given options
    /// </summary>
    /// <param name="input">The SVG document to render</param>
    /// <param name="options">The options to render with</param>
    /// <returns>The SVG drawn to a bitmap</returns>
    public static Bitmap DrawSvg(SvgDocument input, RenderOptions? options)
    {
        if (options?.Dpi is not null)
            input.Ppi = options.Dpi.Value;

        return
            options?.Width is not null && options?.Height is not null
            ? input.Draw(options.Width.Value, options.Height.Value)
            : input.Draw();
    }
}

/// <summary>
/// Options for rendering bitmaps
/// </summary>
/// <param name="Format">The format to save the image as</param>
/// <param name="Width">The width of the output raster image</param>
/// <param name="Height">The height of the output raster image</param>
/// <param name="Dpi">The DPI to use for the raster image</param>
public record class RenderOptions(
    ImageFormat? Format = null,
    int? Width = null,
    int? Height = null,
    int? Dpi = null);