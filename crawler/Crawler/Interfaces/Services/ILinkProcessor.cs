using Shared.Models;

namespace Crawler.Interfaces.Services;

public interface ILinkProcessor
{
    Task<IEnumerable<CrawlItem>> ProcessAsync(CrawlHandlerContext context, CancellationToken token = default);
}

public interface ILinkProcessor<T> : ILinkProcessor where T : ILinkParser
{

}