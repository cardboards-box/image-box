namespace ImageBox.Elements;

/// <summary>
/// Indicates that the class is available as a custom element in the drawing abstract syntax tree
/// </summary>
/// <param name="tag">The name of the tag this element represents</param>
/// <param name="scope">The scope the element can be used in</param>
/// <param name="parentTypes">The type of parent elements this element can be a child of</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class AstElementAttribute(string tag, ScopeType scope, params Type[] parentTypes) : Attribute
{
    /// <summary>
    /// The name of the element in the AST
    /// </summary>
    public string Tag { get; } = tag;

    /// <summary>
    /// The scope the element can be used in
    /// </summary>
    /// <remarks>This isn't validated - it's mostly used for documentation purposes</remarks>
    public ScopeType Scope { get; } = scope;

    /// <summary>
    /// The type of parent elements this element can be a child of
    /// </summary>
    /// <remarks>This should only be used if <see cref="Scope"/> is set to <see cref="ScopeType.CustomParent"/></remarks>
    public Type[] ParentTypes { get; } = parentTypes;
}