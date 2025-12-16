namespace ImageBox.Scripting.Renderers;

using Documentation;

internal class SizeUnitRenderer : IJsTypeRenderer
{
    public bool IsDefault => false;

    public bool CanRender(Type type)
    {
        return type == typeof(SizeUnit);
	}

    public string? ModuleName(Type type)
    {
        return null;
    }

    public IEnumerable<string> Render(Type type)
    {
        yield break;
    }

    public IEnumerable<string> RenderTypings(IDocumentationSettings settings, Type type, HashSet<Type> rendered)
    {
        var types = string.Join(" | ", SizeUnitHelper.Units().Select(t => $"'{t.Symbol}'"));
        yield return $"type SizeUnitUnits = {types};";
        yield return "export type SizeUnit = `${number}${SizeUnitUnits}`;";
    }
}
