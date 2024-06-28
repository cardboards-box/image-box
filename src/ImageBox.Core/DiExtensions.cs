﻿namespace ImageBox.Core;

/// <summary></summary>
public static class DiExtensions
{
    /// <summary></summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        return services
            .AddJson()
            .AddCardboardHttp()
            .AddTransient<IFileCacheService, FileCacheService>()
            .AddTransient<IFileResolverService, FileResolverService>();
    }

    /// <summary>
    /// Add the image box configuration from the instance
    /// </summary>
    /// <typeparam name="T">The image box configuration type</typeparam>
    /// <param name="services">The service collection to bind to</param>
    /// <param name="config">The instance of the configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddImageBoxConfig<T>(this IServiceCollection services, T config)
        where T: class, IBoxedImageConfig
    {
        return services
            .AddSingleton<IBoxedImageConfig>(config);
    }

    /// <summary>
    /// Add the image box configuration from concrete type
    /// </summary>
    /// <typeparam name="T">The image box configuration type</typeparam>
    /// <param name="services">The service collection to bind to</param>
    /// <param name="singleton">Whether or not to register the service as a singleton</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddImageBoxConfig<T>(this IServiceCollection services, bool singleton)
        where T: class, IBoxedImageConfig
    {
        if (singleton)
            return services
                .AddSingleton<IBoxedImageConfig, T>();
       
        return services
            .AddTransient<IBoxedImageConfig, T>();
    }

    /// <summary>
    /// Add the image box configuration from the default config provider
    /// </summary>
    /// <param name="services">The service collection to bind to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddImageBoxConfig(this IServiceCollection services)
    {
        return services
            .AddImageBoxConfig<BoxedImageConfig>(true);
    }
}
