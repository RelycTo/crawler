namespace crawler.Services;

public interface ILinkParser
{
    IEnumerable<string> GetLinks(string content);
}