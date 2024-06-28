namespace ImageBox.Core.FileCache;

/// <summary>
/// Represents the result of a file cache operation
/// </summary>
/// <param name="Stream">The file stream</param>
/// <param name="FileName">The file name</param>
/// <param name="MimeType">The mime type</param>
public record class FileCacheResult(
    Stream Stream,
    string FileName,
    string MimeType);
