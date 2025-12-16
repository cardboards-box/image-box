namespace ImageBox.Documentation.Models;

/// <summary>
/// Represents XML comments for documentation generation
/// </summary>
/// <param name="Summary">The summary comment for the item</param>
/// <param name="Remarks">Any remark comments for the item</param>
/// <param name="Example">Any example comments for the item</param>
/// <param name="Returns">Any return comments for the item (only filled if it's a method)</param>
/// <param name="Options">Any type options for the item</param>
public record class Comments(
	string Summary,
	string? Remarks,
	string? Example,
	string? Returns,
	TypeOption[] Options);