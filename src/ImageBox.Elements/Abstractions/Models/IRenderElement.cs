namespace ImageBox.Elements;

/// <summary>
/// Represents an element that can be rendered to the image
/// </summary>
public interface IRenderElement : IElement
{
    /// <summary>
    /// Applies the element to the render context
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    Task Render(ContextFrame context);
}
