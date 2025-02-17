namespace ImageBox.Cli.Documentation;

public record class Docs(
    Dictionary<string, TypeData> Types,
    DocElement[] Elements);
