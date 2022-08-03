using System.Net;
using Crawler.Infrastructure;
using Crawler.Models;

namespace Crawler.Services;

public class PostProcessor
{
    private readonly PageLoader _loader;
    private Dictionary<string, ResultItem> _processedItems;

    public PostProcessor(PageLoader loader)
    {
        _loader = loader;
        _processedItems = new Dictionary<string, ResultItem>();
    }

    public void SetProcessedItems(IReadOnlyCollection<CrawlItem> items)
    {
        _processedItems = items.ToDictionary(k => k.Url, v => new ResultItem(v.Url, v.Duration, true, false));
    }

    public virtual async Task<IEnumerable<ResultItem>> ProcessAsync(IReadOnlyCollection<CrawlItem> siteMapItems, IReadOnlyCollection<string> excludedMediaTypes, CancellationToken token = default)
    {
        foreach (var item in siteMapItems)
        {
            if (_processedItems.TryGetValue(item.Url, out var processed))
            {
                if (!processed.IsFoundBySiteMap)
                    _processedItems[item.Url].IsFoundBySiteMap = true;
                continue;
            }

            var response = await _loader.GetResponseAsync(item.Uri, excludedMediaTypes, token);
            if (response.StatusCode == HttpStatusCode.OK)
                _processedItems[item.Url] = new ResultItem(item.Url, response.Duration, false, true);
        }

        return _processedItems.Values;
    }
}