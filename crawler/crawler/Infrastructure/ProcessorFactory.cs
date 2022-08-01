using System.Net.Mime;
using crawler.Models;
using crawler.Services;

namespace crawler.Infrastructure;

//Thread unsafe singleton. But it's ok at the moment
public class ProcessorFactory
{
    private readonly PageLoader _loader;
    private static ProcessorFactory? _processorFactory;

    private ProcessorFactory(PageLoader loader)
    {
        _loader = loader;
    }

    public static ProcessorFactory Create(PageLoader loader)
    {
        return _processorFactory ??= new ProcessorFactory(loader);
    }

    public LinkProcessor CreateLinkProcessor(HtmlLinkParser parser, int maxThreads) =>
        new(_loader, parser, new[] { MediaTypeNames.Text.Xml }, maxThreads);

    public SiteMapProcessor CreateSiteMapProcessor(XmlLinkParser parser, int maxThreads) =>
        new(_loader, parser, new[]
        {
            MediaTypeNames.Text.Html,
            MediaTypeNames.Text.Plain,
            MediaTypeNames.Text.RichText
        }, maxThreads);

    public PostProcessor
        CreatePostProcessor(IEnumerable<CrawlItem> crawledItems, IEnumerable<CrawlItem> siteMapItems) =>
        new(_loader, crawledItems, siteMapItems, new[] { MediaTypeNames.Text.Xml });
}