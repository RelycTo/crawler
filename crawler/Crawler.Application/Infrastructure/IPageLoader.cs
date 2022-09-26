namespace Crawler.Application.Infrastructure;

public interface IPageLoader
{
    Task<PageLoaderResponse> GetResponseAsync(Uri uri, IEnumerable<string> excludedMediaTypes,
        CancellationToken token = default);
}