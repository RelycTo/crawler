using crawler.Models;
using crawler.Services;

namespace crawler;

internal class Crawler
{
    private readonly PageLoader _loader;
    private readonly ILinkParser _parser;
    private readonly int _maxThreads;

    public Crawler(PageLoader loader, ILinkParser parser, int maxThreads)
    {
        _loader = loader;
        _maxThreads = maxThreads;
        _parser = parser;
    }

    public async Task<IEnumerable<CrawlItem>> CrawlAsync(Uri uri, CancellationToken token = default)
    {
        var processor = CreateProcessor(_loader, _parser, new[] { uri }, _maxThreads);
        return await processor.ProcessAsync(uri, token);
    }

    //TODO: should be moved out into processors factory
    private static LinkProcessor CreateProcessor(PageLoader loader, ILinkParser parser, IEnumerable<Uri> links,
        int threadCount)
    {
        return parser is HtmlLinkParser
            ? new LinkProcessor(loader, parser, links, threadCount)
            : new SiteMapProcessor(loader, parser, links, threadCount);
    }
}