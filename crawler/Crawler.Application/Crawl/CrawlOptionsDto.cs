using Crawler.Domain.Models.Enums;

namespace Crawler.Application.Crawl;

public class CrawlOptionsDto
{
    public CrawlOptionsDto(string url, string baseUrl, int taskCount, ProcessStep step,
        IReadOnlyCollection<string> excludedMediaTypes)
    {
        Uri = new Uri(url);
        TaskCount = taskCount;
        BaseUri = new Uri(baseUrl);
        ExcludedMediaTypes = excludedMediaTypes;
        Step = step;
    }

    public IReadOnlyCollection<string> ExcludedMediaTypes { get; private set; }
    public Uri Uri { get; }
    public Uri BaseUri { get; }
    public int TaskCount { get; }
    public ProcessStep Step { get; }
}