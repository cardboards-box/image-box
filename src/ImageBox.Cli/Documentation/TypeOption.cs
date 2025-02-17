namespace ImageBox.Cli.Documentation;

public record class TypeOption(
    string Name,
    int Value,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    string Description);
