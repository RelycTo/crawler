using Crawler.Application.Models;
using Crawler.Application.Services.Repositories;
using Crawler.Domain.Models;
using Crawler.Domain.Models.Enums;

namespace Crawler.Application.Services;

public class CrawlDataService
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

        return result?
            .Select(r => new CrawlInfoDto
            {
                Id = r.Id,
                Status = r.Status,
                Url = r.TargetUrl,
                CreatedUtc = r.CreatedUtc,
                UpdatedUtc = r.UpdatedUtc,
            }) ?? Array.Empty<CrawlInfoDto>();
    }

    public async Task<CrawlInfoDto> GetCrawlInfoAsync(int id, CancellationToken token = default) {
        var result = await _infoRepository.GetAsync(id, token);

        return result != null
            ? new CrawlInfoDto {
                Id = result.Id,
                Status = result.Status,
                Url = result.TargetUrl,
                CreatedUtc = result.CreatedUtc,
                UpdatedUtc = result.UpdatedUtc,
            }
            : null;
    }

    public async Task<IEnumerable<CrawlDetailDto>> GetCrawlsDetailsAsync(int crawlId, CancellationToken token = default)
    {
        var result = await _detailsRepository.GetCrawlDetailsAsync(crawlId, token);

        return result?
            .Select(r => new CrawlDetailDto {
                Id = r.Id,
                Address = r.Address,
                SourceType = r.SourceType,
                Duration = r.Duration,
                CrawlInfoId = r.CrawlInfoId,
                CreatedUtc = r.CreatedUtc,
                StatusCode = r.StatusCode
            }) ?? Array.Empty<CrawlDetailDto>();
    }

    public async Task<int> AddCrawlInfo(CrawlInfoDto dto, CancellationToken token = default) {
        return await _infoRepository.InsertAsync(dto, token);
    }

    public async Task SaveCrawlData(int crawlId, IEnumerable<CrawlItemDto> items, CancellationToken token = default) {
        await InsertCrawlDetailsAsync(crawlId, items, token);
        await UpdateCrawlInfoAsync(crawlId, token);
    }
    
    private async Task InsertCrawlDetailsAsync(int crawlId, IEnumerable<CrawlItemDto> items, CancellationToken token)
    {
        await _detailsRepository.BulkInsertAsync(items.Select(i => new CrawlDetail
        {
            CrawlInfoId = crawlId,
            Address = i.Url,
            Duration = i.Duration,
            SourceType = i.SourceType
        }), 100, token);
    }
    
    private async Task UpdateCrawlInfoAsync(int id, CancellationToken token)
    {
        var entity = await _infoRepository.GetAsync(id, token);
        if (entity == null)
            throw new InvalidOperationException("Try to change missed crawl info occurred.");
        
        entity.Status = CrawlStatus.Success;
        
        await _infoRepository.UpdateAsync(entity, token);
    }    
}