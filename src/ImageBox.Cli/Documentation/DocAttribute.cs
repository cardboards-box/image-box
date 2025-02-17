namespace ImageBox.Cli.Documentation;

public record class DocAttribute(
    string Type,
    string PropertyName,
    string Name,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    string[]? Aliases,
    bool Required,
    bool Bindable,
    string Description,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    string? Remarks,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    string? Example);
