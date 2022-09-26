using System.Xml;
using Crawler.Application.Infrastructure;

namespace Crawler.Infrastructure.Parsers;

public class XmlLinkParser : IXmlParser
{
    public IEnumerable<string> GetLinks(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            yield break;
        }

        var document = new XmlDocument();
        try
        {
            document.LoadXml(content);
        }
        catch (XmlException e)
        {
            Console.WriteLine(e.Message);
            yield break;
        }

        var nodes = document.GetElementsByTagName("loc");

        foreach (XmlNode node in nodes)
        {
            if (string.IsNullOrWhiteSpace(node.FirstChild?.Value))
                yield break;
            yield return node.FirstChild.Value;
        }
    }
}