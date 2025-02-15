namespace ImageBox.Core.FileCache;

using IOPath;

/// <summary>
/// The optional properties that can be passed to a file fetcher
/// </summary>
/// <param name="source">The URI of the file to fetch</param>
public class FileFetchProperties(IOPath source)
{
    /// <summary>
    /// The URI of the file to fetch
    /// </summary>
    public IOPath Source { get; } = source;

    /// <summary>
    /// The optional width of the file
    /// </summary>
    public int? Width { get; set; } = null;

    /// <summary>
    /// The optional height of the file
    /// </summary>
    public int? Height { get; set; } = null;

    /// <summary>
    /// The optional user agent to use when fetching the file
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// The optional accept header to use when fetching the file
    /// </summary>
    public string? Accepts { get; set; }

    /// <summary>
    /// Whether or not the file should potentially be cached
    /// </summary>
    public bool ShouldCache { get; set; } = true;

    /// <summary>
    /// The working directory to use for loading files
    /// </summary>
    public string? WorkingDirectory { get; set; }
}
