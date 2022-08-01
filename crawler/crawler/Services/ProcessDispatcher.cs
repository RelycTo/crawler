using Crawler.Infrastructure;
using Crawler.Models;

namespace Crawler.Services;

public class ProcessDispatcher
{
    private readonly int _maxThreads;
    private readonly ConsoleReport _report;
    private readonly ProcessorFactory _factory;

    public ProcessDispatcher(ProcessorFactory factory, int maxThreads, ConsoleReport report)
    {
        _report = report;
        _factory = factory;
        _maxThreads = maxThreads;
    }

    public async Task Run(string baseUrl, CancellationToken token = default)
    {
        var crawl = await CrawlLinkAsync(baseUrl, token);
        var siteMapCrawl = await CrawlSiteMapAsync(baseUrl, token);
        var result = await PostProcessAsync(crawl, siteMapCrawl, token);

        _report.Build(result.ToList().AsReadOnly());
    }

    private async Task<IEnumerable<CrawlItem>> CrawlLinkAsync(string url, CancellationToken token)
    {
        var processor = _factory.CreateLinkProcessor(new HtmlLinkParser(), _maxThreads);
        return await processor.ProcessAsync(new Uri(url), token);
    }

    private async Task<IEnumerable<CrawlItem>> CrawlSiteMapAsync(string url, CancellationToken token)
    {
        url = url.TrimEnd('/') + "/sitemap.xml";
        var processor = _factory.CreateSiteMapProcessor(new XmlLinkParser(), _maxThreads);
        return await processor.ProcessAsync(new Uri(url), token);
    }

    private async Task<IEnumerable<ResultItem>> PostProcessAsync(IEnumerable<CrawlItem> links,
        IEnumerable<CrawlItem> siteMapLinks, CancellationToken token)
    {
        var processor = _factory.CreatePostProcessor(links, siteMapLinks);
        return await processor.ProcessAsync(token);
    }
}