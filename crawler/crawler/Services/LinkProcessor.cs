using System.Collections.Concurrent;
using crawler.Extensions;
using crawler.Models;

namespace crawler.Services;

public class LinkProcessor
{
    private readonly LinkLoader _linkLoader;
    private readonly ConcurrentDictionary<string, ResultItem> _processedLinks;
    private readonly ConcurrentQueue<string> _queue;
    private readonly int _maxThreads;
    private readonly bool _isSiteMapLink;

    public LinkProcessor(LinkLoader linkLoader, IEnumerable<string> links, int maxThreads = 4)
    {
        _linkLoader = linkLoader;
        _queue = new ConcurrentQueue<string>(links);
        _processedLinks = new ConcurrentDictionary<string, ResultItem>();
        _maxThreads = maxThreads;
    }
    public LinkProcessor(LinkLoader linkLoader, IEnumerable<string> links, Dictionary<string, ResultItem> processed, bool isSiteMapLink, int maxThreads = 4)
    {
        _linkLoader = linkLoader;
        _queue = new ConcurrentQueue<string>(links);
        _processedLinks = new ConcurrentDictionary<string, ResultItem>(processed);
        _maxThreads = maxThreads;
        _isSiteMapLink = isSiteMapLink;
    }

    public async Task<IEnumerable<ResultItem>> Process(string url)
    {
        var tasks = new List<Task>();
            for (var n = 0; n < _maxThreads; n++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    while (_queue.TryDequeue(out var currentLink))
                    {
                        if (!UrlExtensions.IsLinkAcceptable(currentLink, url) ||
                            _processedLinks.ContainsKey(currentLink))
                            continue;
                        await ProcessLink(currentLink);
                    }
                }));
            }

            await Task.WhenAll(tasks);

        return _processedLinks.Values;
    }

    private async Task ProcessLink(string link)
    {
        var (duration, links) = await _linkLoader.GetLinks(link);

        if (duration >= 0)
        {
            _processedLinks.TryAdd(link, new ResultItem(link, duration, _isSiteMapLink));
            PopulateLinks(links);
        }
        else
        {
            _processedLinks.TryAdd(link, new ResultItem(link, _isSiteMapLink,"unavailable"));
        }
    }

    private void PopulateLinks(IEnumerable<string> links)
    {
        foreach (var link in links)
        {
            if (_processedLinks.ContainsKey(link))
                continue;
            _queue.Enqueue(link);
        }
    }
}
