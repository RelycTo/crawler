using Crawler.Application.Models;

namespace Crawler.Application.Services.Loaders;

public interface IPageLoader
{
    Task<PageLoaderResponse> GetResponseAsync(Uri uri, IEnumerable<string> excludedMediaTypes,
        CancellationToken token = default);
}