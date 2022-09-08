using System.Net;
using Crawler.Application.Models;
using Crawler.Application.Services.Loaders;
using Crawler.Application.Services.Parsers;
using Crawler.Application.Services.Utilities;
using Crawler.Domain.Models.Enums;

namespace Crawler.Application.Services.Processors;

public class SiteMapProcessor : BaseLinkProcessor<IXmlParser>
{
    public SiteMapProcessor(IPageLoader pageLoader, IXmlParser parser, ILinkRestorer restorer) : base(pageLoader,
        parser, restorer)
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
            if (ProcessedLinks.ContainsKey(uri.AbsoluteUri))
            {
                continue;
            }

            if (!link.EndsWith(".xml"))
            {
                ProcessedLinks.TryAdd(uri.AbsoluteUri, new CrawlItemDto(SourceType.SiteMap, uri.AbsoluteUri, -1, HttpStatusCode.OK));
                continue;
            }

            Queue.Enqueue(uri);
        }
    }
}
