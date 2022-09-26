using Crawler.Application.Infrastructure;
using Crawler.Application.Models;
using Crawler.Domain.Models.Enums;

namespace Crawler.Application.Services.Processors
{
    public interface ILinkProcessor<T> where T : ILinkParser
    {
        Task<IEnumerable<CrawlItemDto>> CrawlAsync(Uri uri, Uri baseUri, int taskCount, IReadOnlyCollection<string> excludedMediaTypes, ProcessStep step, CancellationToken token = default);
    }
}