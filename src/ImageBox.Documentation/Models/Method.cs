namespace ImageBox.Documentation.Models;

/// <summary>
/// Represents a method on a class for documentation generation
/// </summary>
/// <param name="Name">The name of the method</param>
/// <param name="ReturnType">The return type of the method</param>
/// <param name="Parameters">The parameters of the method</param>
/// <param name="Async">Whether or not the method is asynchronous - returning a promise or a task</param>
/// <param name="Comments">The comments for the item</param>
/// <param name="Attributes">Any attributes that were requested in the config</param>
public record class Method(
	string Name,
	Type ReturnType,
	Property[] Parameters,
	bool Async,
	Comments Comments,
	Attribute[] Attributes);