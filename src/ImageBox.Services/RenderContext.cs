namespace ImageBox.Services;

using Jint.Native;
using Scripting;

/// <summary>
/// Represents a cached instance of a <see cref="BoxedImage"/> for rendering
/// </summary>
public class RenderContext
{
    private readonly List<RenderScope> _scopes = [];
    private RenderScope? _globalScope;

    /// <summary>
    /// The boxed image to render
    /// </summary>
    public required BoxedImage Image { get; init; }

    /// <summary>
    /// The template for the image render
    /// </summary>
    public required TemplateElem Template { get; init; }

    /// <summary>
    /// The font families from the template
    /// </summary>
    public required FontFamilyElem[] FontFamilies { get; init; } = [];

    /// <summary>
    /// The context of the image size from unit conversion
    /// </summary>
    public required SizeContext Size { get; init; }

    /// <summary>
    /// The script runner for the setup module in the template
    /// </summary>
    public ScriptRunner? Runner { get; init; }

    /// <summary>
    /// The total number of frames in the image
    /// </summary>
    public int TotalFrames { get; init; }

    /// <summary>
    /// The delay between frames in milliseconds
    /// </summary>
    public int FrameDelay { get; init; }

    /// <summary>
    /// Whether or not animation is enabled
    /// </summary>
    public bool Animate => TotalFrames > 1;

    /// <summary>
    /// The current scope of the context
    /// </summary>
    public RenderScope CurrentScope => Stack.Last();

    /// <summary>
    /// The stack of render scopes
    /// </summary>
    public RenderScope[] Stack => [GlobalScope(), .. _scopes];

    /// <summary>
    /// Whether or not the global scope of the image has been set
    /// </summary>
    public bool HasGlobalScope => _scopes.Count > 0;

    /// <summary>
    /// The width of the image
    /// </summary>
    public int Width => Size.Width;

    /// <summary>
    /// The height of the image
    /// </summary>
    public int Height => Size.Height;

    /// <summary>
    /// The font size for the image
    /// </summary>
    public int FontSize => Size.FontSize;

    /// <summary>
    /// The current frame (if animation is enabled)
    /// </summary>
    public int? Frame { get; set; }

    /// <summary>
    /// Gets all of the variables for the global scope
    /// </summary>
    /// <returns></returns>
    public RenderScope GlobalScope()
    {
        return _globalScope ??= new RenderScope
        {
            Size = Size,
            Element = Template,
            Variables = new()
            {
                ["image"] = Image,
                ["imageWidth"] = Width,
                ["imageHeight"] = Height,
                ["fontSize"] = FontSize,
                ["frame"] = Frame,
                ["fonts"] = FontFamilies,
            }
        };
    }

    /// <summary>
    /// Adds a cope and returns it's instance for modification
    /// </summary>
    /// <param name="element">The element to attach the scope to</param>
    /// <param name="context">The size context for the scope</param>
    /// <param name="value">The value of the current scope to attach</param>
    /// <returns>The added scope for modification</returns>
    public RenderScope AddScope(IElement element, SizeContext? context = null, JsValue? value = null)
    {
        var scope = new RenderScope
        {
            Element = element,
            Variables = [],
            Size = context ?? Size
        };
        if (value is not null)
            scope.Set(value);
        AddScope(scope);
        return scope;
    }

    /// <summary>
    /// Adds a scope to the stack
    /// </summary>
    /// <param name="scope">The render scope to add</param>
    public void AddScope(RenderScope scope) => _scopes.Add(scope);

    /// <summary>
    /// Adds or sets the variables for the root scope
    /// </summary>
    /// <param name="variables">The variables to set</param>
    public void AddRootScope(Dictionary<string, object?> variables)
    {
        if (HasGlobalScope)
        {
            _scopes[0].Set(variables);
            return;
        }

        AddScope(new RenderScope
        {
            Element = Template,
            Variables = variables,
            Size = Size
        });
    }

    /// <summary>
    /// Remove the last scope from the stack
    /// </summary>
    public void RemoveLastScope()
    {
        if (_scopes.Count <= 1) return;

        _scopes.RemoveAt(_scopes.Count - 1);
    }

    /// <summary>
    /// Sets the scope of the expression evaluator
    /// </summary>
    /// <param name="expression"></param>
    public void SetScope(ExpressionEvaluator expression)
    {
        //Set the variables from each of the scopes
        foreach (var scope in Stack)
            expression.SetContext(scope.Variables);
    }
}
