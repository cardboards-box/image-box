using IFontStyle = SixLabors.Fonts.FontStyle;

namespace ImageBox.Elements;

using Drawing;
using Drawing.Models;

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
        int fontSize = previousScope.FontSize;
        if (element is IFontElement fontElem)
            fontSize = fontElem.FontSize.Value?.Pixels(previousScope) ?? previousScope.FontSize;
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

    /// <summary>
    /// Gets the raw font from the context, using the <see cref="IFontElement.FontSize"/> value
    /// </summary>
    /// <param name="element">The element to get the font from</param>
    /// <param name="context">The font context</param>
    /// <returns>The font</returns>
    /// <exception cref="RenderContextException">Thrown if the font family is not present</exception>
    internal static Font GetRawFontFromContext(this IFontElement element, ContextScope context)
    {
        var fontName = element.FontFamily.Value ?? context.Size.FontFamily;
        if (string.IsNullOrEmpty(fontName))
            throw new RenderContextException(
                "Font family is required for this element",
                context.Frame.BoxContext.Ast, element.Context);

        var style = IFontStyle.Regular;
        if (!string.IsNullOrEmpty(element.FontStyle.Value) &&
            Enum.TryParse<IFontStyle>(element.FontStyle.Value, true, out var parsed))
            style = parsed;

        return context.Frame.BoxContext.Fonts.GetFont(fontName, context, style);
    }

    /// <summary>
    /// Gets the font for the current element
    /// </summary>
    /// <param name="element">The element to get the font from</param>
    /// <param name="text">The text to be rendered</param>
    /// <param name="context">The font context</param>
    /// <returns>The font</returns>
    public static Font GetFont(this IFontElement element, string text, ContextScope context)
    {
        var auto = element.AutoFontSize.Value ?? false;
        var font = GetRawFontFromContext(element, context);
        if (!auto) return font;

        var rect = context.Size.GetRectangle();
        float padding = element.AutoFontSizePadding.Value?.Pixels(context.Size, true) ?? 0;

        if (!Enum.TryParse<WordBreaking>(element.WordBreaking?.Value, true, out var workBreaking))
            workBreaking = WordBreaking.Standard;

        var minFontSize = FontSizeUtil.DetermineFontSize(text, rect, padding, font, workBreaking);
        return new(font, minFontSize);
    }

    /// <summary>
    /// Gets the sibling element at the given index
    /// </summary>
    /// <param name="element">The current element</param>
    /// <param name="index">The index of the sibling to fetch</param>
    /// <returns>The element at the given index or null if not found</returns>
    public static IElement? Sibling(this IElement element, int index)
    {
        var parentEl = element.ParentElement;
        if (parentEl is null || parentEl is not IParentElement parent) return null;

        var children = parent.Children;
        if (index >= children.Length || index < 0) return null;

        return children[index];
    }

    /// <summary>
    /// Gets the previous sibling element
    /// </summary>
    /// <param name="element">The element to get the sibling of</param>
    /// <returns>The previous sibling or null if there isn't one</returns>
    public static IElement? PreviousSibling(this IElement element)
    {
        return Sibling(element, element.SiblingIndex - 1);
    }

    /// <summary>
    /// Gets the next sibling element
    /// </summary>
    /// <param name="element">The element to get the sibling of</param>
    /// <returns>The next sibling or null if there isn't one</returns>
    public static IElement? NextSibling(this IElement element)
    {
        return Sibling(element, element.SiblingIndex + 1);
    }

    /// <summary>
    /// Traverses the siblings of the current element
    /// </summary>
    /// <param name="element">The element whose siblings should be traversed</param>
    /// <param name="predicate">The predicate for the element to find</param>
    /// <param name="forward">Whether to increment (<see langword="true"/>) or decrement (<see langword="false"/>) through the siblings</param>
    /// <returns>All of the elements matching the predicate in the direction given</returns>
    /// <remarks>This does not include the current element in the results</remarks>
    public static IEnumerable<IElement> TraverseSiblings(this IElement element, Func<IElement, bool> predicate, bool forward = false)
    {
        IElement? Next(IElement current) => forward
            ? current.NextSibling()
            : current.PreviousSibling();

        var current = Next(element);
        while (current is not null)
        {
            var result = predicate(current);
            if (!result) break;

            yield return current;
            current = Next(current);
        }
    }
}
