using Crawler.Domain.Models;

namespace Crawler.Application.Data.Repositories;

public interface ICrawlDetailsRepository
{
    public Task<IEnumerable<CrawlDetail>> GetCrawlDetailsAsync(int crawlId, CancellationToken token);
    public Task BulkInsertAsync(IEnumerable<CrawlDetail> entities, int batchSize, CancellationToken token);
}