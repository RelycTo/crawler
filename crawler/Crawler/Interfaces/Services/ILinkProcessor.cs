using Crawler.Infrastructure;
using Crawler.Interfaces.HandlerRequests;
using Crawler.Models;

namespace Crawler.Interfaces.Services;

public interface ILinkProcessor
{
    Task<IEnumerable<CrawlItem>> ProcessAsync(ICrawlRequest request, CancellationToken token = default);
}

public interface ILinkProcessor<T> : ILinkProcessor where T : ILinkParser
{

}