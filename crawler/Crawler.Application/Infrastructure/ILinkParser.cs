namespace Crawler.Application.Infrastructure;

public interface ILinkParser
{
    IEnumerable<string> GetLinks(string content);
}