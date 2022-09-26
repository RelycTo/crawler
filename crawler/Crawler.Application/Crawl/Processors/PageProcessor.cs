using Crawler.Application.Infrastructure;
using Crawler.Application.Services.Utilities;

namespace Crawler.Application.Services.Processors
{
    public class PageProcessor : BaseLinkProcessor<IHtmlParser>
    {
        public PageProcessor(IPageLoader pageLoader, IHtmlParser parser, ILinkRestorer linkRestorer)
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
}