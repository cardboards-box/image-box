using Jint.Native;
using SixLabors.ImageSharp;

namespace ImageBox.Services;

using Loading;
using Scripting;

/// <summary>
/// Represents the context for rendering a single frame
/// </summary>
/// <param name="_frame">The frame number for the current render context</param>
/// <param name="_image">The image to render against</param>
/// <param name="_context">The full image context</param>
/// <param name="_variables">The variables to use for rendering</param>
/// <param name="_scripting">The script execution context</param>
/// <param name="_token">The cancellation token for cancelling rendering of this frame</param>
public class ContextFrame(
    int _frame, 
    Image _image,
    ContextBox _context,
    Dictionary<string, object?> _variables,
    IScriptExecutionService _scripting,
    CancellationToken _token) : IDisposable
{
    private readonly List<ContextScope> _scopes = [];
    private ContextScope? _frameScope;
    private ContextScope? _globalScope;

    /// <summary>
    /// The elements in the template
    /// </summary>
    public required IElement[] Elements { get; init; }

    /// <summary>
    /// The cancellation token for the entire image render
    /// </summary>
    public CancellationToken Token => _token;

    /// <summary>
    /// The current frame
    /// </summary>
    /// <value>If animation is disabled, this will always be 1</value>
    public int Frame => _frame;

    /// <summary>
    /// The total number of frames in the image
    /// </summary>
    public uint TotalFrames => _context.TotalFrames;

    /// <summary>
    /// The delay between frames in milliseconds
    /// </summary>
    public uint FrameDelay => _context.FrameDelay;

    /// <summary>
    /// The number of times to repeat the gif (0 is repeat forever)
    /// </summary>
    public ushort FrameRepeat => _context.FrameRepeat;

    /// <summary>
    /// Whether or not to animate the image
    /// </summary>
    public bool Animate => _context.Animate;

    /// <summary>
    /// The renderer for the frame to render
    /// </summary>
    public Image Image => _image;

    /// <summary>
    /// The full image context
    /// </summary>
    public ContextBox BoxContext => _context;

    /// <summary>
    /// The global scope for the image context
    /// </summary>
    public ContextScope GlobalScope => _globalScope ??= new ContextScope(this)
    {
        Element = null,
        Variables = _variables,
        Size = BoxContext.Size
    };

    /// <summary>
    /// The frame's scope for the frame context
    /// </summary>
    public ContextScope FrameScope => _frameScope ??= new ContextScope(this)
    {
        Element = null,
        Variables = new Dictionary<string, object?>
        {
            ["frame"] = Frame,
            ["image"] = Image,
            ["token"] = Token,
            ["width"] = BoxContext.Size.Width,
            ["height"] = BoxContext.Size.Height,
            ["fontSize"] = BoxContext.Size.FontSize,
            ["fontFamily"] = BoxContext.Size.FontFamily,
            ["animate"] = BoxContext.Animate,
            ["frameTotal"] = BoxContext.TotalFrames,
            ["frameDelay"] = BoxContext.FrameDelay,
            ["frameRepeat"] = BoxContext.FrameRepeat,
        },
        Size = BoxContext.Size
    };

    /// <summary>
    /// The most recent scope added to the stack
    /// </summary>
    public ContextScope LastScope => Stack.Last();

    /// <summary>
    /// The stack of render scopes
    /// </summary>
    public ContextScope[] Stack => [GlobalScope, FrameScope, .. _scopes];

    /// <summary>
    /// Whether or not the render should be cancelled
    /// </summary>
    public bool ShouldCancel => Token.IsCancellationRequested;

    /// <summary>
    /// Create a scope and add it to the current stack
    /// </summary>
    /// <param name="element">The element the scope is for</param>
    /// <param name="size">The size to use for the scope</param>
    /// <param name="variables">The variables for the scope</param>
    /// <param name="bindCurrent">Whether to bind the current element's attribute or the children</param>
    /// <returns>The render scope</returns>
    public ContextScope Scope(IElement element, SizeContext? size = null, Dictionary<string, object?>? variables = null, bool bindCurrent = false)
    {
        var scope = new ContextScope(this)
        {
            Element = element,
            Size = size ?? LastScope.Size,
            Variables = variables ?? []
        };
        AddScope(scope);
        if (!bindCurrent && element is IParentElement parent)
            _scripting.HandleAttributes(this, parent.Children);
        else if (bindCurrent) 
            _scripting.HandleAttributes(this, [element]);
        return scope;
    }

    /// <summary>
    /// Adds a scope to the stack
    /// </summary>
    /// <param name="scope">The render scope to add</param>
    public void AddScope(ContextScope scope) => _scopes.Add(scope);

    /// <summary>
    /// Removes a scope from the stack
    /// </summary>
    /// <param name="scope">The render scope to remove</param>
    public void RemoveScope(ContextScope scope)
    {
        _scopes.Remove(scope);
    }

    /// <summary>
    /// Bind and evaluate the expression 
    /// </summary>
    /// <param name="expression">The expression to evaluate</param>
    /// <returns>The return value</returns>
    public JsValue? Evaluate(ExpressionEvaluator expression)
    {
        return expression.Evaluate(c =>
        {
            foreach (var scope in Stack)
                c.Set(scope.Variables);
        });
    }

    /// <summary>
    /// Removes the global scope and disposes of the image
    /// </summary>
    public void Dispose()
    {
        _frameScope?.Dispose();
        foreach (var scope in _scopes.ToArray())
            scope.Dispose();
        GC.SuppressFinalize(this);
    }
}
