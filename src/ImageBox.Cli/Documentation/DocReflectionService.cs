using LoxSmoke.DocXml;

namespace ImageBox.Cli.Documentation;

using Elements.Attributes;
using Elements.Base;
using Services;
using Services.Loading;

public interface IDocReflectionService
{
    Docs GetDocs();
}

internal class DocReflectionService(
    IElementReflectionService _reflection,
    IEnumerable<IElement> _elements) : IDocReflectionService
{
    public DocXmlReader GetReader()
    {
        var scripts = _reflection.GetAllOfType<IScriptItem>().Select(t => t.Assembly);
        var assemblies = _elements
            .Select(t => t.GetType().Assembly)
            .Concat(scripts)
            .Distinct();
        return new DocXmlReader(assemblies);
    }

    public static TypeData GetTypeData(Type type, DocXmlReader reader, Dictionary<string, TypeData> types)
    {
        var fullName = type.FullName ?? type.Name;
        if (types.TryGetValue(fullName, out var output))
            return output;

        var description = reader.GetTypeComments(type);
        var options = !type.IsEnum ? null : reader.GetEnumComments(type, true)
            .ValueComments
            .Select(t => new TypeOption(t.Name, t.Value, t.Summary))
            .ToArray();

        return types[fullName] = new TypeData(type, type.Name, fullName, description.Summary, options);
    }

    public static IEnumerable<DocAttribute> GetAttributes(Type type, DocXmlReader reader, Dictionary<string, TypeData> types)
    {
        var binder = typeof(AstValue<>);
        var props = type.GetProperties();
        foreach (var prop in props)
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

            var comments = reader.GetMemberComments(prop);
            var aliases = attributes.Skip(1).Select(t => t.Name).ToArray();
            var required = attributes.Any(t => t.Required);
            yield return new DocAttribute(
                GetTypeData(propType, reader, types).FullName,
                prop.Name,
                first.Name,
                aliases.Length == 0 ? null : aliases,
                required,
                bindable,
                comments.Summary,
                comments.Remarks?.ForceNull(),
                comments.Example?.ForceNull());
        }
    }

    public IEnumerable<DocElement> GetElements(DocXmlReader reader, Dictionary<string, TypeData> types)
    {
        var children = typeof(IParentElement);
        var value = typeof(IValueElement);
        foreach (var element in _elements)
        {
            var type = element.GetType();
            var elements = type.GetCustomAttributes<AstElementAttribute>().ToArray();
            if (elements is null || elements.Length == 0) continue;

            var description = reader.GetTypeComments(type);
            var first = elements.First();
            var aliases = elements.Skip(1).Select(t => t.Tag).ToArray();
            var attributes = GetAttributes(type, reader, types).ToArray();
            var contentType = children.IsAssignableFrom(type)
                ? ElementType.Children
                : value.IsAssignableFrom(type)
                    ? ElementType.Value
                    : ElementType.SelfClosing;
            string[]? validParents = first.Scope != ScopeType.CustomParent ? null :
                first.ParentTypes.SelectMany(t => t.GetCustomAttributes<AstElementAttribute>()).Select(t => t.Tag).ToArray();
            yield return new DocElement(
                GetTypeData(type, reader, types).FullName,
                first.Tag,
                aliases.Length == 0 ? null : aliases,
                contentType,
                first.Scope,
                validParents,
                attributes,
                description.Summary,
                description.Remarks?.ForceNull(),
                description.Example?.ForceNull());
        }
    }

    public Docs GetDocs()
    {
        var types = new Dictionary<string, TypeData>();
        var reader = GetReader();
        var elements = GetElements(reader, types)
            .OrderBy(t => t.Tag)
            .OrderBy(t => t.Scope)
            .ToArray();
        return new Docs(
            types, 
            elements);
    }
}
