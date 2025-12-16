namespace ImageBox.Documentation.Models;

/// <summary>
/// Represents a class for documentation generation
/// </summary>
/// <param name="Name">The name of the class</param>
/// <param name="Type">The type of the class</param>
/// <param name="Properties">The properties on the class</param>
/// <param name="Methods">The methods on the class</param>
/// <param name="Comments">The comments on the class</param>
/// <param name="Attributes">Any attributes that were requested in the config</param>
public record class Class(
	string Name,
	Type Type,
	Property[] Properties,
	Method[] Methods,
	Comments Comments,
	Attribute[] Attributes);