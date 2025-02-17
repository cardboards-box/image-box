namespace ImageBox.Cli.Documentation;

public interface IMarkdownService
{
    Task Render(StreamWriter writer, Docs docs);
}

internal partial class MarkdownService : IMarkdownService
{
    private readonly Dictionary<string, string> _references = [];
    private const string TAB = "  ";

    public async Task RenderTableOfContents(StreamWriter writer, Docs docs)
    {
        await writer.WriteLineAsync("# Table of Contents");
        await writer.WriteLineAsync();
        await writer.WriteLineAsync("| Type | Name | Description |");
        await writer.WriteLineAsync("| ---- | ---- | ----------- |");

        foreach(var element in docs.Elements)
        {
            await writer.WriteLineAsync($"| [Element](#{LinkElement(element)}) | `{element.Tag}` | {element.Description} |");
        }

        foreach (var (key, type) in docs.Types)
        {
            var hasElement = _references.TryGetValue(key, out var link) && link.StartsWith("element-");
            if (hasElement) continue;

            await writer.WriteLineAsync($"| [Type](#{LinkType(type)}) | `{type.Name}` | {type.Description} |");
        }

        await writer.WriteLineAsync();
        await writer.WriteLineAsync();
    }

    public static string LinkType(TypeData data)
    {
        return $"type-{data.FullName.Replace(".", "-").Replace("[]", "-array")}";
    }

    public static string LinkElement(DocElement element)
    {
        return $"element-{element.Tag}";
    }

    public static string LinkAttribute(DocElement element, DocAttribute attribute)
    {
        return $"attribute-{element.Tag}-{attribute.Name}";
    }

    public void BuildReferences(Docs docs)
    {
        if (_references.Count > 0) return;

        foreach (var (_, type) in docs.Types)
            _references[type.FullName] = LinkType(type);

        foreach (var element in docs.Elements)
        {
            var type = docs.Types[element.Type];

            _references[type.FullName] = LinkElement(element);
            foreach (var attribute in element.Attributes)
            {
                var fullName = type.FullName + "." + attribute.PropertyName;
                _references[fullName] = LinkAttribute(element, attribute);

                var property = type.Type.GetProperty(attribute.PropertyName);
                if (property is null || property.DeclaringType is null) continue;

                fullName = property.DeclaringType.FullName + "." + attribute.PropertyName;
                _references[fullName] = LinkAttribute(element, attribute);
            }
        }
    }

    public string HandleReferences(string text, Docs docs)
    {
        BuildReferences(docs);

        if (string.IsNullOrEmpty(text) ||
            !text.ContainsIc("<see ")) return text;

        var match = SeeRefRegex().Matches(text);
        foreach (Match m in match)
        {
            var item = m.Groups[0].Value;
            var cref = m.Groups[1].Value;
            var propName = string.Join(":", cref.Split(':').Skip(1));
            var actualName = propName.Split('.').Last();

            if (_references.TryGetValue(propName, out var link))
                text = text.Replace(item, $"[{actualName}](#{link})");
            else
                text = text.Replace(item, $"`{actualName}`");
        }

        return text;
    }

    public async Task RenderExample(StreamWriter writer, DocElement element)
    {
        await writer.WriteLineAsync("```html");

        var sameLineCount = 2;

        if (element.Attributes.Length <= 1 && element.ContentType == ElementType.SelfClosing)
        {
            await writer.WriteAsync($"<{element.Tag} ");
            if (element.Attributes.Length == 1)
                await writer.WriteAsync($"{element.Attributes[0].Name}=\"\" ");
            await writer.WriteLineAsync("/>");
            await writer.WriteLineAsync("```");
            await writer.WriteLineAsync();
            return;
        }

        var oneLine = element.Attributes.Length <= sameLineCount;
        await writer.WriteAsync($"<{element.Tag} ");
        if (!oneLine)
            await writer.WriteLineAsync();

        foreach (var attribute in element.Attributes)
        {
            if (!oneLine) await writer.WriteAsync(TAB);
            await writer.WriteAsync($"{attribute.Name}=\"\" ");
            if (!oneLine) await writer.WriteLineAsync();
        }

        switch (element.ContentType)
        {
            case ElementType.SelfClosing:
                await writer.WriteLineAsync("/>");
                break;
            case ElementType.Value:
                await writer.WriteLineAsync($">");
                await writer.WriteLineAsync($"{TAB}<!-- TEXT VALUE HERE -->");
                await writer.WriteLineAsync($"</{element.Tag}>");
                break;
            case ElementType.Children:
                await writer.WriteLineAsync(">");
                await writer.WriteLineAsync($"{TAB}<!-- CHILDREN HERE -->");
                await writer.WriteLineAsync($"</{element.Tag}>");
                break;
        }

        await writer.WriteLineAsync("```");
        await writer.WriteLineAsync();
    }

