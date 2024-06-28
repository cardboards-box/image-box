using Jint;
using Jint.Native;

namespace ImageBox;

using Ast;
using Elements.Base;

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
    /// The AST element that owns this scope
    /// </summary>
    public AstElement? AstElement => Element?.Context;

    /// <summary>
    /// Sets the variables for the scope from the given value
    /// </summary>
    /// <param name="value">The value to attach by</param>
    public void Set(JsValue? value)
    {
        if (value is null ||
            value.IsUndefined() ||
            value.IsNull()) return;


        if (value.IsArray())
        {
            foreach (var item in value.AsArray())
                Set(item);
            return;
        }

        if (!value.IsObject()) return;

        var dic = value.AsObject().GetOwnProperties();
        foreach (var prop in dic)
        {
            var key = prop.Key.ToString();
            if (Variables.TryAdd(key, prop.Value))
                Variables[key] = prop.Value;
        }
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
