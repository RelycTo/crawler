using Crawler.Models;

namespace Crawler.Interfaces.HandlerRequests;

public interface ICrawlProcessRequest: IProcessRequest
{

    Uri Uri { get; }
    ICrawlProcessRequest SetExcludedMediaTypes(IEnumerable<string> mediaTypes);
    ICrawlProcessRequest SetProcessedItems(IReadOnlyDictionary<ProcessStep, IEnumerable<CrawlItem>> items);
    ICrawlProcessRequest SetStep(ProcessStep step);
}