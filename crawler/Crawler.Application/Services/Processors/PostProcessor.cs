using Crawler.Application.Models;
using Crawler.Application.Services.Loaders;
using Crawler.Domain.Models.Enums;

namespace Crawler.Application.Services.Processors;

public class PostProcessor : ILinkProcessor
{
    private readonly IPageLoader _loader;
    private Dictionary<string, CrawlItemDto> _processedItems;

    public PostProcessor(IPageLoader loader)
    {
        _loader = loader;
        _processedItems = new Dictionary<string, CrawlItemDto>();
    }

    public async Task<IEnumerable<CrawlItemDto>> ProcessAsync(CrawlHandlerContext context, CancellationToken token = default)
    {
        _processedItems = context.ProcessedItems[ProcessStep.Site].ToDictionary(k => k.Url, v => v);
        return await ProcessAsync(context.ProcessedItems[ProcessStep.SiteMap], context.Options.ExcludedMediaTypes, token);
    }

    private async Task<IEnumerable<CrawlItemDto>> ProcessAsync(IEnumerable<CrawlItemDto> items, IReadOnlyCollection<string> excludedMediaTypes, CancellationToken token = default)
    {
        foreach (var item in items)
        {
            if (_processedItems.TryGetValue(item.Url, out var processed))
            {
                if (processed.SourceType == SourceType.Site)
                {
                    _processedItems[item.Url] =
                        new CrawlItemDto(SourceType.Both, processed.Url, processed.Duration, processed.StatusCode);
                }

                continue;
            }

            var response = await _loader.GetResponseAsync(item.Uri, excludedMediaTypes, token);
            _processedItems[item.Url] =
                new CrawlItemDto(SourceType.SiteMap, item.Url, response.Duration, response.StatusCode);
        }

        return _processedItems.Values;
    }
}