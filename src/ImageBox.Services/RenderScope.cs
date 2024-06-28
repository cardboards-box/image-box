using Jint.Native;

namespace ImageBox.Services;

using Ast;
using Scripting;

/// <summary>
/// Represents the rendering scope for boxed images
/// </summary>
public class RenderScope
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
    public required SizeContext Size { get; init; }

    /// <summary>
    /// The AST element that owns this scope
    /// </summary>
    public AstElement? AstElement => Element?.Context;

    /// <summary>
    /// Sets the variables for the scope from the given value
    /// </summary>
    /// <param name="value">The value to attach by</param>
    public void Set(JsValue? value)
    {
        value.AppendTo(Variables);
    }

    /// <summary>
    /// Sets a variable in the current scope
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Set(string key, object? value)
    {
        if (!Variables.TryAdd(key, value))
            Variables[key] = value;
    }

    /// <summary>
    /// Sets the variables for the scope from the given dictionary
    /// </summary>
    /// <param name="values">The scope variables</param>
    public void Set(Dictionary<string, object?> values)
    {
        foreach (var (key, value) in values)
            Set(key, value);
    }
}
