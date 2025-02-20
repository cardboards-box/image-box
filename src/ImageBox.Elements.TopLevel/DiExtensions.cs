namespace ImageBox.Elements;

using TopLevel;

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
    public static IServiceCollection AddTopLevelElements(this IServiceCollection services)
    {
        return services
            .AddTransient<IElement, TemplateElem>()
            .AddTransient<IElement, ScriptElem>()
            .AddTransient<IElement, ResourcesElem>()
            .AddTransient<IElement, RemoteResourceElem>()
            .AddTransient<IElement, FontFamilyElem>();
    }
}
