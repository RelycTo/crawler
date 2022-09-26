namespace Crawler.Application.Services.Parsers;

public interface ILinkParser
{
    IEnumerable<string> GetLinks(string content);
}