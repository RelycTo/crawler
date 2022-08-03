using System.Net.Mime;
using Crawler.Infrastructure;
using Crawler.Models;

namespace Crawler.Services;

public class ProcessDispatcher
{
    private readonly ConsoleReport _report;
    private readonly PostProcessor _postProcessor;
    private readonly ILinkProcessor<HtmlLinkParser> _linkProcessor;
    private readonly ILinkProcessor<XmlLinkParser> _siteMapProcessor;

    public ProcessDispatcher(ILinkProcessor<HtmlLinkParser> linkProcessor,
        ILinkProcessor<XmlLinkParser> siteMapProcessor,
        PostProcessor postProcessor,
        ConsoleReport report)
    {
        _report = report;
        _linkProcessor = linkProcessor;
        _postProcessor = postProcessor;
        _siteMapProcessor = siteMapProcessor;
    }

    public async Task Run(string baseUrl, int maxThreads, CancellationToken token = default)
    {
        var crawl = await CrawlLinkAsync(baseUrl, maxThreads, token);
        var siteMapCrawl = await CrawlSiteMapAsync(baseUrl, maxThreads, token);
        var result = await PostProcessAsync(crawl, siteMapCrawl, token);

        _report.Build(result.ToList().AsReadOnly());
    }

    private async Task<IEnumerable<CrawlItem>> CrawlLinkAsync(string url, int maxThreads, CancellationToken token)
    {
        return await _linkProcessor.ProcessAsync(new Uri(url), new[] { MediaTypeNames.Text.Xml }, maxThreads, token);
    }

    private async Task<IEnumerable<CrawlItem>> CrawlSiteMapAsync(string url, int maxThreads, CancellationToken token)
    {
        url = url.TrimEnd('/') + "/sitemap.xml";
        return await _siteMapProcessor.ProcessAsync(new Uri(url), new[]
        {
            MediaTypeNames.Text.Html,
            MediaTypeNames.Text.Plain,
            MediaTypeNames.Text.RichText
        }, maxThreads, token);
    }

    private async Task<IEnumerable<ResultItem>> PostProcessAsync(IEnumerable<CrawlItem> links,
        IEnumerable<CrawlItem> siteMapLinks, CancellationToken token)
    {
        _postProcessor.SetProcessedItems(links.ToArray());
        return await _postProcessor.ProcessAsync(siteMapLinks.ToArray(), new[] { MediaTypeNames.Text.Xml }, token);
    }
}