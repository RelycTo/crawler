using DataAccess.Repositories;
using Shared.Models;

namespace Crawler.Services.Handlers;

public class PreProcessHandler : AbstractCrawlHandler<CrawlHandlerContext>
{
    private readonly ICrawlInfoRepository _repository;

    public PreProcessHandler(ICrawlInfoRepository repository)
    {
        _repository = repository;
    }

    public override async Task Handle(CrawlHandlerContext context, CancellationToken token = default)
    {
        var crawlId = await _repository.InsertAsync(context.CrawlInfo, token);
        UpdateContext(context, crawlId);
        await base.Handle(context, token);
    }

    private static void UpdateContext(CrawlHandlerContext context, int crawlId)
    {
        context.SetStep(context.Step + 1);
        context.CrawlInfo.Id = crawlId;
    }
}