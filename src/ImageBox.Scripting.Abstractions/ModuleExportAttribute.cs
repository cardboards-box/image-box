namespace ImageBox.Scripting;

/// <summary>
/// Marks a method or property as a publicly visible item on a <see cref="IScriptModule"/>
/// </summary>
/// <remarks>This will be included in the index.d.ts file generation</remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
public class ModuleExportAttribute(
	string? name = null,
	Type? type = null) : Attribute
{
	/// <summary>
	/// The optional name of the exported item
	/// </summary>
	/// <remarks>Defaults to the member name</remarks>
	public string? Name { get; } = name;

	/// <summary>
	/// The optional type of the exported item
	/// </summary>
	/// <remarks>Defaults to the member type</remarks>
	public Type? Type { get; } = type;
}

