namespace Crawler.Infrastructure;

public interface ILinkParser
{
    IEnumerable<string> GetLinks(string content);
}