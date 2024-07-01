using ImageBox;
using ImageBox.Cli.Verbs;

return await new ServiceCollection()
    .AddImageBox()
    .AddSerilog()
    .AddAppSettings()
    .Cli(c => c
        .Add<GenerateVerb>()
        .Add<WatchVerb>());