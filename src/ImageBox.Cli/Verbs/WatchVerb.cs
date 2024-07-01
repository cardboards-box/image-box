
using ImageBox.Drawing;

namespace ImageBox.Cli.Verbs;

[Verb("watch", HelpText = "Watches a file for changes and regenerates the image")]
public class WatchVerbOptions
{
    [Option('p', "path", Required = true, HelpText = "The path to the template file")]
    public string? Path { get; set; }

    [Option('d', "directory", HelpText = "The directory to save the image to")]
    public string? Dir { get; set; }

    [Option('f', "filename", HelpText = "The name of the file to save as (without extension)")]
    public string? FileName { get; set; }
}

internal class WatchVerb(
    ILogger<WatchVerb> logger,
    IImageBoxService _image) : BooleanVerb<WatchVerbOptions>(logger)
{
    public async Task<string?> GenerateOutputPath(WatchVerbOptions options, IImageBox ib)
    {
        var ctx = await _image.LoadContext(ib);
        var path = Path.GetFullPath(options.Path!);

        var filename = options.FileName ?? Path.GetFileNameWithoutExtension(path);
        var ext = ctx.Animate ? "gif" : "png";
        var dir = options.Dir ?? Path.GetDirectoryName(path)!;

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        return Path.Combine(dir, $"{filename}.{ext}");
    }

    public async Task<bool> Render(WatchVerbOptions options)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(options.Path))
            {
                _logger.LogError("No path was provided");
                return false;
            }

            using var ib = _image.Create(options.Path);
            var output = await GenerateOutputPath(options, ib);
            if (string.IsNullOrEmpty(output))
            {
                _logger.LogError("Couldn't determine the output directory");
                return false;
            }

            await _image.RenderToFile(output, ib);
            _logger.LogInformation("Generated image at {Output}", output);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate image");
            return false;
        }
    }

    public override async Task<bool> Execute(WatchVerbOptions options, CancellationToken token)
    {
        var run = () => Render(options).Wait(token);

        if (string.IsNullOrWhiteSpace(options.Path))
        {
            _logger.LogError("No path was provided");
            return false;
        }

        var fullPath = Path.GetFullPath(options.Path);
        var dir = Path.GetDirectoryName(fullPath)!;
        var file = Path.GetFileName(fullPath);
        var bouncy = run.Debounce(300);

        using var watcher = new FileSystemWatcher();
        watcher.Changed += (sender, e) =>
        {
            _logger.LogInformation("File {File} changed, regenerating image", e.FullPath);
            bouncy();
        };
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Path = dir;
        watcher.Filter = file;
        watcher.EnableRaisingEvents = true;

        await Task.Delay(-1, token);
        return true;
    }
}
