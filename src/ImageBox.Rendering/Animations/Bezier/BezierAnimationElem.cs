namespace ImageBox.Rendering.Animations.Bezier;

/// <summary>
/// Renders the children of the element with a Bezier animation
/// </summary>
/// <param name="_execution">The script execution service</param>
[AstElement("animation-bezier")]
public class BezierAnimationElem(IScriptExecutionService _execution) : PositionalElement(_execution), IParentElement
{
    /// <summary>
    /// The points of the curve to animate between
    /// </summary>
    public PointElem[] Points => Children.OfType<PointElem>().ToArray();

    /// <summary>
    /// All of the child elements on the parent element
    /// </summary>
    public IElement[] Children { get; set; } = [];

    /// <summary>
    /// Renders the element to the render context
    /// </summary>
    /// <param name="context">The context to render</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override Task Render(RenderContext context)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// The type of Bezier curve to use
    /// </summary>
    public enum BezierType
    {
        /// <summary>
        /// Linear interpolation
        /// </summary>
        Linear,
        /// <summary>
        /// Quadratic interpolation
        /// </summary>
        Quadratic,
        /// <summary>
        /// Cubic interpolation
        /// </summary>
        Cubic,
    }
}