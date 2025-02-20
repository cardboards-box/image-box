namespace ImageBox.Elements;

using Directives;
using Directives.SwitchElems;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class DiExtensions
{
    /// <summary>
    /// Adds all of the directive elements to the service collection
    /// </summary>
    /// <param name="services">The service collection to add to</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddDirectiveElements(this IServiceCollection services)
    {
        return services
            .AddTransient<IElement, ForEachDir>()
            .AddTransient<IElement, RangeDir>()
            .AddTransient<IElement, IfDir>()
            .AddTransient<IElement, SwitchDir>()
            .AddTransient<IElement, SwitchCaseDir>()
            .AddTransient<IElement, SwitchDefaultDir>();
    }
}
