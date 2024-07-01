namespace ImageBox.Rendering;

using Directives;
using Renderers;

/// <summary>
/// DI extensions for adding custom elements
/// </summary>
public static class DiExtensions
{
    /// <summary>
    /// Add the custom elements to the service collection
    /// </summary>
    /// <param name="services">The service collection to attach to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCustomElements(this IServiceCollection services)
    {
        //We don't actually need to do this, because they are never resolved via DI services
        //However, C#'s type system will not include these classes in the assembly if they are not referenced
        //So here we are, "using" them in some way...
        return services
            .AddSingleton<ForEachDir>()
            .AddSingleton<IfDir>()
            .AddSingleton<RangeDir>()
            .AddSingleton<ClearElem>()
            .AddSingleton<ImageElem>()
            .AddSingleton<RectangleElem>()
            .AddSingleton<TextElem>();
    }
}
