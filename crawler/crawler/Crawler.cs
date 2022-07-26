using System.Collections.Concurrent;
using crawler.Parsers;

namespace crawler;

internal class Crawler
{
    private readonly string _url;
    private readonly HttpClient _httpClient;
    private readonly HtmlParser _htmlParser;
    private readonly SiteMapXMLParser _xmlParser;
    private HashSet<string> _processedLinks;
    private readonly ConcurrentQueue<string> _queue;

    private readonly HashSet<string> _acceptablePrefixes = new()
    {
        "http:",
        "https:"
    };

    public Crawler(string url, HttpClient client, HtmlParser htmlParser, SiteMapXMLParser xmlParser)
    {
        _url = url;
        _httpClient = client;
        _htmlParser = htmlParser;
        _xmlParser = xmlParser;
        _queue = new ConcurrentQueue<string>();
        _processedLinks = new HashSet<string>();
    }

    public async Task Crawl()
    {
        _queue.Enqueue(_url);

        var currentLink = string.Empty;
        while (_queue.TryDequeue(out currentLink))
        {
            if (!IsLinkAcceptable(currentLink, _url, _acceptablePrefixes) || _processedLinks.Contains(currentLink))
                continue;
            var response = await _httpClient.GetStringAsync(currentLink);
            _processedLinks.Add(currentLink);
            var links = _htmlParser.GetLinks(response);
            foreach (var link in links)
            {
                if(_processedLinks.Contains(link))
                    continue;
                _queue.Enqueue(link);
            }
        }
        //var siteMapLinks = _xmlParser.GetLinks(response).ToArray();
    }

    private static bool IsLinkAcceptable(string link, string baseUrl, IEnumerable<string> prefixes)
    {
        return !string.IsNullOrWhiteSpace(link) &&
               IsAcceptablePrefix(link, prefixes) &&
               IsInternalLink(new Uri(link), new Uri(baseUrl));
    }

    private static bool IsAcceptablePrefix(string link, IEnumerable<string> prefixes) =>
        prefixes.Any(link.StartsWith);

    private static bool IsInternalLink(Uri checkUri, Uri baseUri) =>
        checkUri.Host == baseUri.Host;
}
