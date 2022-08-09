using Crawler.DataAccess.Models;

namespace Crawler.DataAccess.Repositories;

public interface ICrawlDetailsRepository : IRepository<CrawlDetail>
{
    public Task<IEnumerable<CrawlDetail>> GetCrawlDetailsAsync(int crawlId, CancellationToken token);
    public Task BulkInsertAsync(IEnumerable<CrawlDetail> entities, int batchSize, CancellationToken token);
}