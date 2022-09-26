using Crawler.Application.Infrastructure;
using Crawler.Domain.Models.Enums;

namespace Crawler.Application.Crawl.Processors;

public class PostProcessor
{
    private readonly IPageLoader _loader;
    private Dictionary<string, CrawlItemDto> _processedItems;

    public PostProcessor(IPageLoader loader)
    {
        _loader = loader;
        _processedItems = new Dictionary<string, CrawlItemDto>();
    }

    public async Task<IEnumerable<CrawlItemDto>> ProcessAsync(IEnumerable<CrawlItemDto> siteItems,
        IEnumerable<CrawlItemDto> sitemapItems,
        IReadOnlyCollection<string> excludedMediaTypes, CancellationToken token = default)
    {
        _processedItems = siteItems.ToDictionary(k => k.Url, v => v);
        foreach (var item in sitemapItems)
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