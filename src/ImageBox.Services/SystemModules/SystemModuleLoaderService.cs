using Jint.Runtime.Modules;

namespace ImageBox.Services.SystemModules;

internal class SystemModuleLoaderService(
    ILogger<SystemModuleLoaderService> _logger) : IModuleSourceService
{
    public string Name => "system";

    public Task<Action<ModuleBuilder>> Register(LoadedAst image)
    {
        void Builder(ModuleBuilder builder)
        {
            builder
                .ExportType<Drawing>()
                .ExportType<Context>()
                .ExportObject("logger", new Logger(_logger, image));
        }

        return Task.FromResult(Builder);
    }
}
