namespace ImageBox.Elements;

/// <summary>
/// Handles executing and binding attributes in templates and scripts
/// </summary>
public interface IScriptExecutionService
{
    /// <summary>
    /// Executes the script and binds all of the top-level elements
    /// </summary>
    /// <param name="context">The render context to attach to</param>
    /// <returns></returns>
    /// <exception cref="RenderContextException">Thrown if something goes wrong during execution</exception>
    Task Execute(ContextFrame context);

    /// <summary>
    /// Traverse through all of the elements in the context and handle binds or spreads
    /// </summary>
    /// <param name="context">The render context</param>
    /// <param name="elements">The elements to traverse through</param>
    void HandleAttributes(ContextFrame context, IEnumerable<IElement>? elements = null);
}