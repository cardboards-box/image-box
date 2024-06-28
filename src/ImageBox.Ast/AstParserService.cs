using HtmlAgilityPack;

namespace ImageBox.Ast;

/// <summary>
/// A service for parsing templates in to abstract syntax tree elements
/// </summary>
public interface IAstParserService
{
    /// <summary>
    /// Parse the given file and return the abstract syntax tree elements
    /// </summary>
    /// <param name="path">The file path to parse</param>
    /// <param name="config">The AST configuration</param>
    /// <returns>The abstract syntax tree</returns>
    IEnumerable<AstElement> ParseFile(string path, AstConfig config);

    /// <summary>
    /// Parse the given HTML and return the abstract syntax tree elements
    /// </summary>
    /// <param name="html">The HTML to parse</param>
    /// <param name="config">The AST configuration</param>
    /// <returns>The abstract syntax tree</returns>
    IEnumerable<AstElement> ParseString(string html, AstConfig config);

    /// <summary>
    /// Parse the given stream and return the abstract syntax tree elements
    /// </summary>
    /// <param name="stream">The stream to parse</param>
    /// <param name="config">The AST configuration</param>
    /// <returns>The abstract syntax tree</returns>
    IEnumerable<AstElement> ParseStream(Stream stream, AstConfig config);
}

internal class AstParserService : IAstParserService
{
    public IEnumerable<AstElement> ParseFile(string path, AstConfig config)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("The file path does not exist", path);

        var doc = new HtmlDocument();
        doc.Load(path);

        return Parse(doc.DocumentNode, config);
    }

    public IEnumerable<AstElement> ParseString(string html, AstConfig config)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        return Parse(doc.DocumentNode, config);
    }

    public IEnumerable<AstElement> ParseStream(Stream stream, AstConfig config)
    {
        var doc = new HtmlDocument();
        doc.Load(stream);

        return Parse(doc.DocumentNode, config);
    }

    public static (AstElementType type, string? text) DetermineType(HtmlNode node)
    {
        if (node.ChildNodes.Count == 0 ||
            string.IsNullOrWhiteSpace(node.InnerHtml))
            return (AstElementType.Empty, null);

        if (node.ChildNodes.Count == 1 &&
            node.FirstChild.NodeType == HtmlNodeType.Text)
        {
            var inner = node.InnerText;
            if (string.IsNullOrWhiteSpace(inner))
                return (AstElementType.Empty, null);

            return (AstElementType.Text, inner);
        }

        return (AstElementType.Children, null);
    }

    public static IEnumerable<AstAttribute> GetAttributes(HtmlNode node, AstConfig config)
    {
        foreach (var attribute in node.Attributes)
        {
            var name = attribute.OriginalName.Trim();
            var value = attribute.Value;

            if (name.StartsWith(config.AttributeBind))
            {
                yield return new AstAttribute
                {
                    Name = name[1..].Trim(),
                    Type = AstAttributeType.Bind,
                    Value = value
                };
                continue;
            }

            if (name.StartsWith(config.AttributeSpreadStart) &&
                name.EndsWith(config.AttributeSpreadEnd))
            {
                yield return new AstAttribute
                {
                    Name = name[1..^1].Trim(),
                    Type = AstAttributeType.Spread
                };
                continue;
            }

            var hasEqual = attribute.GetPrivateFieldValue<HtmlAttribute, bool>("_hasEqual");

            if (!hasEqual &&
                string.IsNullOrEmpty(value))
            {
                yield return new AstAttribute
                {
                    Name = name,
                    Type = AstAttributeType.BooleanTrue
                };
                continue;
            }

            yield return new AstAttribute
            {
                Name = name,
                Type = AstAttributeType.Value,
                Value = value
            };
        }
    }

    public static IEnumerable<AstElement> Parse(HtmlNode parent, AstConfig config)
    {
        if (parent.ChildNodes.Count == 0 ||
            string.IsNullOrWhiteSpace(parent.InnerHtml)) yield break;

        foreach (var node in parent.ChildNodes)
        {
            if (node.NodeType == HtmlNodeType.Text) continue;

            var name = node.OriginalName;
            var (type, value) = DetermineType(node);

            var element = new AstElement
            {
                StreamPosition = node.StreamPosition,
                Line = node.Line,
                Column = node.LinePosition,
                Tag = name,
                Type = type,
                Value = value,
                Attributes = GetAttributes(node, config).ToArray()
            };

            if (type == AstElementType.Children)
                element.Children = Parse(node, config).ToArray();

            yield return element;
        }
    }
}
