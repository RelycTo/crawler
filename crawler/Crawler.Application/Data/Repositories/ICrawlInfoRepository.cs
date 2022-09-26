using Crawler.Application.Data;
using Crawler.Application.Models;
using Crawler.Domain.Models;

namespace Crawler.Application.Services.Repositories
{
    public interface ICrawlInfoRepository
    {
        Task<int> InsertAsync(CrawlInfoDto dto, CancellationToken token);
        Task<CrawlInfo> GetAsync(int id, CancellationToken token);
        Task UpdateAsync(CrawlInfo entity, CancellationToken token);
        Task<IEnumerable<CrawlInfo>> GetAllAsync(CancellationToken token);
    }
}