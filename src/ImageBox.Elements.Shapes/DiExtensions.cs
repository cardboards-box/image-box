namespace ImageBox.Elements;

using Shapes;
using Shapes.Animations;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class DiExtensions
{
    /// <summary>
    /// Adds all of the shape elements to the service collection
    /// </summary>
    /// <param name="services">The service collection to add to</param>
    /// <returns>The service collection for method chaining</returns>
    public static IServiceCollection AddShapeElements(this IServiceCollection services)
    {
        return services
            .AddTransient<IElement, LineElem>()
            .AddTransient<IElement, RectangleElem>()
            .AddTransient<IElement, PointElem>()
            .AddTransient<IElement, CircleElem>()
            .AddTransient<IElement, BezierAnimationElem>();
    }
}
