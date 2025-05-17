namespace ImageBox.Elements.Shapes;

using Drawing;

/// <summary>
/// Represents an element that can be drawn or filled based on <see cref="IPath"/>s
/// </summary>
public abstract class DrawPathElement : PositionalElement, IParentElement
{
    /// <summary>
    /// The color to fill with
    /// </summary>
    [AstAttribute("color")]
    public AstValue<string?> Color { get; set; } = new();

    /// <summary>
    /// The color of the border of the rectangle
    /// </summary>
    [AstAttribute("border-color")]
    public AstValue<string?> BorderColor { get; set; } = new();

    /// <summary>
    /// The width of the border of the rectangle
    /// </summary>
    [AstAttribute("border-width")]
    public AstValue<SizeUnit?> BorderWidth { get; set; } = new();

    /// <summary>
    /// The number of degrees to rotate the path (around it's center) before rendering
    /// </summary>
    [AstAttribute("rotate")]
    public AstValue<float?> Rotate { get; set; } = new();

    /// <summary>
    /// The x coordinate of the origin point of the text rotation
    /// </summary>
    /// <remarks>Requires <see cref="RotateOriginY"/> to be set as well</remarks>
    [AstAttribute("rotate-origin-x")]
    public AstValue<SizeUnit?> RotateOriginX { get; set; } = new();

    /// <summary>
    /// The y coordinate of the origin point of the text rotation
    /// </summary>
    /// <remarks>Requires <see cref="RotateOriginX"/> to be set as well</remarks>
    [AstAttribute("rotate-origin-y")]
    public AstValue<SizeUnit?> RotateOriginY { get; set; } = new();

    /// <summary>
    /// All of the child elements on the parent element
    /// </summary>
    public IElement[] Children { get; set; } = [];

    /// <summary>
    /// Get the path of the current element
    /// </summary>
    /// <param name="context">The size of the current context</param>
    /// <param name="origin">The origin to use for the path</param>
    /// <returns>The path</returns>
    public abstract IPath GetPath(SizeContext context, Vector2? origin = null);

    /// <summary>
    /// Applies any transforms for the current element
    /// </summary>
    /// <param name="context">The size of the current context</param>
    /// <param name="path">The path of the element</param>
    /// <returns>The transformed path</returns>
    public virtual IPath ApplyTransforms(SizeContext context, IPath path)
    {
        IPath ApplyRotate(IPath path)
        {
            //If not rotation is specified, skip rotates.
            if (!Rotate.Value.HasValue)
                return path;

            var degrees = Rotate.Value.Value;

            //If no rotate origin is specified, use the built in
            //transform for rotating around the center
            if (!RotateOriginX.Value.HasValue ||
                !RotateOriginY.Value.HasValue)
                return path.RotateDegree(degrees);

            //Get the origin point
            var x = RotateOriginX.Value.Value.Pixels(context, true);
            var y = RotateOriginY.Value.Value.Pixels(context, false);
            //Get the transform from the rotation
            var transform = Matrix3x2Extensions.CreateRotationDegrees(degrees, new Vector2(x, y));
            //Apply the transform to the path
            return path.Transform(transform);
        }

        //Change all of the transforms (only rotates for now)
        return ApplyRotate(path);
    }

    /// <summary>
    /// Applies the element to the render context
    /// </summary>
    /// <param name="context">The rendering context</param>
    /// <returns></returns>
    public override async Task Render(ContextFrame context)
    {
        var scope = context.LastScope;
        var current = this.BoundContext(scope.Size);
        var path = GetPath(current);
        path = ApplyTransforms(scope.Size, path);

        if (!string.IsNullOrWhiteSpace(Color.Value))
        {
            var color = Color.Value.ParseColor();
            context.Image.Mutate(x => x.Fill(color, path));
        }

        if (!string.IsNullOrWhiteSpace(BorderColor.Value) && BorderWidth.Value is not null)
        {
            var color = BorderColor.Value.ParseColor();
            var width = BorderWidth.Value.Value.Pixels(current);
            context.Image.Mutate(x => x.Draw(color, width, path));
        }

        if (Children.Length == 0) return;

        await this.RenderChildren(context, current);
    }
}
