namespace ImageBox.Elements.Base;

/// <summary>
/// Indicates the scope the element can be used in
/// </summary>
[Flags]
public enum ScopeType
{
    /// <summary>
    /// The element can be used in the top-level of the image-box file
    /// </summary>
    TopLevel = 1 << 0,
    /// <summary>
    /// The element can be used within the template element
    /// </summary>
    Template = 1 << 1,
    /// <summary>
    /// The element can ONLY be used within a custom parent element
    /// </summary>
    /// <remarks>
    /// This should be a stand-alone flag. 
    /// If the element can be used within any scope in the template use <see cref="Template"/>
    /// </remarks>
    CustomParent = 1 << 2,
}
