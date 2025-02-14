namespace ImageBox.Rendering;

using Animations;
using Animations.Bezier;
using Directives;
using Elements;
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
        //This isn't necessary for the application to use the elements,
        //but it does allow the auto-documentation generator to see the elements
        return services
            .AddTransient<IElement, ForEachDir>()
            .AddTransient<IElement, IfDir>()
            .AddTransient<IElement, RangeDir>()
            .AddTransient<IElement, ClearElem>()
            .AddTransient<IElement, ImageElem>()
            .AddTransient<IElement, RectangleElem>()
            .AddTransient<IElement, TextElem>()
            .AddTransient<IElement, BezierAnimationElem>()
            .AddTransient<IElement, PointElem>()
            .AddTransient<IElement, FontFamilyElem>()
            .AddTransient<IElement, RemoteResourceElem>()
            .AddTransient<IElement, ScriptElem>()
            .AddTransient<IElement, TemplateElem>()
            .AddTransient<IElement, ResourcesElem>();
    }
}
