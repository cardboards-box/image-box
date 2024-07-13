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
    /// <returns>The abstract syntax tree</returns>
    IEnumerable<AstElement> ParseFile(string path);

    /// <summary>
    /// Parse the given HTML and return the abstract syntax tree elements
    /// </summary>
    /// <param name="html">The HTML to parse</param>
    /// <returns>The abstract syntax tree</returns>
    IEnumerable<AstElement> ParseString(string html);

    /// <summary>
    /// Parse the given stream and return the abstract syntax tree elements
    /// </summary>
    /// <param name="stream">The stream to parse</param>
    /// <returns>The abstract syntax tree</returns>
    IEnumerable<AstElement> ParseStream(Stream stream);
}

internal class AstParserService(
    IServiceConfig _config) : IAstParserService
{
    public IEnumerable<AstElement> ParseFile(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("The file path does not exist", path);

        var doc = new HtmlDocument();
        doc.Load(path);

        return Parse(doc.DocumentNode);
    }

    public IEnumerable<AstElement> ParseString(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        return Parse(doc.DocumentNode);
    }

    public IEnumerable<AstElement> ParseStream(Stream stream)
    {
        var doc = new HtmlDocument();
        doc.Load(stream);

        return Parse(doc.DocumentNode);
    }

    public static (AstElementType type, string? text) DetermineType(HtmlNode node)
    {
        //No children and no text - Empty element
        if (node.ChildNodes.Count == 0 ||
            string.IsNullOrWhiteSpace(node.InnerHtml))
            return (AstElementType.Empty, null);
        //Only one node and it's just text - Text element
        if (node.ChildNodes.Count == 1 &&
            node.FirstChild.NodeType == HtmlNodeType.Text)
        {
            //Get the inner text and validate the existence of it's value
            //No value - Empty element
            var inner = node.InnerText;
            if (string.IsNullOrWhiteSpace(inner))
                return (AstElementType.Empty, null);
            //Inner text has value - Text element
            return (AstElementType.Text, inner);
        }
        //Element has more than one child or the child is not text - Children element
        return (AstElementType.Children, null);
    }

    public IEnumerable<AstAttribute> GetAttributes(HtmlNode node)
    {
        //Iterate through each of the attributes
        foreach (var attribute in node.Attributes)
        {
            //Get the name of the tag
            var name = attribute.OriginalName.Trim();
            //Get the value of the tag (if it exists)
            var value = attribute.Value;
            //If the name has a colon, it's a bind-attribute and should be handled via an expression
            if (name.StartsWith(_config.Parser.Bind))
            {
                yield return AstAttribute.Bind(name[1..].Trim(), value);
                continue;
            }
            //The name starts with the spread start and ends, so handle the name as an expression
            if (name.StartsWith(_config.Parser.SpreadStart) &&
                name.EndsWith(_config.Parser.SpreadEnd))
            {
                yield return AstAttribute.Spread(name[1..^1].Trim());
                continue;
            }
            //Check if the attribute has an equal sign and no value
            var hasEqual = attribute.QuoteType == AttributeValueQuote.WithoutValue;
            //If the attribute has no equal sign and no value, it's a boolean true attribute
            if (!hasEqual &&
                string.IsNullOrEmpty(value))
            {
                yield return AstAttribute.BooleanTrue(name);
                continue;
            }
            //regular attribute with no bind or spreads
            yield return AstAttribute.Text(name, value);
        }
    }

    public IEnumerable<AstElement> Parse(HtmlNode parent)
    {
        //If the parent has no children or no text, skip children
        if (parent.ChildNodes.Count == 0 ||
            string.IsNullOrWhiteSpace(parent.InnerHtml)) yield break;
        //Iterate through each of the children
        foreach (var node in parent.ChildNodes)
        {
            //Skip just text nodes
            if (node.NodeType == HtmlNodeType.Text) continue;
            //get the name of the element
            var name = node.OriginalName;
            //Determine the type of the children of the element
            var (type, value) = DetermineType(node);
            var children = type == AstElementType.Children
                ? Parse(node).ToArray()
                : [];
            var attributes = GetAttributes(node).ToArray();
            //Create the element
            yield return new AstElement(
                node.StreamPosition, node.Line, node.LinePosition,
                name, type, attributes, children, value);
        }
    }
}
