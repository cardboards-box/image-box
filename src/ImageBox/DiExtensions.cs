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
    /// <param name="config">The configuration instance</param>
    /// <param name="section">The root section for the configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddImageBox(this IServiceCollection services, IConfiguration config, string section = "ImageBox")
    {
        var data = new ServiceConfig();
        config.GetSection(section).Bind(data);

        return services
            .AddBaseImageBox(data.InternalServicesOnly)
            .AddSingleton(config)
            .AddSingleton<IServiceConfig>(data);
    }

    /// <summary>
    /// Adds all of the image box services to the service collection
    /// </summary>
    /// <param name="services">The service collection to add to</param>
    /// <param name="config">The configuration for image box</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddImageBox(this IServiceCollection services, IServiceConfig config)
    {
        return services
            .AddBaseImageBox(config.InternalServicesOnly)
            .AddSingleton(config);
    }

    /// <summary>
    /// Gets all of the assemblies for the image-box library
    /// </summary>
    /// <returns>All of the assemblies for the image-box library</returns>
    /// <remarks>This can be used as a target for <see cref="RenderConfig.AssemblyFactory"/> when rendering in asp.net core environments (cause it's silly)</remarks>
    public static IEnumerable<Assembly> ImageBoxAssemblies()
    {
        yield return typeof(DiExtensions).Assembly;
        yield return typeof(Ast.DiExtensions).Assembly;
        yield return typeof(Core.DiExtensions).Assembly;
        yield return typeof(Drawing.DiExtensions).Assembly;
        yield return typeof(Elements.TemplateElem).Assembly;
        yield return typeof(Rendering.DiExtensions).Assembly;
        yield return typeof(Scripting.Extensions).Assembly;
        yield return typeof(Services.DiExtensions).Assembly;
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
