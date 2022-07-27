using HtmlAgilityPack;

namespace crawler.Services;

public class HtmlLinkParser: ILinkParser
{
    private const string Href = "href";
    
    public IEnumerable<string> GetLinks(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            yield break;

        var document = new HtmlDocument();
        document.LoadHtml(content);
        var nodes = document.DocumentNode.SelectNodes($"//a[@{Href}]");

        if (nodes == null)
            yield break;

        foreach (var node in nodes
                     .Where(n => n.Attributes.Contains(Href))
                     .Select(n => n.Attributes[Href]))
            yield return node.Value;
    }
}