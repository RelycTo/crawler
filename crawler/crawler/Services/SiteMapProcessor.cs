using System.Collections.Concurrent;
using crawler.Extensions;

namespace crawler.Services;

public class SiteMapProcessor
{
    private readonly LinkLoader _linkLoader;
    private readonly ConcurrentDictionary<string, string> _processedLinks;
    private readonly ConcurrentQueue<string> _queue;
    private readonly int _maxThreads;

    public SiteMapProcessor(LinkLoader linkLoader, int maxThreads = 4)
    {
        _linkLoader = linkLoader;
        _maxThreads = maxThreads;
        _processedLinks = new ConcurrentDictionary<string, string>();
        _queue = new ConcurrentQueue<string>();
    }

    public async Task<IEnumerable<string>> Process(string url)
    {
        _queue.Enqueue(url);
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
        if (!link.EndsWith(".xml"))
        {
            _processedLinks.TryAdd(link, link);
        }
        else
        {
            var (duration, links) = await _linkLoader.GetLinks(link);
            if (duration >= 0)
            {
                _processedLinks.TryAdd(link, link);
                PopulateLinks(links);
            }
            else
            {
                _processedLinks.TryAdd(link, link);
            }
        }
    }
 
    private void PopulateLinks(IEnumerable<string> links)
    {
        foreach (var link in links)
        {
            if (_processedLinks.ContainsKey(link))
                continue;
            if (link.EndsWith(".xml"))
                _queue.Enqueue(link);
            else
            {
                _processedLinks.TryAdd(link, link);
            }
        }
    }

}
