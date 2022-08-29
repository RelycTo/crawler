﻿using System.Net;
using Crawler.App.DTOs;
using Crawler.App.Infrastructure;
using Crawler.App.Infrastructure.Loaders;
using Crawler.App.Infrastructure.Parsers;
using Crawler.Entities.Models.Enums;

namespace Crawler.App.Services.Processors;

public class SiteMapProcessor : BaseLinkProcessor<XmlLinkParser>
{
    public SiteMapProcessor(PageLoader pageLoader, XmlLinkParser parser, LinkRestorer restorer) : base(pageLoader,
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
                ProcessedLinks.TryAdd(uri.AbsoluteUri, new CrawlItem(SourceType.SiteMap, uri.AbsoluteUri, -1, HttpStatusCode.OK));
                continue;
            }

            Queue.Enqueue(uri);
        }
    }
}
