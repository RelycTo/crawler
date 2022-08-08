using Crawler.Interfaces.HandlerRequests;
using Crawler.Models;
using DataAccess.Models;
using DataAccess.Repositories;
using Shared.Models;

namespace Crawler.Services.Handlers;

public class CrawlPersistHandler : AbstractCrawlHandler<ICrawlRequest>
{
    private readonly ICrawlInfoRepository _infoRepository;
    private readonly ICrawlDetailsRepository _detailRepository;

    public CrawlPersistHandler(ICrawlInfoRepository infoRepository, ICrawlDetailsRepository detailRepository)
    {
        _infoRepository = infoRepository;
        _detailRepository = detailRepository;
    }

    public override async Task Handle(ICrawlRequest request, CancellationToken token = default)
    {
        if (request is not IDataProcessRequest dataProcessRequest)
            throw new InvalidOperationException($"Unsupported type of request: {request.GetType()}");
        await InsertCrawlDetailsAsync(request.CrawlId, dataProcessRequest.CrawledItems, token);
        await UpdateCrawlInfoAsync(request.CrawlId, token);
        await base.Handle(request, token);
    }

    private async Task InsertCrawlDetailsAsync(int crawlId, IEnumerable<CrawlItem> items, CancellationToken token)
    {
        await _detailRepository.BulkInsertAsync(items.Select(i => new CrawlDetail
        {
            CrawlId = crawlId,
            Address = i.Url,
            Duration = i.Duration,
            SourceType = i.SourceType
        }), 100, token);
    }

    private async Task UpdateCrawlInfoAsync(int id, CancellationToken token)
    {
        var entity = await _infoRepository.GetAsync(id, token);
        entity.Status = CrawlStatus.Success;
        await _infoRepository.UpdateAsync(entity, token);
    }
}
