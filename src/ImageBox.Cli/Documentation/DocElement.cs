namespace ImageBox.Cli.Documentation;

using Elements;

public record class DocElement(
    string Type,
    string Tag,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    string[]? Aliases,
    ElementType ContentType,
    ScopeType Scope,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    string[]? ValidParents,
    DocAttribute[] Attributes,
    string Description,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    string? Remarks,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    string? Example);
