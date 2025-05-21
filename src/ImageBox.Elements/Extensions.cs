using Jint;
using Jint.Native.Object;
using System.Diagnostics.CodeAnalysis;
using IFontStyle = SixLabors.Fonts.FontStyle;

namespace ImageBox.Elements;

using Drawing;

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
        if (element.Source.Value is null)
            throw new RenderContextException("File source is required for this element", element.Context);

        return new(element.Source.Value.Value)
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

    /// <summary>
    /// Renders all of the child elements of the given element
    /// </summary>
    /// <param name="element">The element whose children should be rendered</param>
    /// <param name="context">The context of the element</param>
    /// <param name="size">The size of the context</param>
    /// <param name="vars">The variables to give the new scope</param>
    /// <param name="bindCurrent">Whether or not to just bind the child elements or the current element as well</param>
    public static async Task RenderChildren(this IParentElement element, ContextFrame context, SizeContext? size = null, Dictionary<string, object?>? vars = null, bool bindCurrent = false)
    {
        using var scope = context.Scope(element, size, vars, bindCurrent);
        foreach (var child in element.Children)
            if (child is RenderElement render)
                await render.Render(context);
    }

    /// <summary>
    /// Tries to get the template size property from the given JS object
    /// </summary>
    /// <param name="obj">The JS Object to get the property from</param>
    /// <param name="name">The name of the property</param>
    /// <param name="defCtx">The default size context to use</param>
    /// <param name="isWidth">Whether or not this property is width/x-axis aligned or height/y-axis aligned</param>
    /// <param name="output">The value of the property</param>
    /// <returns>Whether or not the property was valid</returns>
    public static bool TryGetSize(this ObjectInstance? obj, string name, SizeContext defCtx, bool isWidth, [MaybeNullWhen(false)] out int output)
    {
        output = default;
        if (!obj.TryGetString(name, out var value))
            return false;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        value = value.Trim();

        if (float.TryParse(value, out var fo))
        {
            output = (int)fo;
            return true;
        }

        var size = SizeUnit.Parse(value);
        if (size == SizeUnit.Zero) return false;

        var pixels = size.Pixels(defCtx, isWidth);
        if (pixels <= 0) return false;

        output = pixels;
        return true;
    }

    /// <summary>
    /// Tries to get the time unit property from the given JS object
    /// </summary>
    /// <param name="obj">The JS Object to get the property from</param>
    /// <param name="name">The name of the property</param>
    /// <param name="output">The value of the property</param>
    /// <returns>Whether or not the property was valid</returns>
    public static bool TryGetTime(this ObjectInstance? obj, string name, [MaybeNullWhen(false)] out TimeUnit output)
    {
        output = default;
        if (!obj.TryGetString(name, out var value))
            return false;

        var time = TimeUnit.Parse(value);
        if (time == TimeUnit.Zero) return false;

        output = time;
        return true;
    }

    /// <summary>
    /// Tries to get the int from the given JS object
    /// </summary>
    /// <param name="obj">The JS Object to get the property from</param>
    /// <param name="name">The name of the property</param>
    /// <param name="output">The value of the property</param>
    /// <returns>Whether or not the property was valid</returns>
    public static bool TryGetInt(this ObjectInstance? obj, string name, [MaybeNullWhen(false)] out int output)
    {
        output = default;
        if (obj is null ||
            !obj.TryGetValue(name, out var value) ||
            value.IsUndefined()) return false;

        if (value.IsNumber())
        {
            output = (int)value.AsNumber();
            return true;
        }

        return value.IsString() && int.TryParse(value.ToString(), out output);
    }

    /// <summary>
    /// Tries to get the int from the given JS object
    /// </summary>
    /// <param name="obj">The JS Object to get the property from</param>
    /// <param name="name">The name of the property</param>
    /// <param name="output">The value of the property</param>
    /// <returns>Whether or not the property was valid</returns>
    public static bool TryGetUint(this ObjectInstance? obj, string name, [MaybeNullWhen(false)] out uint output)
    {
        output = default;
        if (obj is null ||
            !obj.TryGetValue(name, out var value) ||
            value.IsUndefined()) return false;

        if (value.IsNumber())
        {
            output = (uint)value.AsNumber();
            return true;
        }

        return value.IsString() && uint.TryParse(value.ToString(), out output);
    }

    /// <summary>
    /// Tries to get the int from the given JS object
    /// </summary>
    /// <param name="obj">The JS Object to get the property from</param>
    /// <param name="name">The name of the property</param>
    /// <param name="output">The value of the property</param>
    /// <returns>Whether or not the property was valid</returns>
    public static bool TryGetUshort(this ObjectInstance? obj, string name, [MaybeNullWhen(false)] out ushort output)
    {
        output = default;
        if (obj is null ||
            !obj.TryGetValue(name, out var value) ||
            value.IsUndefined()) return false;

        if (value.IsNumber())
        {
            output = (ushort)value.AsNumber();
            return true;
        }

        return value.IsString() && ushort.TryParse(value.ToString(), out output);
    }

    /// <summary>
    /// Tries to get the int from the given JS object
    /// </summary>
    /// <param name="obj">The JS Object to get the property from</param>
    /// <param name="name">The name of the property</param>
    /// <param name="output">The value of the property</param>
    /// <returns>Whether or not the property was valid</returns>
    public static bool TryGetDouble(this ObjectInstance? obj, string name, [MaybeNullWhen(false)] out double output)
    {
        output = default;
        if (obj is null ||
            !obj.TryGetValue(name, out var value) ||
            value.IsUndefined()) return false;

        if (value.IsNumber())
        {
            output = (double)value.AsNumber();
            return true;
        }

        return value.IsString() && double.TryParse(value.ToString(), out output);
    }

    /// <summary>
    /// Tries to get the string from the given JS object
    /// </summary>
    /// <param name="obj">The JS Object to get the property from</param>
    /// <param name="name">The name of the property</param>
    /// <param name="output">The value of the property</param>
    /// <returns>Whether or not the property was valid</returns>
    public static bool TryGetString(this ObjectInstance? obj, string name, [MaybeNullWhen(false)] out string output)
    {
        output = default;
        if (obj is null ||
            !obj.TryGetValue(name, out var value) ||
            value.IsUndefined()) return false;

        var str = value.ToString();
        if (string.IsNullOrWhiteSpace(str)) return false;

        output = str;
        return true;
    }
}
