using Crawler.Extensions;
using Crawler.Infrastructure;
using Crawler.Models;

namespace Crawler.Services;

public class SiteMapProcessor : LinkProcessor
{
    public SiteMapProcessor(PageLoader pageLoader, ILinkParser parser,
        IEnumerable<string> excludedMediaTypes, int maxThreads)
        : base(pageLoader, parser, excludedMediaTypes, maxThreads)
    {
    }

    protected override void PopulateLinks(IEnumerable<string> links, Uri baseUri)
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
                ProcessedLinks.TryAdd(uri.AbsoluteUri, new CrawlItem(uri.AbsoluteUri, -1));
                continue;
            }

            Queue.Enqueue(uri);
        }
    }
}
