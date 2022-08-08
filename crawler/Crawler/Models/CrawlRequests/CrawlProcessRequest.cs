using Crawler.Interfaces.HandlerRequests;
using Shared.Models;

namespace Crawler.Models.CrawlRequests;

public class CrawlProcessRequest : ICrawlProcessRequest
{
    public CrawlProcessRequest(int crawlId, ProcessStep step, Uri uri, int taskCount, CrawlInfoDto dto)
    {
        Uri = uri;
        Step = step;
        CrawlId = crawlId;
        TaskCount = taskCount;
        CrawlInfo = dto; 
        ExcludedMediaTypes = new List<string>();
        ProcessedItems = new Dictionary<ProcessStep, IEnumerable<CrawlItem>>();
    }

    public ProcessStep Step { get; private set; }
    public IReadOnlyCollection<string> ExcludedMediaTypes { get; private set; }
    public IReadOnlyDictionary<ProcessStep, IEnumerable<CrawlItem>> ProcessedItems { get; private set; }
    public Uri Uri { get; }
    public int CrawlId { get; set; }
    public int TaskCount { get; }
    public CrawlInfoDto CrawlInfo { get; }

    public ICrawlProcessRequest SetExcludedMediaTypes(IEnumerable<string> mediaTypes)
    {
        ExcludedMediaTypes = mediaTypes.ToArray();
        return this;
    }

    public ICrawlProcessRequest SetProcessedItems(IReadOnlyDictionary<ProcessStep, IEnumerable<CrawlItem>> items)
    {
        ProcessedItems = items;
        return this;
    }

    public ICrawlProcessRequest SetStep(ProcessStep step)
    {
        Step = step;
        return this;
    }
}