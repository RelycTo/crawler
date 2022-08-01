using System.Collections.Concurrent;
using System.Net;
using crawler.Extensions;
using crawler.Infrastructure;
using crawler.Models;

namespace crawler.Services;

public class LinkProcessor
{
    private readonly PageLoader _pageLoader;
    private readonly ILinkParser _parser;
    protected readonly ConcurrentDictionary<string, CrawlItem> ProcessedLinks;
    protected readonly ConcurrentQueue<Uri> Queue;
    private readonly IEnumerable<string> _excludedMediaTypes;
    private readonly int _maxThreads;

    public LinkProcessor(PageLoader pageLoader, ILinkParser parser, IEnumerable<string> excludedMediaTypes,
        int maxThreads)
    {
        _parser = parser;
        _pageLoader = pageLoader;
        _maxThreads = maxThreads;
        Queue = new ConcurrentQueue<Uri>();
        _excludedMediaTypes = excludedMediaTypes;
        ProcessedLinks = new ConcurrentDictionary<string, CrawlItem>();
    }

    public async Task<IEnumerable<CrawlItem>> ProcessAsync(Uri uri, CancellationToken token = default)
    {
        var tasks = new List<Task>();
        Queue.Enqueue(uri);
        for (var i = 0; i < _maxThreads; i++)
        {
            var isFirst = i == 0;
            tasks.Add(Task.Run(async () =>
            {
                if (isFirst)
                    await Task.Delay(3000, token);
                while (Queue.TryDequeue(out var currentLink))
                {
                    if (!currentLink.IsLinkAcceptable(uri) ||
                        ProcessedLinks.ContainsKey(currentLink.AbsoluteUri))
                        continue;
                    await ProcessLinkAsync(currentLink, token);
                }
            }, token));
        }

        await Task.WhenAll(tasks);

        return ProcessedLinks.Values.Where(v => !v.Url.EndsWith(".xml"));
    }

    private async Task ProcessLinkAsync(Uri uri, CancellationToken token = default)
    {
        var response = await _pageLoader.GetResponseAsync(uri, _excludedMediaTypes, token);

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

    protected virtual void PopulateLinks(IEnumerable<string> links, Uri baseUri)
    {
        foreach (var link in links)
        {
            var restored = link.RestoreAbsolutePath(baseUri);
            if (!Uri.IsWellFormedUriString(restored, UriKind.Absolute))
                continue;
            var uri = new Uri(restored);
            if (ProcessedLinks.ContainsKey(uri.AbsoluteUri.TrimEnd('/'))
                || link.EndsWith(".xml") || !uri.IsLinkAcceptable(baseUri))
                continue;
            Queue.Enqueue(uri);
        }
    }
}