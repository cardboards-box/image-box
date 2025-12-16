using Jint.Native;

namespace ImageBox.Documentation;

internal class JsTypeService(
	IDocumentReflectionService _reflection) : IJsTypeService
{
	private readonly Dictionary<Type, string> _jsNames = [];

	public IReadOnlyDictionary<Type, string> DefaultTypes { get; } = new Dictionary<Type, string>
	{
		[typeof(string)] = "string",
		[typeof(char)] = "string",
		[typeof(bool)] = "boolean",
		[typeof(byte)] = "number",
		[typeof(sbyte)] = "number",
		[typeof(short)] = "number",
		[typeof(ushort)] = "number",
		[typeof(int)] = "number",
		[typeof(uint)] = "number",
		[typeof(long)] = "number",
		[typeof(ulong)] = "number",
		[typeof(float)] = "number",
		[typeof(double)] = "number",
		[typeof(decimal)] = "number",
		[typeof(object)] = "any",
		[typeof(void)] = "void",
		[typeof(JsValue)] = "any",
		[typeof(DateTime)] = "Date",
		[typeof(DateTimeOffset)] = "Date",
		[typeof(TimeSpan)] = "string",
		[typeof(Guid)] = "string",
		[typeof(Uri)] = "string",
		[typeof(Task)] = "Promise<void>",
		[typeof(ValueTask)] = "Promise<void>"
	};

	public void FillCache()
	{
		if (_jsNames.Count > 0) return;

		foreach (var (type, name) in DefaultTypes)
			_jsNames[type] = name;
	}

	public bool HandleCollections(IDocumentationSettings settings, Type type, [MaybeNullWhen(false)] out string name)
	{
		if (!type.IsCollection(out var elem))
		{
			name = null;
			return false;
		}

		name = _jsNames[type] = $"{TypeName(settings, elem)}[]";
		return true;
	}

	public bool HandleDictionaries(IDocumentationSettings settings, Type type, [MaybeNullWhen(false)] out string name)
	{
		if (!type.IsDictionary(out var keyType, out var valueType))
		{
			name = null;
			return false;
		}

		name = _jsNames[type] = $"{{ [key: {TypeName(settings, keyType)}]: {TypeName(settings, valueType)} }}";
		return true;
	}

	public bool HandleTasks(IDocumentationSettings settings, Type type, [MaybeNullWhen(false)] out string name)
	{
		if (!type.IsTask(out var arg))
		{
			name = null;
			return false;
		}

		if (arg is null)
		{
			name = _jsNames[type] = "Promise<void>";
			return true;
		}

		name = _jsNames[type] = $"Promise<{TypeName(settings, arg)}>";
		return true;
	}

	public string TypeName(IDocumentationSettings settings, Type type)
	{
		FillCache();

		if (Nullable.GetUnderlyingType(type) is Type underlying)
			return TypeName(settings, underlying);

		if (_jsNames.TryGetValue(type, out var name))
			return name;

		if (HandleDictionaries(settings, type, out name))
			return name;

		if (HandleCollections(settings, type, out name))
			return name;

		if (HandleTasks(settings, type, out name))
			return name;

		var doc = _reflection.Get(settings,type);
		return _jsNames[type] = doc.Name;
	}
}
