using Crawler.Interfaces.HandlerRequests;
using Shared.Models;

namespace Crawler.Models.CrawlRequests;

public class DataProcessRequest : IDataProcessRequest
{
    public DataProcessRequest(int crawlId, ProcessStep step, IEnumerable<CrawlItem> crawledItems, CrawlInfoDto dto,
        int taskCount = 0)
    {
        Step = step;
        CrawlId = crawlId;
        CrawledItems = crawledItems;
        TaskCount = taskCount;
        CrawlInfo = dto;
    }

    public ProcessStep Step { get; }
    public int TaskCount { get; }
    public CrawlInfoDto CrawlInfo { get; }
    public int CrawlId { get; }
    public IEnumerable<CrawlItem> CrawledItems { get; }
}