using Crawler.Application.Infrastructure;

namespace Crawler.Application.Crawl.Processors;

public class PageProcessor : BaseLinkProcessor<IHtmlParser>
{
    public PageProcessor(IPageLoader pageLoader, IHtmlParser parser, LinkRestorer linkRestorer)
        : base(pageLoader, parser, linkRestorer)
    {
    }

    protected sealed override void PopulateLinks(IEnumerable<string> links, Uri baseUri)
    {
        foreach (var link in links)
        {
            var restored = LinkRestorer.RestoreAbsolutePath(link, baseUri);
            if (!Uri.IsWellFormedUriString(restored, UriKind.Absolute))
            {
                continue;
            }

            var uri = new Uri(restored);
            if (ProcessedLinks.ContainsKey(uri.AbsoluteUri.TrimEnd('/')) ||
                link.EndsWith(".xml") || !LinkRestorer.IsLinkAcceptable(uri, baseUri))
            {
                continue;
            }

            Queue.Enqueue(uri);
        }
    }
}