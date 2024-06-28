using Esprima.Ast;
using Jint;
using Jint.Native;

namespace ImageBox.Scripting;

/// <summary>
/// Evaluate JavaScript expressions within a certain scope
/// </summary>
/// <param name="Expression"></param>
public class ExpressionEvaluator(
    string Expression)
{
    private readonly Engine _engine = new();
    private readonly Prepared<Script> _statement = Engine.PrepareScript(Expression);

    /// <summary>
    /// Sets the context using the given value
    /// </summary>
    /// <param name="value">The value to read properties from</param>
    /// <returns>The current expression evaluator for chaining</returns>
    public ExpressionEvaluator SetContext(JsValue? value)
    {
        if (value is null ||
            value.IsUndefined() ||
            value.IsNull()) return this;

        if (value.IsArray())
        {
            foreach (var item in value.AsArray())
                SetContext(item);
            return this;
        }

        if (!value.IsObject()) return this;

        var dic = value.AsObject().GetOwnProperties();
        foreach (var prop in dic)
        {
            _engine.SetValue(prop.Key.ToString(), prop.Value);
        }
        return this;
    }

    /// <summary>
    /// Sets the context using the given dictionary
    /// </summary>
    /// <param name="value">The value to read properties of</param>
    /// <returns>The current expression evaluator for chaining</returns>
    public ExpressionEvaluator SetContext(Dictionary<string, object?> value)
    {
        foreach (var (key, obj) in value)
            _engine.SetValue(key.ToString(), obj);

        return this;
    }

    /// <summary>
    /// Evaluates the current expression
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public JsValue? Evaluate(JsValue? context)
    {
        SetContext(context);
        return _engine.Evaluate(_statement);
    }
}

