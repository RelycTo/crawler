using System.Net;
using System.Net.Mime;
using crawler.Extensions;
using crawler.Models;

namespace crawler.Services;

public class ProcessDispatcher
{
    private readonly PageLoader _loader;

    public ProcessDispatcher(PageLoader loader)
    {
        _loader = loader;
    }

    public async Task Run(string baseUrl, CancellationToken token = default)
    {
        var crawledResult = await ProcessAsync(new HtmlLinkParser(), baseUrl, new[] { MediaTypeNames.Text.Xml }, token);
        var siteMapResult = await ProcessAsync(new XmlLinkParser(), baseUrl.TrimEnd('/') + "/sitemap.xml", new[]
            { MediaTypeNames.Text.Html, MediaTypeNames.Text.Plain, MediaTypeNames.Text.RichText }, token);

        _loader.SetExcludedMediaTypes(new[] { MediaTypeNames.Text.Xml });
        var afterProcessor = new AfterCrawlProcessor(_loader, crawledResult, siteMapResult);
        var data = await afterProcessor.ProcessAsync(token);

        new ReportMaker(data).Print();
    }

    private async Task<IEnumerable<CrawlItem>> ProcessAsync(ILinkParser parser, string url,
        IEnumerable<string> mediaTypes, CancellationToken token = default)
    {
        var uri = new Uri(url);
        var loader = _loader
            .SetExcludedMediaTypes(mediaTypes);
        var crawler = new Crawler(loader, parser, 10);
        return await crawler.CrawlAsync(uri, token);
    }
}