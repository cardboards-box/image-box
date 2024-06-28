namespace ImageBox.Core.FileCache;

/// <summary>
/// Represents an item that has been cache to the file system
/// </summary>
/// <param name="Name">The name of the file</param>
/// <param name="MimeType">The mime type of the file</param>
/// <param name="Created">When the file was created</param>
public record class FileCacheItem(
    string Name,
    string MimeType,
    DateTime Created);