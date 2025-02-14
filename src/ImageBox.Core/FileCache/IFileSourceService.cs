namespace ImageBox.Core.FileCache;

/// <summary>
/// A service to fetch a file from the given properties
/// </summary>
public interface IFileSourceService
{
    /// <summary>
    /// Fetches the file from the given properties
    /// </summary>
    /// <param name="properties">The properties to use to fetch the file</param>
    /// <returns>The result of the file or null if it could not be resolved</returns>
    Task<FileResult?> Fetch(FileFetchProperties properties);
}
