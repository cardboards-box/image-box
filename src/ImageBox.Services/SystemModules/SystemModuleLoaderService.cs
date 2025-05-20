using Jint.Runtime.Modules;

namespace ImageBox.Services.SystemModules;

internal class SystemModuleLoaderService(
    ILogger<SystemModuleLoaderService> _logger,
    IFileResolverService _resolver) : IModuleSourceService
{
    public string Name => "system";

    public Task<Action<ModuleBuilder>> Register(LoadedAst image)
    {
        void Builder(ModuleBuilder builder)
        {
            builder
                .ExportType<Drawing>()
                .ExportType<Context>()
                .ExportObject("logger", new Logger(_logger, image))
                .ExportObject("imaging", new Imaging(_resolver, image));
        }

        return Task.FromResult(Builder);
    }
}
