using Crawler.Application.Infrastructure;

namespace Crawler.Application.Crawl.Processors;

public interface ILinkProcessor<T> where T : ILinkParser
{
    Task<IEnumerable<CrawlItemDto>> CrawlAsync(CrawlOptionsDto options, CancellationToken token = default);
}