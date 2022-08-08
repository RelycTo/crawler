using Crawler.Interfaces.HandlerRequests;
using Shared.Models;

namespace Crawler.Models.CrawlRequests;

public class CrawlRequest : ICrawlRequest
{
    public CrawlRequest(ProcessStep step, int crawlId, int taskCount, CrawlInfoDto dto)
    {
        Step = step;
        CrawlId = crawlId;
        CrawlInfo = dto;
        TaskCount = taskCount;
    }

    public int CrawlId { get; }
    public int TaskCount { get; }
    public ProcessStep Step { get; }
    public CrawlInfoDto CrawlInfo { get; }
}
