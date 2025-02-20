namespace ImageBox.Elements;

using Drawing;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class DiExtensions
{
    /// <summary>
    /// Adds all of the top level elements to the service collection
    /// </summary>
    /// <param name="services">The service collection to add to</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddDrawingElements(this IServiceCollection services)
    {
        return services
            .AddTransient<IElement, ClearElem>()
            .AddTransient<IElement, TextElem>()
            .AddTransient<IElement, ImageElem>();
    }
}
