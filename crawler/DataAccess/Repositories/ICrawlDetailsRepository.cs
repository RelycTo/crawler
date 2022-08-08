using DataAccess.Models;

namespace DataAccess.Repositories;

public interface ICrawlDetailsRepository : IRepository<CrawlDetail>
{
    public Task<IEnumerable<CrawlDetail>> GetAllAsync(int crawlId, CancellationToken token);
    public Task BulkInsertAsync(IEnumerable<CrawlDetail> entities, int batchSize, CancellationToken token);
}