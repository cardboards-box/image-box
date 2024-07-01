using Jint.Native;

namespace ImageBox.Services;

using Loading;

/// <summary>
/// Handy class for managing the scope of a render context
/// </summary>
/// <param name="_context">The render context</param>
/// <param name="_execution">The script execution service</param>
/// <param name="_parent">The element this scope is bound to</param>
public class ScopeContext(
    RenderContext _context,
    IScriptExecutionService _execution,
    IElement _parent) : IDisposable
{
    /// <summary>
    /// The render context for the current scope
    /// </summary>
    public RenderContext Context { get; } = _context;

    /// <summary>
    /// The current scope for this context
    /// </summary>
    public RenderScope Scope { get; } = _context.AddScope(_parent);

    /// <summary>
    /// The size for the current context
    /// </summary>
    public SizeContext Size => Scope.Size;

    /// <summary>
    /// Set the size of the current scope
    /// </summary>
    /// <param name="size">The size</param>
    /// <returns>The current instance for chaining</returns>
    public ScopeContext SetSize(SizeContext size)
    {
        Scope.Size = size;
        return this;
    }

    /// <summary>
    /// Set the variables for the current scope
    /// </summary>
    /// <param name="vars">The variables</param>
    /// <returns>The current instance for chaining</returns>
    public ScopeContext SetVars(Dictionary<string, object?> vars)
    {
        Scope.Set(vars);
        return this;
    }

    /// <summary>
    /// Set the variables for the current scope
    /// </summary>
    /// <param name="vars">The variables</param>
    /// <returns>The current instance for chaining</returns>
    public ScopeContext SetVars(JsValue? vars)
    {
        Scope.Set(vars);
        return this;
    }

    /// <summary>
    /// Sets a single variable on the current scope
    /// </summary>
    /// <param name="name">The name of the variable</param>
    /// <param name="value">The value of the variable</param>
    /// <returns>The current instance for chaining</returns>
    public ScopeContext SetVar(string name, object? value)
    {
        Scope.Set(name, value);
        return this;
    }

    /// <summary>
    /// Binds the current scope to the parent element
    /// </summary>
    /// <returns>The current instance for disposal</returns>
    public ScopeContext Bind()
    {
        if (_parent is IParentElement parent)
            _execution.HandleAttributes(Context, parent.Children);
        return this;
    }

    /// <summary>
    /// Remove the scope from the context
    /// </summary>
    public void Dispose()
    {
        Context.RemoveLastScope();
        GC.SuppressFinalize(this);
    }
}
