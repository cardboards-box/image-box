namespace ImageBox.Drawing;

/// <summary></summary>
public static class DiExtensions
{
    /// <summary>
    /// Register the drawing services to the service collection
    /// </summary>
    /// <param name="services">The service collection to add to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDrawing(this IServiceCollection services)
    {
        return services
            .AddTransient<ISvgService, SvgService>();
    }
}
