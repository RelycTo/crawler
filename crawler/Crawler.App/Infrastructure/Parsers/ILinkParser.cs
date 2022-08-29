namespace Crawler.App.Infrastructure.Parsers;

public interface ILinkParser
{
    IEnumerable<string> GetLinks(string content);
}