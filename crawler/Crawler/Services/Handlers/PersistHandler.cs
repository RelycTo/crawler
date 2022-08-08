using DataAccess.Models;
using DataAccess.Repositories;
using Shared.Models;

namespace Crawler.Services.Handlers;

public class PersistHandler : AbstractCrawlHandler<CrawlHandlerContext>
{
    private readonly ICrawlInfoRepository _infoRepository;
    private readonly ICrawlDetailsRepository _detailRepository;

    public PersistHandler(ICrawlInfoRepository infoRepository, ICrawlDetailsRepository detailRepository)
    {
        _infoRepository = infoRepository;
        _detailRepository = detailRepository;
    }

    public override async Task Handle(CrawlHandlerContext context, CancellationToken token = default)
    {
        await InsertCrawlDetailsAsync(context.CrawlInfo.Id, context.ProcessedItems[ProcessStep.PostProcess], token);
        await UpdateCrawlInfoAsync(context.CrawlInfo.Id, token);
        await base.Handle(context, token);
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