namespace Shared.Models;

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

    public static CrawlHandlerContext Create(Uri uri, int taskCount)
    {
        return new CrawlHandlerContext(ProcessStep.Start)
        {
            Options = new CrawlOptions(uri, taskCount)
        };
    }
}