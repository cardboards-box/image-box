using ImageBox;
using ImageBox.Cli.Verbs;

return await new ServiceCollection()
    .AddImageBox()
    .AddSerilog()
    .AddConfig(c =>
    {
        c.AddEnvironmentVariables()
         .AddFile("appsettings.json");
    })
    .Cli(c => c
        .Add<GenerateVerb>()
        .Add<WatchVerb>()
        .Add<WatchDirVerb>());