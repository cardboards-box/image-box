namespace ImageBox;

using Ast;
using Drawing;
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
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddImageBox(this IServiceCollection services)
    {
        return services
            .AddBaseImageBox()
            .AddImageBoxConfig();
    }

    /// <summary>
    /// Adds all of the image box services to the service collection
    /// </summary>
    /// <typeparam name="T">The image box configuration type</typeparam>
    /// <param name="services">The service collection to add to</param>
    /// <param name="config">The instance of the configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddImageBox<T>(this IServiceCollection services, T config)
        where T : class, IBoxedImageConfig
    {
        return services
            .AddBaseImageBox()
            .AddImageBoxConfig(config);
    }

    /// <summary>
    /// Adds all of the image box services to the service collection
    /// </summary>
    /// <typeparam name="T">The image box configuration type</typeparam>
    /// <param name="services">The service collection to add to</param>
    /// <param name="singleton">Whether or not to register the service as a singleton</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddImageBox<T>(this IServiceCollection services, bool singleton)
        where T : class, IBoxedImageConfig
    {
        return services
            .AddBaseImageBox()
            .AddImageBoxConfig<T>(singleton);
    }

    internal static IServiceCollection AddBaseImageBox(this IServiceCollection services)
    {
        return services
            .AddJson()
            .AddCardboardHttp()
            .AddCore()
            .AddAst()
            .AddDrawing()
            .AddServices();
    }
}
