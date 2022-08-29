using Crawler.App.Infrastructure;
using Crawler.App.Infrastructure.Loaders;
using Crawler.App.Infrastructure.Parsers;

namespace Crawler.App.Services.Processors;

public class PageProcessor : BaseLinkProcessor<HtmlLinkParser>
{
    public PageProcessor(PageLoader pageLoader, HtmlLinkParser parser, LinkRestorer linkRestorer)
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