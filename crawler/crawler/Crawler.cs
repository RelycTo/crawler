using System.Net.Mime;
using crawler.Models;
using crawler.Services;

namespace crawler;

internal class Crawler
{
    private readonly string _url;
    private readonly HttpClient _httpClient;

    public Crawler(string url, HttpClient client)
    {
        _url = url;
        _httpClient = client;
    }

    public async Task Crawl()
    {
        var crawlResults = await ProcessLinks();
        var siteMapLinks = (await GetSiteMapLinks()).ToArray();

        var results = new Dictionary<string, ResultItem>();

        foreach (var link in crawlResults)
        {
            results[link.Url] = link;
            if (siteMapLinks.Contains(link.Url))
                results[link.Url].UpdateFoundBySiteMapFlag(true);
        }

        var linksToProcess = siteMapLinks.Where(l => !results.ContainsKey(l));
        var loader = new LinkLoader(_httpClient, new HtmlLinkParser(), new[] { MediaTypeNames.Text.Xml });
        var processor = new LinkProcessor(loader, linksToProcess, results, true);
        var r = await processor.Process(_url);

    }

    private async Task<IEnumerable<ResultItem>> ProcessLinks()
    {
        var loader = new LinkLoader(_httpClient, new HtmlLinkParser(), new[] { MediaTypeNames.Text.Xml });
        var processor = new LinkProcessor(loader, new []{_url});
        return await processor.Process(_url);
    }

    private async Task<IEnumerable<string>> GetSiteMapLinks()
    {
        var loader = new LinkLoader(_httpClient, new XmlLinkParser(), new[] { MediaTypeNames.Text.Html });
        var processor = new SiteMapProcessor(loader);
        return await processor.Process(_url + "/sitemap.xml");
    }
}
