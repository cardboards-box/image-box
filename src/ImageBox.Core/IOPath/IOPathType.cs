namespace ImageBox.Core.IOPath;

/// <summary>
/// Represents the type of path
/// </summary>
[Flags]
public enum IOPathType : ushort
{
    /// <summary>
    /// Could not determine the path type
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// Indicates a local file path
    /// </summary>
    /// <remarks>Incompatible with <see cref="Remote"/></remarks>
    Local = 1,
    /// <summary>
    /// Indicates a remote file path
    /// </summary>
    /// <remarks>Incompatible with <see cref="Local"/></remarks>
    Remote = 2,
    /// <summary>
    /// Indicates a HTTP or HTTPS scheme
    /// </summary>
    /// <remarks>Incompatible with <see cref="Ftp"/></remarks>
    Http = 4,
    /// <summary>
    /// Indicates a FTP or FTPS scheme
    /// </summary>
    /// <remarks>Incompatible with <see cref="Http"/></remarks>
    Ftp = 8,
    /// <summary>
    /// Indicates the path is relative
    /// </summary>
    Relative = 16,
    /// <summary>
    /// Indicates the path is absolute
    /// </summary>
    Absolute = 32,
    /// <summary>
    /// Indicates the path starts with a tilde
    /// </summary>
    Tilde = 64,
    /// <summary>
    /// Indicates the path is for a remote-source cache
    /// </summary>
    Cache = 128,

    /// <summary>
    /// Path resolves to a local file relative to the base URI
    /// </summary>
    LOCAL_RELATIVE = Local | Relative,
    /// <summary>
    /// Path resolves to a local file relative to the base URI with a tilde
    /// </summary>
    LOCAL_RELATIVE_TILDE = Local | Relative | Tilde,
    /// <summary>
    /// Path resolves to a local file with an absolute path
    /// </summary>
    LOCAL_ABSOLUTE = Local | Absolute,
    /// <summary>
    /// Path resolves to a remote file to be accessed over HTTP or HTTPS
    /// </summary>
    REMOTE_HTTP = Remote | Http | Absolute,
    /// <summary>
    /// Path resolves to a remote file to be accessed over FTP or FTPS
    /// </summary>
    /// <remarks>Not implemented yet</remarks>
    REMOTE_FTP = Remote | Ftp | Absolute,
    /// <summary>
    /// Path resolves to a remote-source cache key
    /// </summary>
    CACHE = Cache | Absolute
}
