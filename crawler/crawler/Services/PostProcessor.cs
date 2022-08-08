using Crawler.Infrastructure;
using Crawler.Interfaces.HandlerRequests;
using Crawler.Interfaces.Services;
using Crawler.Models;
using Shared.Models;

namespace Crawler.Services;

public class PostProcessor: ILinkProcessor
{
    private readonly PageLoader _loader;
    private Dictionary<string, CrawlItem> _processedItems;

    public PostProcessor(PageLoader loader)
    {
        _loader = loader;
        _processedItems = new Dictionary<string, CrawlItem>();
    }

    public async Task<IEnumerable<CrawlItem>> ProcessAsync(ICrawlRequest request, CancellationToken token = default)
    {
        if (request is ICrawlProcessRequest processRequest)
        {
            _processedItems = processRequest.ProcessedItems[ProcessStep.Site].ToDictionary(k => k.Url, v => v);
            return await ProcessAsync(processRequest.ProcessedItems[ProcessStep.SiteMap],
                processRequest.ExcludedMediaTypes, token);
        }

        throw new InvalidOperationException($"Unsupported type of request: {request.GetType()}");
    }

    private async Task<IEnumerable<CrawlItem>> ProcessAsync(IEnumerable<CrawlItem> items, IReadOnlyCollection<string> excludedMediaTypes, CancellationToken token = default)
    {
        foreach (var item in items)
        {
            if (_processedItems.TryGetValue(item.Url, out var processed))
            {
                if (processed.SourceType == SourceType.Site)
                    _processedItems[item.Url] = new CrawlItem(SourceType.Both, processed.Url, processed.Duration,
                        processed.StatusCode);
                continue;
            }

            var response = await _loader.GetResponseAsync(item.Uri, excludedMediaTypes, token);
            _processedItems[item.Url] =
                new CrawlItem(SourceType.SiteMap, item.Url, response.Duration, response.StatusCode);
        }

        return _processedItems.Values;
    }
}