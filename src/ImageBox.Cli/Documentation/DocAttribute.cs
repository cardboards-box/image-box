namespace ImageBox.Cli.Documentation;

public record class DocAttribute(
    string Type,
    string Name,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    string[]? Aliases,
    bool Required,
    bool Bindable,
    string Description);
