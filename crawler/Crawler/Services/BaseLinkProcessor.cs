using System.Collections.Concurrent;
using System.Net;
using Crawler.Extensions;
using Crawler.Infrastructure;
using Crawler.Models;

namespace Crawler.Services;

public abstract class BaseLinkProcessor<T> : ILinkProcessor<T> where T : ILinkParser
{
    private readonly PageLoader _pageLoader;
    private readonly ILinkParser _parser;
    protected readonly ConcurrentDictionary<string, CrawlItem> ProcessedLinks;
    protected readonly ConcurrentQueue<Uri> Queue;

    protected BaseLinkProcessor(PageLoader pageLoader, T parser)
    {
        _parser = parser;
        _pageLoader = pageLoader;
        Queue = new ConcurrentQueue<Uri>();
        ProcessedLinks = new ConcurrentDictionary<string, CrawlItem>();
    }

    public async Task<IEnumerable<CrawlItem>> ProcessAsync(Uri uri, IReadOnlyCollection<string> excludedMediaTypes, int maxThreads, CancellationToken token = default)
    {
        var tasks = new List<Task>();
        Queue.Enqueue(uri);
        for (var i = 0; i < maxThreads; i++)
        {
            var isFirst = i == 0;
            tasks.Add(Task.Run(async () =>
            {
                if (!isFirst)
                    await Task.Delay(3000, token);
                while (Queue.TryDequeue(out var currentLink))
                {
                    if (!currentLink.IsLinkAcceptable(uri) ||
                        ProcessedLinks.ContainsKey(currentLink.AbsoluteUri))
                        continue;
                    await ProcessLinkAsync(currentLink, excludedMediaTypes, token);
                }
            }, token));
        }

        await Task.WhenAll(tasks);

        return ProcessedLinks.Values.Where(v => !v.Url.EndsWith(".xml"));
    }

    private async Task ProcessLinkAsync(Uri uri, IEnumerable<string> excludedMediaTypes, CancellationToken token)
    {
        var response = await _pageLoader.GetResponseAsync(uri, excludedMediaTypes, token);

        if (response.Duration >= 0)
        {
            ProcessedLinks.TryAdd(uri.AbsoluteUri, new CrawlItem(uri.AbsoluteUri, response.Duration));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var links = _parser.GetLinks(response.Content);
                PopulateLinks(links, uri);
            }
        }
        else
        {
            ProcessedLinks.TryAdd(uri.AbsoluteUri, new CrawlItem(uri.AbsoluteUri, response.StatusCode.ToString()));
        }
    }

    protected abstract void PopulateLinks(IEnumerable<string> links, Uri baseUri);
}