using Path = System.IO.Path;

namespace ImageBox.Cli.Verbs;

using Documentation;

[Verb("doc", HelpText = "Generates documentation for the various aspects of image-box")]
public class DocVerbOptions
{
    public const string DEFAULT_JSON_FILE_NAME = "image-box-docs.json";
    public const string DEFAULT_MARKDOWN_FILE_NAME = "image-box-docs.md";

    [Option('j', "json-output", HelpText = "The file to write the documentation json to", Default = DEFAULT_JSON_FILE_NAME)]
    public string JsonOutput { get; set; } = DEFAULT_JSON_FILE_NAME;

    [Option('m', "markdown-output", HelpText = "The file to write the documentation markdown to", Default = DEFAULT_MARKDOWN_FILE_NAME)]
    public string MarkdownOutput { get; set; } = DEFAULT_MARKDOWN_FILE_NAME;
}

internal class DocVerb(
    ILogger<DocVerb> logger,
    IDocReflectionService _reflection,
    IMarkdownService _markdown) : BooleanVerb<DocVerbOptions>(logger)
{
    private readonly JsonSerializerOptions _indented = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public override async Task<bool> Execute(DocVerbOptions options, CancellationToken token)
    {
        var dir = Path.GetDirectoryName(options.JsonOutput);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var docs = _reflection.GetDocs();

        await using var io = File.Create(options.JsonOutput);
        await JsonSerializer.SerializeAsync(io, docs, _indented, token);
        _logger.LogInformation("Wrote documentation to {JsonOutput}", options.JsonOutput);

        await using var writer = new StreamWriter(options.MarkdownOutput);
        await _markdown.Render(writer, docs);
        _logger.LogInformation("Wrote documentation to {MarkdownOutput}", options.MarkdownOutput);
        return true;
    }
}
