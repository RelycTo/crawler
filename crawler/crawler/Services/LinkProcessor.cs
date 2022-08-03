using Crawler.Extensions;
using Crawler.Infrastructure;

namespace Crawler.Services;

public class LinkProcessor: BaseLinkProcessor<HtmlLinkParser>
{
    public LinkProcessor(PageLoader pageLoader, HtmlLinkParser parser) : base(pageLoader, parser) { }

    protected sealed override void PopulateLinks(IEnumerable<string> links, Uri baseUri)
    {
        foreach (var link in links)
        {
            var restored = link.RestoreAbsolutePath(baseUri);
            if (!Uri.IsWellFormedUriString(restored, UriKind.Absolute))
                continue;
            var uri = new Uri(restored);
            if (ProcessedLinks.ContainsKey(uri.AbsoluteUri.TrimEnd('/'))
                || link.EndsWith(".xml") || !uri.IsLinkAcceptable(baseUri))
                continue;
            Queue.Enqueue(uri);
        }
    }
}