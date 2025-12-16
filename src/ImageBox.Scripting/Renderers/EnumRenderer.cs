namespace ImageBox.Scripting.Renderers;

using Documentation;

internal class EnumRenderer(
    IEnumReflectionService _reflection,
    IDocumentReflectionService _document) : IJsTypeRenderer
{
    public bool CanRender(Type type) => type.IsEnum;

	public bool IsDefault => false;

    public string? ModuleName(Type type)
    {
        return type.GetCustomAttribute<ModuleAttribute>()?.Name;
	}

    public IEnumerable<string> Render(Type type)
    {
        var description = _reflection.Describe(type);
        yield return $"export class {description.Name} {{";
		foreach (var value in description.Values)
			yield return $"\tstatic get {value.Name}() {{ return {value.Value}; }}";
		yield return "}";
	}

    public IEnumerable<string> RenderTypings(IDocumentationSettings settings, Type type, HashSet<Type> rendered)
    {
		if (!rendered.Add(type)) yield break;

		var desc = _document.Enum(settings, type);
		if (!string.IsNullOrEmpty(desc.Summary))
		{
			var remarks = string.IsNullOrEmpty(desc.Remarks)
				? string.Empty
				: $" - {desc.Remarks}";
			yield return $"/** {desc.Summary}{remarks} */";
		}

		yield return $"declare class {type.Name} {{";

		foreach (var value in desc.Options)
		{
			if (!string.IsNullOrEmpty(value.Description))
			{
				yield return $"{settings.IndentCharacter}/**";
				yield return $"{settings.IndentCharacter} * {value.Description} - {value.Value}";
				yield return $"{settings.IndentCharacter} * @type {{number}}";
				yield return $"{settings.IndentCharacter} */";
			}

			yield return $"{settings.IndentCharacter}static get {value.Name}(): number;";
		}

		yield return "}";
	}
}
