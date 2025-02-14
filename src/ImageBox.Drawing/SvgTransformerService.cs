using System.Drawing.Imaging;

namespace ImageBox.Drawing;

internal class SvgTransformerService(
    ISvgService _svg) : IFileTransformerService
{
    public Task<FileResult> Transform(FileResult result, FileFetchProperties properties)
    {
        if (result.MimeType != "image/svg+xml") return Task.FromResult(result);

        var svg = _svg.GetStream(result.Stream, new RenderOptions
        {
            Width = properties.Width,
            Height = properties.Height,
            Format = ImageFormat.Png,
        });

        var output = new FileResult(svg, result.FileName, "image/png", result.Cacheable);
        return Task.FromResult(output);
    }
}
