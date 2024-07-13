namespace ImageBox.Services;

using Ast;

/// <summary>
/// Represents the boxed image data loaded from a file
/// </summary>
public class LoadedAst
{
    /// <summary>
    /// The directory the file was loaded from
    /// </summary>
    public required string WorkingDirectory { get; init; }

    /// <summary>
    /// The name of the file in the <see cref="WorkingDirectory"/>
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// All of the AST elements in the template
    /// </summary>
    public required AstElement[] SyntaxTree { get; init; } = [];

    /// <summary>
    /// Any files or directories that need to be cleaned up after the image is rendered
    /// </summary>
    public List<string> Cleanup { get; } = [];
}
