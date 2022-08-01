using System.Net;
using crawler.Infrastructure;
using crawler.Models;

namespace crawler.Services;

public class PostProcessor
{
    private readonly PageLoader _loader;
    private readonly IEnumerable<CrawlItem> _siteMapItems;
    private readonly Dictionary<string, ResultItem> _processedItems;
    private readonly IEnumerable<string> _excludedMediaTypes;

    public PostProcessor(PageLoader loader, IEnumerable<CrawlItem> crawledItems, IEnumerable<CrawlItem> siteMapItems,
        IEnumerable<string> excludedMediaTypes)
    {
        _loader = loader;
        _siteMapItems = siteMapItems;
        _processedItems = crawledItems
            .ToDictionary(k => k.Url,
                v => new ResultItem(v.Url, v.Duration, true, false));
        _excludedMediaTypes = excludedMediaTypes;
    }

    public async Task<IEnumerable<ResultItem>> ProcessAsync(CancellationToken token)
    {
        foreach (var item in _siteMapItems)
        {
            if (_processedItems.TryGetValue(item.Url, out var processed))
            {
                if (!processed.IsFoundBySiteMap)
                    _processedItems[item.Url].IsFoundBySiteMap = true;
                continue;
            }

            var response = await _loader.GetResponseAsync(item.Uri, _excludedMediaTypes, token);
            if (response.StatusCode == HttpStatusCode.OK)
                _processedItems[item.Url] = new ResultItem(item.Url, response.Duration, false, true);
        }

        return _processedItems.Values;
    }
}