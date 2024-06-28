namespace ImageBox.Core.IOPath;

/// <summary>
/// A utility class for working with file and URI paths
/// </summary>
/// <param name="Value">The path to attach to</param>
[JsonConverter(typeof(IOPathSerializer))]
public record struct IOPath(string Value)
{
    private string? _safePath;
    private IOPathType? _type;

    /// <summary>
    /// The type of path
    /// </summary>
    public IOPathType Type => _type ??= IOPathHelper.DetermineType(Value);

    /// <summary>
    /// The path in a format that is safe for the current operating system
    /// </summary>
    public string OSSafe => _safePath ??= IOPathHelper.CurrentOsSafe(Value, Type);

    /// <summary>
    /// Whether or not the path is local to the current machine
    /// </summary>
    public bool Local => Type.HasFlag(IOPathType.Local);

    /// <summary>
    /// Whether or not the file exists at the given path
    /// </summary>
    /// <remarks>Remote paths will always return false</remarks>
    public readonly bool Exists => IOPathHelper.FileExists(this);

    /// <summary>
    /// Get the absolute path of the current path
    /// </summary>
    /// <param name="relativeTo">The optional root directory to start relative paths in</param>
    /// <returns>The absolute path safe for the current OS</returns>
    public IOPath GetAbsolute(string? relativeTo = null)
    {
        if (!Type.HasFlag(IOPathType.Local)) return this;

        var path = OSSafe;
        if (!string.IsNullOrWhiteSpace(relativeTo) &&
            Type.HasFlag(IOPathType.Relative))
            path = Path.Combine(relativeTo, path);

        return new IOPath(Path.GetFullPath(path));
    }

    /// <summary>
    /// Gets the current value of the path
    /// </summary>
    /// <returns>The value of the path</returns>
    public readonly override string ToString() => Value;

    /// <summary>
    /// Converts between an IOPath and a string
    /// </summary>
    /// <param name="path">The IOPath</param>
    public static implicit operator string(IOPath path) => path.Value;

    /// <summary>
    /// Converts between a string and an IOPath
    /// </summary>
    /// <param name="path">The string</param>
    public static implicit operator IOPath(string path) => new(path);
}
