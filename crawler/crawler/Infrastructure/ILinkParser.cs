namespace crawler.Infrastructure;

public interface ILinkParser
{
    IEnumerable<string> GetLinks(string content);
}