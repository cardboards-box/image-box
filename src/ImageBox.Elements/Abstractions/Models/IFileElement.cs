namespace ImageBox.Elements;

/// <summary>
/// Represents an element that has a src parameter
/// </summary>
public interface IFileElement : IPositionElement
{
    /// <summary>
    /// The file's source
    /// </summary>
    AstValue<IOPath> Source { get; }

    /// <summary>
    /// The optional User-Agent header for fetching the file
    /// </summary>
    AstValue<string?> UserAgent { get; }

    /// <summary>
    /// The optional Accepts header for fetching the file
    /// </summary>
    AstValue<string?> Accepts { get; }

    /// <summary>
    /// Indicates whether or not the file should be cached locally
    /// </summary>
    AstValue<bool?> ShouldCache { get; }
}
