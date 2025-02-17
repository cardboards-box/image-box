namespace ImageBox.Cli.Verbs;

using Documentation;

[Verb("doc", HelpText = "Generates documentation for the various aspects of image-box")]
public class DocVerbOptions
{
    public const string DEFAULT_FILE_NAME = "image-box-docs.json";

    [Option('o', "output", HelpText = "The file to write the documentation json to", Default = DEFAULT_FILE_NAME)]
    public string Output { get; set; } = DEFAULT_FILE_NAME;
}

internal class DocVerb(
    ILogger<DocVerb> logger,
    IDocReflectionService _reflection) : BooleanVerb<DocVerbOptions>(logger)
{
    private readonly JsonSerializerOptions _indented = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public override async Task<bool> Execute(DocVerbOptions options, CancellationToken token)
    {
        var dir = Path.GetDirectoryName(options.Output);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var docs = _reflection.GetDocs();

        await using var io = File.Create(options.Output);
        await JsonSerializer.SerializeAsync(io, docs, _indented, token);
        _logger.LogInformation("Wrote documentation to {Output}", options.Output);
        return true;
    }
}
