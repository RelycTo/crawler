using System.Net;
using Crawler.Extensions;
using Crawler.Infrastructure;
using Crawler.Models;
using Shared.Models;

namespace Crawler.Services;

public class SiteMapProcessor : BaseLinkProcessor<XmlLinkParser>
{
    public SiteMapProcessor(PageLoader pageLoader, XmlLinkParser parser) : base(pageLoader, parser) { }

    protected sealed override void PopulateLinks(IEnumerable<string> links, Uri baseUri)
    {
        foreach (var link in links)
        {
            var restored = link.RestoreAbsolutePath(baseUri);
            if (!Uri.IsWellFormedUriString(restored, UriKind.Absolute))
                continue;
            var uri = new Uri(restored);
            if (ProcessedLinks.ContainsKey(uri.AbsoluteUri))
                continue;
            if (!link.EndsWith(".xml"))
            {
                ProcessedLinks.TryAdd(uri.AbsoluteUri, new CrawlItem(SourceType.SiteMap, uri.AbsoluteUri, -1, HttpStatusCode.OK));
                continue;
            }

            Queue.Enqueue(uri);
        }
    }
}
