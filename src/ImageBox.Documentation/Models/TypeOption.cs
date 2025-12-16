namespace ImageBox.Documentation.Models;

/// <summary>
/// Represents the various options related to a type (normally for enums)
/// </summary>
/// <param name="Name">The name of the option</param>
/// <param name="Value">The value of the option</param>
/// <param name="Description">The description of the option</param>
public record class TypeOption(
	string Name,
	int Value,
	[property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	string Description);