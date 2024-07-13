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

    public async Task<Bitmap> GetBitmap(IOPath path, RenderOptions? options = null)
    {
        var (stream, _, _) = await _resolver.Fetch(path);
        var svg = OpenSvg(stream);
        await stream.DisposeAsync();
        return DrawSvg(svg, options);
    }

    public Bitmap GetBitmap(Stream svg, RenderOptions? options = null)
    {
        var input = OpenSvg(svg);
        return DrawSvg(input, options);
    }

    public async Task<Stream> GetStream(IOPath path, RenderOptions? options = null)
    {
        using var bitmap = await GetBitmap(path, options);
        return ToStream(bitmap, options);
    }

    public Stream GetStream(Stream svg, RenderOptions? options = null)
    {
        using var bitmap = GetBitmap(svg, options);
        return ToStream(bitmap, options);
    }

    public async Task SaveBitmap(Stream output, IOPath path, RenderOptions? options = null)
    {
        var format = options?.Format ?? _defaultFormat;
        using var bitmap = await GetBitmap(path, options);
        bitmap.Save(output, format);
        await output.FlushAsync();
    }

    public async Task SaveBitmap(Stream output, Stream svg, RenderOptions? options = null)
    {
        var format = options?.Format ?? _defaultFormat;
        using var bitmap = GetBitmap(svg, options);
        bitmap.Save(output, format);
        await output.FlushAsync();
    }

    public static Stream ToStream(Bitmap map, RenderOptions? options)
    {
        var format = options?.Format ?? _defaultFormat;
        var output = new MemoryStream();
        map.Save(output, format);
        output.Position = 0;
        return output;
    }

    public static SvgDocument OpenSvg(Stream svg) => SvgDocument.Open<SvgDocument>(svg);

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