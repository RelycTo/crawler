using Crawler.Shared.Models;

namespace Crawler.Shared.Interfaces;

public interface ICrawlDataService
{
    Task<IEnumerable<CrawlInfoDto>> GetCrawlInfosAsync(CancellationToken token = default);
    Task<IEnumerable<CrawlDetailDto>> GetCrawlsDetailsAsync(int crawlId, CancellationToken token = default);
}
