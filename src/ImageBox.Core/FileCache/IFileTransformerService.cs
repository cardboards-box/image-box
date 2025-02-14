namespace ImageBox.Core.FileCache;

/// <summary>
/// A service for transforming files that are resolved
/// </summary>
public interface IFileTransformerService
{
    /// <summary>
    /// Transforms the file result based on the properties
    /// </summary>
    /// <param name="result">The file to transform</param>
    /// <param name="properties">The properties of the file</param>
    /// <returns>The transformed file</returns>
    Task<FileResult> Transform(FileResult result, FileFetchProperties properties);
}
