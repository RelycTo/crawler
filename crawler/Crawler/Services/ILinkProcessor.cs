using Crawler.Infrastructure;
using Crawler.Models;

namespace Crawler.Services;

public interface ILinkProcessor<T> where T : ILinkParser
{
    Task<IEnumerable<CrawlItem>> ProcessAsync(Uri uri, IReadOnlyCollection<string> excludedMediaTypes, int maxThreads, CancellationToken token = default);
}