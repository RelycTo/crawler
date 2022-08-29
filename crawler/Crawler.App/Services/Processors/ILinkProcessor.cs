using Crawler.App.DTOs;
using Crawler.App.Infrastructure.Parsers;

namespace Crawler.App.Services.Processors;

public interface ILinkProcessor
{
    Task<IEnumerable<CrawlItem>> ProcessAsync(CrawlHandlerContext context, CancellationToken token = default);
}

public interface ILinkProcessor<T> : ILinkProcessor where T : ILinkParser
{

}