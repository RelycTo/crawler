using crawler.Extensions;
using crawler.Models;

namespace crawler.Services;

public class SiteMapProcessor: LinkProcessor
{
    public SiteMapProcessor(PageLoader pageLoader, ILinkParser parser, IEnumerable<Uri> links, int maxThreads) : base(pageLoader, parser, links, maxThreads)
    {
    }

    protected override void PopulateLinks(IEnumerable<string> links, Uri baseUri)
    {
        foreach (var link in links)
        {
            try
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
            catch (UriFormatException e)
            {
                Console.WriteLine(e);
            }
        }
    }

}
