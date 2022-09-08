using Crawler.Domain.Models.Enums;

namespace Crawler.Application.Models;

public class CrawlHandlerContext
{
    private CrawlHandlerContext(ProcessStep step, CrawlInfoDto info, CrawlOptionsDto options)
    {
        ProcessedItems = new Dictionary<ProcessStep, IEnumerable<CrawlItemDto>>();
        Step = step;
        CrawlInfo = info;
        Options = options;
    }

    public ProcessStep Step { get; private set; }

    public CrawlInfoDto CrawlInfo { get; set; }
    public CrawlOptionsDto Options { get; set; }

    public Dictionary<ProcessStep, IEnumerable<CrawlItemDto>> ProcessedItems { get; private set; }

    public CrawlHandlerContext SetProcessedItems(ProcessStep step, IEnumerable<CrawlItemDto> items)
    {
        ProcessedItems[step] = items;
        return this;
    }

    public CrawlHandlerContext SetStep(ProcessStep step)
    {
        Step = step;
        return this;
    }

    public static CrawlHandlerContext Create(Uri uri, int taskCount, string siteMap = "sitemap.xml")
    {
        var siteMapUri = GetCombinedAbsoluteUri(uri.AbsoluteUri, siteMap);

        var info = new CrawlInfoDto
        {
            Url = uri.AbsoluteUri,
            Status = CrawlStatus.InProgress
        };

        var options = new CrawlOptionsDto(uri, siteMapUri, taskCount);

        return new CrawlHandlerContext(ProcessStep.Start, info, options);
    }

    private static Uri GetCombinedAbsoluteUri(string absoluteUri, string endPart)
    {
        return new Uri(absoluteUri.TrimEnd('/') + "/" + endPart);
    }
}