using Crawler.App.DTOs;
using Crawler.App.Infrastructure.Loaders;
using Crawler.Entities.Models.Enums;

namespace Crawler.App.Services.Processors;

public class PostProcessor : ILinkProcessor
{
    private readonly PageLoader _loader;
    private Dictionary<string, CrawlItem> _processedItems;

    public PostProcessor(PageLoader loader)
    {
        _loader = loader;
        _processedItems = new Dictionary<string, CrawlItem>();
    }

    public async Task<IEnumerable<CrawlItem>> ProcessAsync(CrawlHandlerContext context, CancellationToken token = default)
    {
        _processedItems = context.ProcessedItems[ProcessStep.Site].ToDictionary(k => k.Url, v => v);
        return await ProcessAsync(context.ProcessedItems[ProcessStep.SiteMap], context.Options.ExcludedMediaTypes, token);
    }

    private async Task<IEnumerable<CrawlItem>> ProcessAsync(IEnumerable<CrawlItem> items, IReadOnlyCollection<string> excludedMediaTypes, CancellationToken token = default)
    {
        foreach (var item in items)
        {
            if (_processedItems.TryGetValue(item.Url, out var processed))
            {
                if (processed.SourceType == SourceType.Site)
                {
                    _processedItems[item.Url] =
                        new CrawlItem(SourceType.Both, processed.Url, processed.Duration, processed.StatusCode);
                }

                continue;
            }

            var response = await _loader.GetResponseAsync(item.Uri, excludedMediaTypes, token);
            _processedItems[item.Url] =
                new CrawlItem(SourceType.SiteMap, item.Url, response.Duration, response.StatusCode);
        }

        return _processedItems.Values;
    }
}