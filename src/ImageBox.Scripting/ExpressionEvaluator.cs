using Acornima.Ast;
using Jint;
using Jint.Native;

namespace ImageBox.Scripting;

/// <summary>
/// Evaluate JavaScript expressions within a certain scope
/// </summary>
/// <param name="Expression"></param>
public class ExpressionEvaluator(string Expression)
{
    /// <summary>
    /// The prepared statement for the expression
    /// </summary>
    public Prepared<Script> Statement { get; } = Engine.PrepareScript(Expression);

    /// <summary>
    /// Evaluates the current expression
    /// </summary>
    /// <param name="builder">The expression builder for setting variables</param>
    /// <returns>The value of the expression</returns>
    public JsValue? Evaluate(Action<ExpressionBuilder>? builder = null)
    {
        using var bob = new ExpressionBuilder(Statement);
        builder?.Invoke(bob);
        return bob.Run();
    }

    /// <summary>
    /// The expression builder for setting variables
    /// </summary>
    /// <param name="Script">The script to be run</param>
    public class ExpressionBuilder(Prepared<Script> Script) : IDisposable
    {
        private readonly Engine _engine = new();

        /// <summary>
        /// Set the scope from the given variables
        /// </summary>
        /// <param name="variables">The variables to set</param>
        /// <returns>The builder for chaining</returns>
        public ExpressionBuilder Set(JsValue? variables)
        {
            foreach (var (key, obj) in variables.Enumerator())
                _engine.SetValue(key, obj);
            return this;
        }

        /// <summary>
        /// Set the scope from the given variables
        /// </summary>
        /// <param name="variables">The variables to set</param>
        /// <returns>The builder for chaining</returns>
        public ExpressionBuilder Set(Dictionary<string, object?> variables)
        {
            foreach (var (key, obj) in variables)
                _engine.SetValue(key, obj);
            return this;
        }

        /// <summary>
        /// Executes the script with the given variables
        /// </summary>
        /// <returns></returns>
        public JsValue? Run()
        {
            return _engine.Evaluate(Script);
        }

        /// <summary>
        /// Disposes of the engine
        /// </summary>
        public void Dispose()
        {
            _engine.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}