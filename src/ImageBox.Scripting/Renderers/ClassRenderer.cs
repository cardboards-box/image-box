namespace ImageBox.Scripting.Renderers;

using Documentation;
using Documentation.Models;

internal class ClassRenderer(
    IJsTypeService _types,
    IDocumentReflectionService _document) : IJsTypeRenderer
{
    public bool IsDefault => true;

    public bool CanRender(Type type) => true;

    public string? ModuleName(Type type)
	{
		return type.GetCustomAttribute<ModuleAttribute>()?.Name;
	}

    public IEnumerable<string> Render(Type type)
    {
        yield break;
    }

    public IEnumerable<string> RenderTypings(IDocumentationSettings settings, Type type, HashSet<Type> rendered)
    {
		if (!rendered.Add(type)) yield break;

		yield return $"// Type definitions for {type.FullName ?? type.Name}";
		var item = _document.Get(settings, type);
		var allTypes = ExplodeClass(settings, type, rendered);
		foreach (var t in allTypes)
			foreach(var line in WriteInterface(settings, t, 1))
				yield return line;

		foreach (var line in WriteInterface(settings, type, 1))
			yield return line;

		yield return string.Empty;
		yield return Scope(settings, $"export var {item.Name}: {item.Name};", 1);
	}

	public IEnumerable<string> WriteInterface(IDocumentationSettings settings, Type type, int indent)
	{
		var def = _document.Get(settings, type);

		foreach (var line in Class(def))
			yield return Scope(settings, line, indent);

		foreach (var prop in def.Properties)
			foreach (var line in Property(settings, prop))
				yield return Scope(settings, line, indent + 1);

		foreach (var method in def.Methods)
			foreach (var line in Method(settings, method))
				yield return Scope(settings, line, indent + 1);

		yield return Scope(settings, "}", indent);
	}

	public static string Scope(IDocumentationSettings settings, string line, int indent)
	{
		if (indent <= 1) return line;
		return string.Join("", Enumerable.Repeat(settings.IndentCharacter, indent - 1)) + line;
	}

	public IEnumerable<Type> YieldTypes(IDocumentationSettings settings, Type type, HashSet<Type> found)
	{
		if (Nullable.GetUnderlyingType(type) is Type underlying)
			type = underlying;

		//Skip types that have already been found
		if (found.Contains(type)) yield break;
		//Skip default types
		if (_types.DefaultTypes.ContainsKey(type)) yield break;
		//Enums have no children - yield the current and skip rest
		if (type.IsEnum)
		{
			found.Add(type);
			yield return type;
			yield break;
		}
		//Handle dictionaries
		if (type.IsDictionary(out var key, out var val))
		{
			foreach (var res in YieldTypes(settings, key, found))
				yield return res;
			foreach (var res in YieldTypes(settings, val, found))
				yield return res;
			yield break;
		}
		//Handle collections
		if (type.IsCollection(out var arg))
		{
			foreach (var res in YieldTypes(settings, arg, found))
				yield return res;
			yield break;
		}
		//Handle tasks
		if (type.IsTask(out var taskArg))
		{
			if (taskArg is null) yield break;
			foreach (var res in YieldTypes(settings, taskArg, found))
				yield return res;
			yield break;
		}
		//Found unrestricted type
		found.Add(type);
		foreach (var explode in ExplodeClass(settings, type, found))
			yield return explode;

		yield return type;
	}

	public IEnumerable<Type> ExplodeClass(IDocumentationSettings settings, Type type, HashSet<Type> found)
	{
		var item = _document.Get(settings, type);
		var props = item.Properties
				.Select(t => t.Type)
				.Distinct()
				.SelectMany(t => YieldTypes(settings, t, found));
		var methods = item.Methods
			.SelectMany(t => t.Parameters
				.Select(t => t.Type)
				.Append(t.ReturnType)
				.Distinct()
				.SelectMany(t => YieldTypes(settings, t, found)));
		return methods.Concat(props);
	}

	public IEnumerable<string> Property(IDocumentationSettings settings, Property property)
	{
		var type = _types.TypeName(settings, property.Type);
		var nullable = property.Nullable ? "?" : string.Empty;
		var ro = property.ReadOnly ? "readonly " : string.Empty;

		if (!string.IsNullOrEmpty(property.Comments.Summary))
		{
			var remarks = string.IsNullOrEmpty(property.Comments.Remarks)
				? string.Empty
				: $" - {property.Comments.Remarks}";
			yield return $"/** {property.Comments.Summary}{remarks} */";
		}

		yield return $"{ro}{property.Name}{nullable}: {type};";
	}

	public IEnumerable<string> Method(IDocumentationSettings settings, Method method)
	{
		string Declaration(string type)
		{
			var bob = new StringBuilder();
			bob.Append(method.Name);
			bob.Append('(');

			for (var i = 0; i < method.Parameters.Length; i++)
			{
				if (i != 0) bob.Append(", ");
				var param = method.Parameters[i];
				var paramType = _types.TypeName(settings, param.Type);
				var nullable = param.Nullable ? "?" : string.Empty;
				var isSpread = param.ArrayParam ? "..." : string.Empty;
				bob.Append($"{isSpread}{param.Name}{nullable}: {paramType}");
			}

			bob.Append("): ");
			bob.Append(type);
			bob.Append(';');
			return bob.ToString();
		}

		var type = _types.TypeName(settings, method.ReturnType);

		yield return $"/**";
		var remarks = string.IsNullOrEmpty(method.Comments.Remarks)
			? string.Empty
			: $" - {method.Comments.Remarks}";
		yield return $" * {method.Comments.Summary}{remarks}";

		foreach (var par in method.Parameters)
		{
			var paramType = _types.TypeName(settings, par.Type);
			var name = par.Name;
			if (par.Nullable)
				name = $"[{name}]";
			yield return $" * @param {{{paramType}}} {name} {par.Comments.Summary}";
		}

		var returns = string.IsNullOrEmpty(method.Comments.Returns)
			? string.Empty
			: $" {method.Comments.Returns}";
		yield return $" * @returns {{{type}}}{returns}";
		yield return $" */";
		yield return Declaration(type);
	}

	public static IEnumerable<string> Class(Class item)
	{
		if (!string.IsNullOrEmpty(item.Comments.Summary))
		{
			var remarks = string.IsNullOrEmpty(item.Comments.Remarks)
				? string.Empty
				: $" - {item.Comments.Remarks}";
			yield return $"/** {item.Comments.Summary}{remarks} */";
		}

		yield return $"interface {item.Name} {{";
	}
}
