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
        foreach(var (key, obj) in value.Enumerator())
            _engine.SetValue(key, obj);
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
    /// <param name="context">The context of the execution</param>
    /// <returns>The value of the expression</returns>
    public JsValue? Evaluate(JsValue? context = null)
    {
        if (context is null) SetContext(context);
        return _engine.Evaluate(_statement);
    }
}