    public async Task RenderElement(StreamWriter writer, DocElement element, Docs docs)
    {
        await writer.WriteLineAsync($"<a name=\"{LinkElement(element)}\"></a>");
        await writer.WriteLineAsync($"## Element: `<{element.Tag}>`");
        await writer.WriteLineAsync(HandleReferences(element.Description, docs));
        await writer.WriteLineAsync();

        if (!string.IsNullOrEmpty(element.Remarks))
        {
            await writer.WriteLineAsync("**Remarks**:<br>");
            await writer.WriteLineAsync(HandleReferences(element.Remarks, docs));
            await writer.WriteLineAsync();
        }

        await writer.WriteLineAsync("**Example**:<br>");
        if (!string.IsNullOrEmpty(element.Example))
        {
            await writer.WriteLineAsync(HandleReferences(element.Example, docs));
            await writer.WriteLineAsync();
        }

        await RenderExample(writer, element);

        if (element.Attributes.Length == 0)
            return;

        await writer.WriteLineAsync($"### Attributes");
        await writer.WriteLineAsync();

        foreach (var attribute in element.Attributes)
        {
            await writer.WriteLineAsync($"<a name=\"{LinkAttribute(element, attribute)}\"></a>");
            await writer.WriteLineAsync($"__*{attribute.Name}*__: {HandleReferences(attribute.Description, docs)}<br>");

            var type = docs.Types[attribute.Type];

            await writer.WriteLineAsync($"Type: [{attribute.Type}](#{LinkType(type)}).");
            await writer.WriteLineAsync($"This attribute is {(attribute.Required ? "**Required**" : "optional")}.");
            await writer.WriteLineAsync($"This attribute {(attribute.Bindable ? "can" : "cannot")} be bound to a runtime variable.");

            if (attribute.Aliases is not null && attribute.Aliases.Length > 0)
                await writer.WriteLineAsync($"You can use the following aliases: {string.Join(", ", attribute.Aliases.Select(t => $"`{t}`"))}.");

            if (!string.IsNullOrEmpty(attribute.Remarks))
                await writer.WriteLineAsync("*Remarks*: " + HandleReferences(attribute.Remarks, docs));

            if (!string.IsNullOrEmpty(attribute.Example))
                await writer.WriteLineAsync("*Example*: " + HandleReferences(attribute.Example, docs));
            await writer.WriteLineAsync();
        }

        await writer.WriteLineAsync();
    }

    public async Task RenderType(StreamWriter writer, TypeData type, Docs docs)
    {
        await writer.WriteLineAsync($"<a name=\"{LinkType(type)}\"></a>");
        await writer.WriteLineAsync($"### Type: `{type.Name}`");
        await writer.WriteLineAsync($"Full Name: {type.FullName}<br>");

        if (!string.IsNullOrEmpty(type.Description))
        {
            await writer.WriteLineAsync("Description:");
            await writer.WriteLineAsync(HandleReferences(type.Description, docs));
        }

        if (type.Options is null || type.Options.Length == 0)
            return;

        await writer.WriteLineAsync("**Enum Options**:<br>");
        await writer.WriteLineAsync("| Name | Description | Value |");
        await writer.WriteLineAsync("| ---- | ----------- | ----- |");

        foreach (var option in type.Options)
        {
            await writer.WriteLineAsync($"| `{option.Name}` | {HandleReferences(option.Description, docs)} | {option.Value} |");
        }

        await writer.WriteLineAsync();
    }

    public async Task Render(StreamWriter writer, Docs docs)
    {
        await RenderTableOfContents(writer, docs);
        await writer.WriteLineAsync();

        await writer.WriteLineAsync("# Elements / Tags");
        foreach (var element in docs.Elements)
            await RenderElement(writer, element, docs);

        await writer.WriteLineAsync("# Types");
        foreach (var (key, type) in docs.Types)
        {
            var hasElement = _references.TryGetValue(key, out var link) && link.StartsWith("element-");
            if (hasElement) continue;

            await RenderType(writer, type, docs);
        }
        await writer.FlushAsync();
    }

    [GeneratedRegex(@"<see cref=""(.*?)"" />", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex SeeRefRegex();
}
