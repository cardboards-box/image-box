namespace ImageBox.Rendering;

/// <summary>
/// Extension methods for rendering the modules
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Gets the context from the positional data
    /// </summary>
    /// <param name="element">The element to get the context for</param>
    /// <param name="parent">The size context to bind from</param>
    /// <param name="fontSize">The size of the font in the context</param>
    /// <returns>The size context</returns>
    public static SizeContext BoundContext(this IPositionElement element, SizeContext parent, int? fontSize = null)
    {
        var x = element.X.Value?.Pixels(parent, true) ?? 0;
        var y = element.Y.Value?.Pixels(parent, false) ?? 0;
        var width = element.Width.Value?.Pixels(parent, true);
        var height = element.Height.Value?.Pixels(parent, false);

        return parent.GetContext(x, y, width, height, fontSize);
    }

    /// <summary>
    /// Gets the current scope from the context
    /// </summary>
    /// <param name="element">The element that is providing the scope</param>
    /// <param name="context">The context of the scope</param>
    /// <returns>The scope of the current element</returns>
    public static ContextScope Scoped(this IPositionElement element, ContextFrame context)
    {
        var previousScope = context.LastScope.Size;
        var fontSize = element.FontSize.Value?.Pixels(previousScope) ?? previousScope.FontSize;
        var current = element.BoundContext(previousScope, fontSize);
        return context.Scope(element, current);
    }

    /// <summary>
    /// Gets the file fetch properties from the given element
    /// </summary>
    /// <param name="element">The element to fetch from</param>
    /// <param name="size">The context size for the element</param>
    /// <param name="workDir">The working directory for the file</param>
    /// <returns>The properties to use when fetching the file</returns>
    public static FileFetchProperties Properties(this IFileElement element, SizeContext size, string? workDir)
    {
        return new(element.Source.Value)
        {
            Width = element.Width.Value?.Pixels(size, true),
            Height = element.Height.Value?.Pixels(size, false),
            UserAgent = element.UserAgent.Value,
            Accepts = element.Accepts.Value,
            ShouldCache = element.ShouldCache.Value ?? true,
            WorkingDirectory = workDir
        };
    }
}
