namespace Crawler.Application.Models;

public class CrawlOptionsDto
{
    public CrawlOptionsDto(string url, string siteMapPageName, int taskCount)
    {
        Uri = new Uri(url);
        TaskCount = taskCount;
        SiteMapUri = new Uri(url.TrimEnd('/') + '/' + siteMapPageName);
        ExcludedMediaTypes = Array.Empty<string>();
    }

    public IReadOnlyCollection<string> ExcludedMediaTypes { get; private set; }
    public Uri Uri { get; }
    public Uri SiteMapUri { get; }
    public int TaskCount { get; }

    public CrawlOptionsDto SetExcludedMediaTypes(IEnumerable<string> mediaTypes)
    {
        ExcludedMediaTypes = mediaTypes.ToArray();
        return this;
    }
}
