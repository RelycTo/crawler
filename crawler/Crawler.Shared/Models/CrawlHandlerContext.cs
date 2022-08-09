namespace Crawler.Shared.Models;

public class CrawlHandlerContext
{
    private CrawlHandlerContext(ProcessStep step)
    {
        ProcessedItems = new Dictionary<ProcessStep, IEnumerable<CrawlItem>>();
        Step = step;
    }

    public ProcessStep Step { get; private set; }

    public CrawlInfoDto CrawlInfo { get; set; }
    public CrawlOptions Options { get; set; }

    public Dictionary<ProcessStep, IEnumerable<CrawlItem>> ProcessedItems { get; private set; }

    public CrawlHandlerContext SetProcessedItems(ProcessStep step, IEnumerable<CrawlItem> items)
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
        var siteMapUri = new Uri(uri.AbsoluteUri + "/" + siteMap);
        return new CrawlHandlerContext(ProcessStep.Start)
        {
            CrawlInfo = new CrawlInfoDto
            {
                Url = uri.AbsoluteUri,
                Status = CrawlStatus.InProgress
            },
            Options = new CrawlOptions(uri, siteMapUri, taskCount)
        };
    }
}