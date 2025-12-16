using LoxSmoke.DocXml;

namespace ImageBox.Documentation;

using Models;

internal class DocumentationSettings : IDocumentationSettings
{
    internal DocXmlReader? _reader = null;
    internal readonly Dictionary<Type, Class> _typeCache = [];
    internal readonly Dictionary<Type, Comments> _commentCache = [];
    internal Assembly[]? _assemblyCache = null;

    internal Func<IEnumerable<Assembly>> _assemblies = EntryAssemblies;
    internal Func<Type, IEnumerable<Attribute>> _classAttributes = t => t.GetCustomAttributes();
    internal Func<PropertyInfo, bool> _propertyFilter = _ => true;
    internal Func<PropertyInfo, IEnumerable<Attribute>> _propertyAttributes = t => t.GetCustomAttributes();
    internal Func<MethodInfo, bool> _methodFilter = _ => true;
    internal Func<MethodInfo, IEnumerable<Attribute>> _methodAttributes = t => t.GetCustomAttributes();
    internal Func<ParameterInfo, IEnumerable<Attribute>> _methodParameterAttributes = t => t.GetCustomAttributes();

    public string IndentCharacter { get; private set; } = "\t";

	public IDocumentationSettings Assemblies(Func<IEnumerable<Assembly>> factory)
    {
        _assemblies = factory;
        return this;
    }

    public IDocumentationSettings Assemblies(IReadOnlyCollection<Assembly> assemblies) => Assemblies(() => assemblies);

    public IDocumentationSettings AllAssemblies() => Assemblies(EntryAssemblies);

    public IDocumentationSettings ClassAttributes(Func<Type, IEnumerable<Attribute>> factory)
    {
        _classAttributes = factory;
        return this;
    }

    public IDocumentationSettings ClassAttributes(params Type[] attributeTypes)
    {
        return ClassAttributes(t => t
            .GetCustomAttributes()
            .Where(t => attributeTypes.Contains(t.GetType())));
    }

    public IDocumentationSettings ClassAttribute<T>()
        where T : Attribute
    {
        return ClassAttributes(typeof(T));
	}

	public IDocumentationSettings Indent(string indentation)
	{
		IndentCharacter = indentation;
        return this;
	}

	public IDocumentationSettings PropertyFilter(Func<PropertyInfo, bool> filter)
    {
        _propertyFilter = filter;
        return this;
    }

    public IDocumentationSettings PropertyAttributes(Func<PropertyInfo, IEnumerable<Attribute>> factory)
    {
        _propertyAttributes = factory;
        return this;
    }

    public IDocumentationSettings PropertyAttributes(params Type[] attributeTypes)
    {
        return PropertyAttributes(t => t
            .GetCustomAttributes()
            .Where(t => attributeTypes.Contains(t.GetType())));
    }

    public IDocumentationSettings PropertyAttribute<T>()
        where T : Attribute
    {
        return PropertyAttributes(typeof(T));
    }

    public IDocumentationSettings MethodFilter(Func<MethodInfo, bool> filter)
    {
        _methodFilter = filter;
        return this;
    }

    public IDocumentationSettings MethodAttributes(Func<MethodInfo, IEnumerable<Attribute>> factory)
    {
        _methodAttributes = factory;
        return this;
    }

    public IDocumentationSettings MethodAttributes(params Type[] attributeTypes)
    {
        return MethodAttributes(t => t
            .GetCustomAttributes()
            .Where(t => attributeTypes.Contains(t.GetType())));
    }

    public IDocumentationSettings MethodAttribute<T>()
        where T : Attribute
    {
        return MethodAttributes(typeof(T));
    }

    public IDocumentationSettings ParameterAttributes(Func<ParameterInfo, IEnumerable<Attribute>> factory)
    {
        _methodParameterAttributes = factory;
        return this;
    }

    public IDocumentationSettings ParameterAttributes(params Type[] attributeTypes)
    {
        return ParameterAttributes(t => t
            .GetCustomAttributes()
            .Where(t => attributeTypes.Contains(t.GetType())));
    }

    public IDocumentationSettings ParameterAttribute<T>()
        where T : Attribute
    {
        return ParameterAttributes(typeof(T));
    }

    private static IEnumerable<Assembly> EntryAssemblies()
    {
        var resolved = new HashSet<string>();

        Queue<Assembly> toResolve = [];

        toResolve.Enqueue(Assembly.GetExecutingAssembly());
        toResolve.Enqueue(Assembly.GetCallingAssembly());
        var entry = Assembly.GetEntryAssembly();
        if (entry is not null) toResolve.Enqueue(entry);

        while (toResolve.TryDequeue(out var current))
        {
            if (string.IsNullOrEmpty(current.FullName) ||
                !resolved.Add(current.FullName)) continue;

            yield return current;

            var references = current.GetReferencedAssemblies();
            foreach (var reference in references)
            {
                try
                {
                    var asm = Assembly.Load(reference);
                    toResolve.Enqueue(asm);
                }
                catch { }
            }
        }
    }
}
