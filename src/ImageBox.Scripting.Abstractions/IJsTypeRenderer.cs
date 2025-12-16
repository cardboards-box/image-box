using ImageBox.Documentation;

namespace ImageBox.Scripting;

/// <summary>
/// Renders the JavaScript type for a dotnet type
/// </summary>
public interface IJsTypeRenderer
{
	/// <summary>Whether or not this renderer can be used for the given type</summary>
	/// <param name="type">The type to render</param>
	/// <returns>Whether or not this renderer can be used for the given type</returns>
	bool CanRender(Type type);

	/// <summary>Whether or not this is the default renderer</summary>
	bool IsDefault { get; }

	/// <summary>
	/// Gets the module name for the given type
	/// </summary>
	/// <param name="type">The type to get the module name for</param>
	/// <returns>The module name for the given type</returns>
	string? ModuleName(Type type);

	/// <summary>
	/// Renders each line of the JavaScript type for the given dotnet type
	/// </summary>
	/// <param name="type">The type to render</param>
	/// <returns>The rendered JavaScript type</returns>
	IEnumerable<string> Render(Type type);

	/// <summary>
	/// Renders each line of the JavaScript type definitions for the given dotnet type
	/// </summary>
	/// <param name="settings">The settings to use for doing reflection activities</param>
	/// <param name="type">The type to render</param>
	/// <param name="rendered">The set of already rendered types</param>
	/// <returns>The rendered type definitions</returns>
	IEnumerable<string> RenderTypings(IDocumentationSettings settings, Type type, HashSet<Type> rendered);
}
