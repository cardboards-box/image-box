namespace ImageBox.Scripting;

/// <summary>
/// Represents a reflected enum
/// </summary>
/// <param name="Name">The name of the enum</param>
/// <param name="Type">The type of the enum</param>
/// <param name="Values">The values of the enum</param>
public record class EnumDescription(
	string Name,
	Type Type,
	EnumValue[] Values);