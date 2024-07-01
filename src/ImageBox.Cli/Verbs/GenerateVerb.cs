namespace ImageBox.Cli.Verbs;

[Verb("generate", isDefault: true, HelpText = "Generates an image from a template")]
public class GenerateVerbOptions
{
    [Option('p', "path", Required = true, HelpText = "The path to the template file")]
    public string? Path { get; set; }

    [Option('d', "directory", HelpText = "The directory to save the image to")]
    public string? Dir { get; set; }

    [Option('f', "filename", HelpText = "The name of the file to save as (without extension)")]
    public string? FileName { get; set; }
}

public class GenerateVerb(
    ILogger<GenerateVerb> logger,
    IImageBoxService _image) : BooleanVerb<GenerateVerbOptions>(logger)
{
    public async Task<string?> GenerateOutputPath(GenerateVerbOptions options, IImageBox ib)
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

    public override async Task<bool> Execute(GenerateVerbOptions options, CancellationToken token)
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
}
