namespace ImageBox.Cli.Documentation;

public record class TypeData(
    [property: JsonIgnore]
    Type Type,
    string Name,
    string FullName,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? Description = null,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    TypeOption[]? Options = null);
