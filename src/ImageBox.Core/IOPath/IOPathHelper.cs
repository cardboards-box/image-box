namespace ImageBox.Core.IOPath;

/// <summary>
/// Helper utility class for dealing with IO paths
/// </summary>
public static class IOPathHelper
{
    /// <summary>
    /// The character used to separate path segments in Windows
    /// </summary>
    public const char WINDOWS_PATH_SEPARATOR = '\\';

    /// <summary>
    /// The character used to separate path segments in Unix
    /// </summary>
    public const char UNIX_PATH_SEPARATOR = '/';

    /// <summary>
    /// A map between known URI schemes and IO path types
    /// </summary>
    public static Dictionary<string, IOPathType> PathTypeMap { get; } = new()
    {
        { Uri.UriSchemeHttp, IOPathType.REMOTE_HTTP },
        { Uri.UriSchemeHttps, IOPathType.REMOTE_HTTP },
        { Uri.UriSchemeFtp, IOPathType.REMOTE_FTP },
        { Uri.UriSchemeFtps, IOPathType.REMOTE_FTP },
        { Uri.UriSchemeFile, IOPathType.LOCAL_ABSOLUTE }
    };

    /// <summary>
    /// Checks whether the file exists at the given path
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <returns>Whether or not the file exists</returns>
    /// <remarks>Remote paths will always return false</remarks>
    public static bool FileExists(IOPath path)
    {
        return path.Type.HasFlag(IOPathType.Local)
            && File.Exists(path.OSSafe);
    }

    /// <summary>
    /// Gets the path in the format the current operating system expects
    /// </summary>
    /// <param name="path">The path to convert</param>
    /// <param name="type">The type of path we're dealing with</param>
    /// <returns>The operating system safe path</returns>
    public static string CurrentOsSafe(string path, IOPathType type)
    {
        //Remote paths are assumed to be correct already
        if (type == IOPathType.Unknown ||
            !type.HasFlag(IOPathType.Local)) return path;

        //Local paths need to be converted to the correct value for the current OS
        var parts = path.Split(WINDOWS_PATH_SEPARATOR, UNIX_PATH_SEPARATOR);
        return Path.Combine(parts);
    }

    /// <summary>
    /// Determines the type of path based on the path string
    /// </summary>
    /// <param name="path">The path string</param>
    /// <returns>The type of path</returns>
    public static IOPathType DetermineType(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return IOPathType.Unknown;

        if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
            return PathTypeMap.TryGetValue(uri.Scheme, out var type) ? type : IOPathType.Unknown;

        if (path.StartsWith("./") ||
            path.StartsWith("../") ||
            path.StartsWith("..\\") ||
            path.StartsWith(".\\"))
            return IOPathType.LOCAL_RELATIVE;

        if (path.StartsWith("~/") ||
            path.StartsWith("~\\"))
            return IOPathType.LOCAL_RELATIVE_TILDE;

        try
        {
            //Last ditch attempt
            return File.Exists(path)
                ? IOPathType.LOCAL_RELATIVE
                : IOPathType.Unknown;
        }
        catch
        {
            return IOPathType.Unknown;
        }
    }

    /// <summary>
    /// Gets the file extension based on the mime type
    /// </summary>
    /// <param name="mimeType">The mime type</param>
    /// <returns>The file extension</returns>
    public static string DetermineExtension(string mimeType)
    {
        return MimeTypes.GetMimeTypeExtensions(mimeType).FirstOrDefault() ?? "zip";
    }

    /// <summary>
    /// Creates a random directory in the temp file paths
    /// </summary>
    /// <returns>The file path</returns>
    public static string RandomDirectory()
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        return path;
    }
}
