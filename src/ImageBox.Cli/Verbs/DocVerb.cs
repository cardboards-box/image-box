using LoxSmoke.DocXml;

namespace ImageBox.Cli.Verbs;

using Elements.Base;
using Elements.Attributes;
using Services;
using Services.Loading;

[Verb("doc", HelpText = "Generates documentation for the various aspects of image-box")]
public class DocVerbOptions
{
    public const string DEFAULT_FILE_NAME = "image-box-docs.json";

    [Option('o', "output", HelpText = "The file to write the documentation json to", Default = DEFAULT_FILE_NAME)]
    public string Output { get; set; } = DEFAULT_FILE_NAME;
}

internal class DocVerb(
    ILogger<DocVerb> logger,
    IElementReflectionService _reflection,
    IEnumerable<IElement> _elements) : BooleanVerb<DocVerbOptions>(logger)
{
    private readonly JsonSerializerOptions _indented = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    private Type[]? _scripts;

    public Type[] ScriptTypes => _scripts ??= _reflection.GetAllOfType<IScriptItem>().ToArray();

    public DocXmlReader GetReader()
    {
        var assemblies = _elements
            .Select(t => t.GetType().Assembly)
            .Concat(ScriptTypes.Select(t => t.Assembly))
            .Distinct();
        return new DocXmlReader(assemblies);
    }

    public static TypeData GetTypeData(Type type, DocXmlReader reader)
    {
        var description = reader.GetTypeComments(type);
        var options = !type.IsEnum ? null : reader.GetEnumComments(type, true)
            .ValueComments
            .Select(t => new TypeOption(t.Name, t.Value, t.Summary))
            .ToArray();

        return new TypeData(type.Name, type.FullName, description.Summary, options);
    }

    public static IEnumerable<DocAttribute> GetAttributes(Type type, DocXmlReader reader)
    {
        var binder = typeof(AstValue<>);
        var props = type.GetProperties();
        foreach(var prop in props)
        {
            var attributes = prop.GetCustomAttributes<AstAttributeAttribute>().ToArray();
            if (attributes is null || attributes.Length == 0) continue;

            var propType = prop.PropertyType;
            propType = Nullable.GetUnderlyingType(propType) ?? propType;
            var bindable = false;

            if (propType.IsGenericType && propType.GetGenericTypeDefinition() == binder)
            {
                propType = propType.GetGenericArguments()[0];
                bindable = true;
            }

            propType = Nullable.GetUnderlyingType(propType) ?? propType;
            var first = attributes.First();

            if (first.EnumType is not null)
                propType = first.EnumType;

            var description = reader.GetMemberComment(prop);
            var aliases = attributes.Skip(1).Select(t => t.Name).ToArray();
            var required = attributes.Any(t => t.Required);
            yield return new DocAttribute(
                GetTypeData(propType, reader),
                first.Name,
                aliases,
                required,
                bindable,
                description);
        }
    }

    public IEnumerable<DocElement> GetElements(DocXmlReader reader)
    {
        var children = typeof(IParentElement);
        foreach(var element in _elements)
        {
            var type = element.GetType();
            var elements = type.GetCustomAttributes<AstElementAttribute>().ToArray();
            if (elements is null || elements.Length == 0) continue;

            var description = reader.GetTypeComments(type);
            var first = elements.First();
            var aliases = elements.Skip(1).Select(t => t.Tag).ToArray();
            var attributes = GetAttributes(type, reader).ToArray();
            var hasChildren = children.IsAssignableFrom(type);
            yield return new DocElement(
                GetTypeData(type, reader),
                first.Tag,
                aliases,
                hasChildren,
                attributes,
                description.Summary);
        }
    }

    public override async Task<bool> Execute(DocVerbOptions options, CancellationToken token)
    {
        var reader = GetReader();

        var dir = Path.GetDirectoryName(options.Output);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var elements = GetElements(reader).ToArray();
        var doc = new Documentation(elements);

        await using var io = File.Create(options.Output);
        await JsonSerializer.SerializeAsync(io, doc, _indented, token);
        _logger.LogInformation("Wrote documentation to {Output}", options.Output);
        return true;
    }

    public record class DocAttribute(
        TypeData Type,
        string Name,
        string[] Aliases,
        bool Required,
        bool Bindable,
        string Description);

    public record class DocElement(
        TypeData Type,
        string Tag,
        string[] Aliases,
        bool HasChildren,
        DocAttribute[] Attributes,
        string Description);

    public record class Documentation(
        DocElement[] Elements);

    public record class TypeOption(
        string Name,
        int Value,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        string Description);

    public record class TypeData(
        string Name,
        string? FullName,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
        string? Description = null,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        TypeOption[]? Options = null);
}
