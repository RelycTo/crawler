using Crawler.App.DTOs;
using Crawler.Entities.Models;

namespace Crawler.App.Services.Repositories;

public interface ICrawlInfoRepository : IRepository<CrawlInfo>
{
    Task<int> InsertAsync(CrawlInfoDto dto, CancellationToken token);
    Task<CrawlInfo> GetAsync(int id, CancellationToken token);
    Task UpdateAsync(CrawlInfo entity, CancellationToken token);
    Task<IEnumerable<CrawlInfo>> GetAllAsync(CancellationToken token);
}