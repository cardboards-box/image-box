namespace ImageBox;

using Ast;
using Drawing;
using Rendering;
using Services;

/// <summary>
/// Extension methods for the service collections
/// </summary>
public static class DiExtensions
{
    /// <summary>
    /// Adds all of the image box services to the service collection
    /// </summary>
    /// <param name="services">The service collection to add to</param>
    /// <param name="onlyInternal">Whether to only register internal IB services or include 3rd party ones as well</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddImageBox(this IServiceCollection services, bool onlyInternal = false)
    {
        return services
            .AddBaseImageBox(onlyInternal)
            .AddImageBoxConfig();
    }

    /// <summary>
    /// Adds all of the image box services to the service collection
    /// </summary>
    /// <typeparam name="T">The image box configuration type</typeparam>
    /// <param name="services">The service collection to add to</param>
    /// <param name="config">The instance of the configuration</param>
    /// <param name="onlyInternal">Whether to only register internal IB services or include 3rd party ones as well</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddImageBox<T>(this IServiceCollection services, T config, bool onlyInternal = false)
        where T : class, IImageBoxConfig
    {
        return services
            .AddBaseImageBox(onlyInternal)
            .AddImageBoxConfig(config);
    }

    /// <summary>
    /// Adds all of the image box services to the service collection
    /// </summary>
    /// <typeparam name="T">The image box configuration type</typeparam>
    /// <param name="services">The service collection to add to</param>
    /// <param name="singleton">Whether or not to register the service as a singleton</param>
    /// <param name="onlyInternal">Whether to only register internal IB services or include 3rd party ones as well</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddImageBox<T>(this IServiceCollection services, bool singleton, bool onlyInternal = false)
        where T : class, IImageBoxConfig
    {
        return services
            .AddBaseImageBox(onlyInternal)
            .AddImageBoxConfig<T>(singleton);
    }

    internal static IServiceCollection AddBaseImageBox(this IServiceCollection services, bool onlyInternal)
    {
        if (!onlyInternal)
            services
                .AddJson()
                .AddCardboardHttp();

        return services
            .AddCore()
            .AddAst()
            .AddDrawing()
            .AddServices()
            .AddCustomElements()
            .AddTransient<IImageBoxService, ImageBoxService>();
    }
}
