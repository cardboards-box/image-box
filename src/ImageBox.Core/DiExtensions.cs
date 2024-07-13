namespace ImageBox.Core;

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
            .AddTransient<IFileResolverService, FileResolverService>();
    }
}
