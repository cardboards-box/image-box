namespace ImageBox.Elements;

/// <summary>
/// A service for generating render contexts and resolving scripts
/// </summary>
public interface IContextGeneratorService
{
    /// <summary>
    /// Generates the render context from the given boxed image
    /// </summary>
    /// <param name="image">The boxed image to create the context for</param>
    /// <returns>The render context</returns>
    /// <exception cref="RenderContextException">Thrown if a required property is missing</exception>
    /// <exception cref="RenderContextException">Thrown if any exception occurs during preparation</exception>
    /// <exception cref="RenderContextException">Thrown if a remote script failed to resolve</exception>
    /// <exception cref="RenderContextException">Thrown if the script body is empty</exception>
    /// <exception cref="RenderContextException">Thrown if multiple setup scripts listed in the template</exception>
    /// <exception cref="RenderContextException">Thrown if a script module name is not set</exception>
    /// <exception cref="RenderContextException">Thrown if no setup script is listed in the template and there are other scripts</exception>
    /// <exception cref="RenderContextException">Thrown if there are no template elements</exception>
    /// <exception cref="RenderContextException">Thrown if there are more than one template elements</exception>
    Task<ContextBox> Generate(LoadedAst image);
}
