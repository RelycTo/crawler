using System.Net.Mime;
using Crawler.Infrastructure;
using Crawler.Interfaces.HandlerRequests;
using Crawler.Interfaces.Services;
using Crawler.Models;
using Crawler.Models.CrawlRequests;
using Shared.Models;

namespace Crawler.Services;
//[Obsolete]
public class ProcessDispatcher
{
    /*
    private readonly ConsoleReport _report;
    private readonly PostProcessor _postProcessor;
    private readonly ILinkProcessor<HtmlLinkParser> _linkProcessor;
    private readonly ILinkProcessor<XmlLinkParser> _siteMapProcessor;
    */

    private readonly ICrawlHandler<ICrawlRequest> _crawlerHandler;

    public ProcessDispatcher(/*ILinkProcessor<HtmlLinkParser> linkProcessor,
        ILinkProcessor<XmlLinkParser> siteMapProcessor,*/
        ICrawlHandler<ICrawlRequest> crawlerHandler,
        PostProcessor postProcessor,
        ConsoleReport report)
    {
//        _report = report;
        //_linkProcessor = linkProcessor;
//        _postProcessor = postProcessor;
        //_siteMapProcessor = siteMapProcessor;
        _crawlerHandler = crawlerHandler;
        /*
        _crawlerHandler = new SiteCrawlHandler(_linkProcessor);
        _crawlerHandler
            .SetNext(new SiteMapCrawlHandler(_siteMapProcessor));
    */
    }

    public async Task Run(string baseUrl, int maxThreads, CancellationToken token = default)
    {
        var request = new CrawlRequest(ProcessStep.Start, -1, 10, new CrawlInfoDto
        {
            Status = CrawlStatus.InProgress,
            Url = baseUrl
        });
        //var request = new SiteCrawlRequest(ProcessStep.Site, new Uri(baseUrl), maxThreads);
        await _crawlerHandler.Handle(request, token);
        /*
        var crawl = await CrawlLinkAsync(baseUrl, maxThreads, token);
        var siteMapCrawl = await CrawlSiteMapAsync(baseUrl, maxThreads, token);
        var result = await PostProcessAsync(crawl, siteMapCrawl, token);

        _report.Build(result.ToList().AsReadOnly());
    */
    }

    /*
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
    */

    /*
    private async Task<IEnumerable<ResultItem>> PostProcessAsync(IEnumerable<CrawlItem> links,
        IEnumerable<CrawlItem> siteMapLinks, CancellationToken token)
    {
        _postProcessor.SetProcessedItems(links.ToArray());
        return await _postProcessor.ProcessAsync(siteMapLinks.ToArray(), new[] { MediaTypeNames.Text.Xml }, token);
    }
*/
}