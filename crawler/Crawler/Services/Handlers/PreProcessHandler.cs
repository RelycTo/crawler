using Crawler.Interfaces.HandlerRequests;
using Crawler.Models;
using Crawler.Models.CrawlRequests;
using DataAccess.Repositories;
using Shared.Models;

namespace Crawler.Services.Handlers;

public class PreProcessHandler : AbstractCrawlHandler<ICrawlRequest>
{
    private readonly ICrawlInfoRepository _repository;

    public PreProcessHandler(ICrawlInfoRepository repository)
    {
        _repository = repository;
    }

    public override async Task Handle(ICrawlRequest request, CancellationToken token = default)
    {
        var crawlId = await _repository.InsertAsync(request.CrawlInfo, token);
        var nextRequest = GetNextRequest(request.CrawlInfo, crawlId, request.TaskCount);
        await base.Handle(nextRequest, token);
    }

    private static ICrawlProcessRequest GetNextRequest(CrawlInfoDto dto, int crawlId, int taskCount) =>
        new CrawlProcessRequest(crawlId, ProcessStep.Site, new Uri(dto.Url), taskCount, dto);
}
