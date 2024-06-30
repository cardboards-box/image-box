using Jint.Native;
using SixLabors.ImageSharp;

namespace ImageBox.Services;

using Scripting;

/// <summary>
/// Represents a cached instance of a <see cref="BoxedImageData"/> for rendering
/// </summary>
public class RenderContext : IDisposable
{
    private readonly List<RenderScope> _scopes = [];
    private RenderScope? _globalScope;
    private Image? _image;

    /// <summary>
    /// The boxed image to render
    /// </summary>
    public required BoxedImageData Context { get; init; }

    /// <summary>
    /// The template for the image render
    /// </summary>
    public required TemplateElem Template { get; init; }

    /// <summary>
    /// The font families from the template
    /// </summary>
    public required FontFamilyElem[] FontFamilies { get; init; }

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
    /// How many times to repeat the gif
    /// </summary>
    /// <remarks>0 is repeat forever, x is repeat number of times</remarks>
    public ushort FrameRepeat { get; init; }

    /// <summary>
    /// The renderer for the image
    /// </summary>
    public Image Image
    {
        get => ValidateImage();
        set => _image = value;
    }

    /// <summary>
    /// The current frame (if animation is enabled)
    /// </summary>
    public int? Frame { get; set; }

    /// <summary>
    /// Whether or not the image render has been set
    /// </summary>
    public bool HasImage => _image is not null;

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
                ["animate"] = Animate,
                ["fonts"] = FontFamilies,
                ["fontSize"] = FontSize,
                ["frame"] = Frame,
                ["frameTotal"] = TotalFrames,
                ["frameDelay"] = FrameDelay,
                ["image"] = Context,
                ["imageWidth"] = Width,
                ["imageHeight"] = Height,
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
    public void SetRootScope(Dictionary<string, object?> variables)
    {
        var scope = GetRootScope();
        scope.Set(variables);
    }

    /// <summary>
    /// Adds or sets the variables for the root scope
    /// </summary>
    /// <param name="variables">The variables to set</param>
    public void SetRootScope(JsValue? variables)
    {
        var scope = GetRootScope();
        scope.Set(variables);
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
    /// Binds the scope of the expression evaluator
    /// </summary>
    /// <param name="expression">The expression evaluator to bind to</param>
    public void BindTo(ExpressionEvaluator expression)
    {
        //Set the variables from each of the scopes
        foreach (var scope in Stack)
            expression.SetContext(scope.Variables);
    }

    /// <summary>
    /// Get the value of the scope variable or null if it doesn't exist
    /// </summary>
    /// <param name="names">The potential names of the variables</param>
    /// <returns>The value of the scope variable</returns>
    public object? GetScopeValue(params string[] names)
    {
        //Get the stacks in reverse order so that the most recent scope is first
        var stack = Stack
            .ToArray()
            .Reverse();
        //Iterate through each stack
        foreach (var scope in stack)
        {
            foreach(var name in names)
            {
                //If the variable exists in the current scope, return it
                if (scope.Variables.TryGetValue(name, out var value))
                    return value;
            }
        }
        //If the variable does not exist in any scope, return null
        return null;
    }

    /// <summary>
    /// Clear the global scope of the context
    /// </summary>
    public void ClearGlobalScope()
    {
        _globalScope = null;
    }

    /// <summary>
    /// Get the current image renderer or throw an exception if it has not been set yet
    /// </summary>
    /// <returns>The image renderer</returns>
    /// <exception cref="RenderContextException">Thrown if the renderer hasn't been set  yet</exception>
    internal Image ValidateImage()
    {
        return _image ?? throw new RenderContextException("The image has not been rendered yet", Context, CurrentScope.AstElement);
    }

    /// <summary>
    /// Gets or creates the root scope for the image
    /// </summary>
    /// <returns>The root scope</returns>
    internal RenderScope GetRootScope()
    {
        if (HasGlobalScope)
            return _scopes.First();

        return AddScope(Template);
    }

    /// <summary>
    /// Dispose the render context
    /// </summary>
    public void Dispose()
    {
        _scopes.Clear();
        _image?.Dispose();
        GC.SuppressFinalize(this);
    }
}
