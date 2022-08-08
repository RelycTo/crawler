using Crawler.Models;
using Shared.Models;

namespace Crawler.Interfaces.HandlerRequests;

public interface ICrawlRequest
{
    int CrawlId { get; }
    ProcessStep Step { get; }
    int TaskCount { get; }
    CrawlInfoDto CrawlInfo { get; }
}
