using Crawler.Models;

namespace Crawler.Interfaces.HandlerRequests;

public interface IProcessRequest : ICrawlRequest
{
    IReadOnlyCollection<string> ExcludedMediaTypes { get; }
    IReadOnlyDictionary<ProcessStep, IEnumerable<CrawlItem>> ProcessedItems { get; }
}
