using Crawler.Models;

namespace Crawler.Interfaces.HandlerRequests
{
    public interface IDataProcessRequest: ICrawlRequest
    {
        IEnumerable<CrawlItem> CrawledItems { get; }
    }
}
