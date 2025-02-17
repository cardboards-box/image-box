using ImageBox;
using ImageBox.Cli.Documentation;
using ImageBox.Cli.Verbs;

return await new ServiceCollection()
    .AddSerilog()
    .AddConfig(c => c
        .AddEnvironmentVariables()
         .AddFile("appsettings.json"), out var config)
    .AddImageBox(config)
    .AddTransient<IDocReflectionService, DocReflectionService>()
    .Cli(c => c
        .Add<GenerateVerb>()
        .Add<WatchVerb>()
        .Add<WatchDirVerb>()
        .Add<DocVerb>());