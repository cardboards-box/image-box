namespace ImageBox.Scripting;

/// <summary>
/// Represents the value in an enum
/// </summary>
/// <param name="Name">The name of the enum</param>
/// <param name="Value">The value of the enum</param>
/// <param name="Type">The type of the enum</param>
public record class EnumValue(
	string Name,
	long Value,
	Type Type);