using HtmlAgilityPack;

namespace crawler.Services;

public class HtmlLinkParser : ILinkParser
{
    private const string Href = "href";

    private IEnumerable<string> ExcludeLinks = new List<string>
    {
        "mailto:",
        "skype:",
        "tel:",
        "sms:"
    };

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
        {
            if (ExcludeLinks.Any(l => node.Value.StartsWith(l)))
                continue;
            yield return node.Value;
        }
    }
}