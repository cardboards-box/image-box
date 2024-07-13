namespace ImageBox.Rendering.Base;

using Services;

/// <summary>
/// Represents an element that can be rendered to the image
/// </summary>
public abstract class RenderElement : Element
{
    /// <summary>
    /// Applies the element to the render context
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    public abstract Task Render(ContextFrame context);
}
