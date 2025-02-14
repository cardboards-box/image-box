namespace ImageBox.Core.FileCache;

/// <summary>
/// Represents the result of a file request
/// </summary>
/// <param name="Stream">The stream to read the file contents from</param>
/// <param name="FileName">The name of the file</param>
/// <param name="MimeType">The type of the file</param>
/// <param name="Cacheable">Whether or not the result is cacheable</param>
public record class FileResult(
    Stream Stream,
    string FileName,
    string MimeType,
    bool Cacheable = true);
