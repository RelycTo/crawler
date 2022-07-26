using System.Xml;
using System.Xml.Linq;

namespace crawler.Parsers
{
    public class SiteMapXMLParser
    {
        public IEnumerable<string> GetLinks(string content)
        {
            if(string.IsNullOrWhiteSpace(content))
                yield break;

            var document = new XmlDocument();
            document.LoadXml(content);

            var nodes = document.GetElementsByTagName("loc");

            foreach (XmlNode node in nodes)
            {
                if(string.IsNullOrWhiteSpace(node.FirstChild?.Value))
                    yield break;
                yield return node.FirstChild.Value;
            }
        }
    }
}
