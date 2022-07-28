using System.Collections.Concurrent;
using System.Net;
using crawler.Extensions;
using crawler.Models;

namespace crawler.Services;

public class LinkProcessor
{
    private readonly PageLoader _pageLoader;
    protected readonly ILinkParser _parser;
    protected readonly ConcurrentDictionary<Uri, CrawlItem> ProcessedLinks;
    protected readonly ConcurrentQueue<Uri> Queue;
    private readonly int _maxThreads;

    public LinkProcessor(PageLoader pageLoader, ILinkParser parser, IEnumerable<Uri> links, int maxThreads)
    {
        _pageLoader = pageLoader;
        Queue = new ConcurrentQueue<Uri>(links);
        ProcessedLinks = new ConcurrentDictionary<Uri, CrawlItem>();
        _maxThreads = maxThreads;
        _parser = parser;
    }

    public async Task<IEnumerable<CrawlItem>> ProcessAsync(Uri uri, CancellationToken token = default)
    {
        var tasks = new List<Task>();
        Queue.Enqueue(uri);
        for (var n = 0; n < _maxThreads; n++)
        {
            tasks.Add(Task.Run(async () =>
            {
                while (Queue.TryDequeue(out var currentLink))
                {
                    if (!UrlExtensions.IsLinkAcceptable(currentLink, uri) ||
                        ProcessedLinks.ContainsKey(currentLink))
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
        var response = await _pageLoader.GetResponseAsync(uri, token);

        if (response.Duration >= 0)
        {
            ProcessedLinks.TryAdd(uri, new CrawlItem(uri.AbsoluteUri, response.Duration));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var links = _parser.GetLinks(response.Content);
                PopulateLinks(links, uri);
            }
        }
        else
        {
            ProcessedLinks.TryAdd(uri, new CrawlItem(uri.AbsoluteUri, response.StatusCode.ToString()));
        }
    }

    protected virtual void PopulateLinks(IEnumerable<string> links, Uri baseUri)
    {
        foreach (var link in links)
        {
            try
            {
                var uri = new Uri(link.FixLink(baseUri));
                if (ProcessedLinks.ContainsKey(uri) || link.EndsWith(".xml"))
                    continue;
                Queue.Enqueue(uri);
            }
            catch (UriFormatException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}