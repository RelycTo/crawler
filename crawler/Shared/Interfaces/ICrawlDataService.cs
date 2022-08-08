using Shared.Models;

namespace Shared.Interfaces;

public interface ICrawlDataService
{
    Task<int> AddCrawlInfoAsync(CrawlInfoDto dto, CancellationToken token = default);
    //Task AddDetailsAsync(IEnumerable<>)
}
