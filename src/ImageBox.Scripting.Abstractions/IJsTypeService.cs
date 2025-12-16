namespace ImageBox.Documentation;

/// <summary>
/// A service that converts C# types to JavaScript types
/// </summary>
public interface IJsTypeService
{
	/// <summary>
	/// The default types that shouldn't be added to interfaces
	/// </summary>
	IReadOnlyDictionary<Type, string> DefaultTypes { get; }

	/// <summary>
	/// Get the JS version of the type
	/// </summary>
	/// <param name="settings">The settings for the reflection service</param>
	/// <param name="type">The type to fetch</param>
	/// <returns>The JS version of the type</returns>
	string TypeName(IDocumentationSettings settings, Type type);
}

