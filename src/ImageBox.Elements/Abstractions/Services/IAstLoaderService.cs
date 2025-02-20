namespace ImageBox.Elements;

/// <summary>
/// Service for loading <see cref="LoadedAst"/> from a file
/// </summary>
public interface IAstLoaderService
{
    /// <summary>
    /// Loads the <see cref="LoadedAst"/> from the given path
    /// </summary>
    /// <param name="path">The path to load</param>
    /// <returns>The loaded <see cref="LoadedAst"/></returns>
    /// <exception cref="FileNotFoundException">Thrown if the file could not be found</exception>
    /// <exception cref="InvalidOperationException">Thrown if the loaded boxed image is null</exception>
    /// <exception cref="InvalidOperationException">Thrown if the file does not exist after downloading</exception>
    /// <exception cref="InvalidOperationException">Thrown if the zip file contains another zip file</exception>
    /// <exception cref="InvalidOperationException">Thrown if a module with the same name is already loaded</exception>
    /// <exception cref="RenderContextException">Thrown if there is a top-level bind or spread</exception>
    Task<LoadedAst> Load(IOPath path);
}
