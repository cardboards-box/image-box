using Jint.Native;

namespace ImageBox.Services;

using Ast;
using Scripting;

/// <summary>
/// Represents the rendering scope for boxed images
/// </summary>
public class ContextScope(ContextFrame _frame) : IDisposable
{
    /// <summary>
    /// The render element that owns this scope
    /// </summary>
    public IElement? Element { get; init; }

    /// <summary>
    /// All the variables in this scope
    /// </summary>
    public Dictionary<string, object?> Variables { get; init; } = [];

    /// <summary>
    /// The render size in the current scope
    /// </summary>
    public required SizeContext Size { get; set; }

    /// <summary>
    /// The frame this scope is attached to
    /// </summary>
    public ContextFrame Frame => _frame;

    /// <summary>
    /// The AST element that owns this scope
    /// </summary>
    public AstElement? AstElement => Element?.Context;

    /// <summary>
    /// Set the size of the current scope
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public ContextScope Set(SizeContext size)
    {
        Size = size;
        return this;
    }

    /// <summary>
    /// Sets the variables for the scope from the given value
    /// </summary>
    /// <param name="value">The value to attach by</param>
    public ContextScope Set(JsValue? value)
    {
        value.AppendTo(Variables);
        return this;
    }

    /// <summary>
    /// Sets a variable in the current scope
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public ContextScope Set(string key, object? value)
    {
        if (!Variables.TryAdd(key, value))
            Variables[key] = value;
        return this;
    }

    /// <summary>
    /// Sets the variables for the scope from the given dictionary
    /// </summary>
    /// <param name="values">The scope variables</param>
    public ContextScope Set(Dictionary<string, object?> values)
    {
        foreach (var (key, value) in values)
            Set(key, value);
        return this;
    }

    /// <summary>
    /// Removes the current scope from the stack
    /// </summary>
    public void Dispose()
    {
        Variables.Clear();
        _frame.RemoveScope(this);
        GC.SuppressFinalize(this);
    }
}