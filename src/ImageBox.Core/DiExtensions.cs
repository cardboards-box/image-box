namespace ImageBox.Core;

using FileCache.Sources;

/// <summary></summary>
public static class DiExtensions
{
    /// <summary></summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        return services
            .AddTransient<IFileCacheService, FileCacheService>()
            .AddTransient<IFileResolverService, FileResolverService>()
            
            .AddTransient<IFileSourceService, HttpFileSource>()
            .AddTransient<IFileSourceService, LocalFileSource>();
    }
}
