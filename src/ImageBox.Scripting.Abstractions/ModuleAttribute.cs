namespace ImageBox.Scripting;

/// <summary>
/// Indicates that a <see cref="IScriptModule"/> has a custom name that can be used to import into a box-script
/// </summary>
/// <param name="name">The unique name or relative path to the module</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ModuleAttribute(
	string name) : Attribute
{
	/// <summary>
	/// The unique name or relative path to the module
	/// </summary>
	public string Name { get; } = name;
}
