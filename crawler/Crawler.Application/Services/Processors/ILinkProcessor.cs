using Crawler.Application.Models;
using Crawler.Application.Services.Parsers;

namespace Crawler.Application.Services.Processors;

public interface ILinkProcessor
{
    Task<IEnumerable<CrawlItemDto>> ProcessAsync(CrawlHandlerContext context, CancellationToken token = default);
}

public interface ILinkProcessor<T> : ILinkProcessor where T : ILinkParser
{

}