namespace Crawler.Interfaces;

public interface ILinkParser
{
    IEnumerable<string> GetLinks(string content);
}