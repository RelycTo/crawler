using Crawler.App.DTOs;
using Crawler.App.Services.Repositories;

namespace Crawler.App.Services;

public class CrawlDataService : ICrawlDataService
{
    private readonly ICrawlInfoRepository _infoRepository;
    private readonly ICrawlDetailsRepository _detailsRepository;

    public CrawlDataService(ICrawlInfoRepository infoRepository, ICrawlDetailsRepository detailsRepository)
    {
        _infoRepository = infoRepository;
        _detailsRepository = detailsRepository;
    }

    public async Task<IEnumerable<CrawlInfoDto>> GetCrawlInfosAsync(CancellationToken token = default)
    {
        var result = await _infoRepository.GetAllAsync(token);
        if (result == null)
            return new List<CrawlInfoDto>();
        return result
            .Select(r => new CrawlInfoDto
            {
                Id = r.Id,
                Status = r.Status,
                Url = r.TargetUrl,
                CreatedUtc = r.CreatedUtc,
                UpdatedUtc = r.UpdatedUtc,
            });
    }

    public async Task<IEnumerable<CrawlDetailDto>> GetCrawlsDetailsAsync(int crawlId, CancellationToken token = default)
    {
        var result = await _detailsRepository.GetCrawlDetailsAsync(crawlId, token);
        if (result == null)
            return new List<CrawlDetailDto>();

        return result
            .Select(r => new CrawlDetailDto
            {
                Id = r.Id,
                Address = r.Address,
                SourceType = r.SourceType,
                Duration = r.Duration,
                CrawlInfoId = r.CrawlInfoId,
                CreatedUtc = r.CreatedUtc,
                StatusCode = r.StatusCode
            });
    }
}