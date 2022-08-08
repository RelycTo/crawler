using Crawler.Interfaces.HandlerRequests;
using Crawler.Interfaces.Services;
using Crawler.Models;
using Crawler.Models.CrawlRequests;
using Shared.Models;

namespace Crawler.Services.Handlers;

public class PostProcessCrawlHandler : AbstractCrawlHandler<ICrawlRequest>
{
    private readonly ILinkProcessor _processor;

    public PostProcessCrawlHandler(ILinkProcessor processor)
    {
        _processor = processor;
    }

    public override async Task Handle(ICrawlRequest request, CancellationToken token = default)
    {
        var processed = await _processor.ProcessAsync(request, token);
        var nextRequest = GetNextRequest(request.CrawlId, processed, request.CrawlInfo);
        await base.Handle(nextRequest, token);
    }

    private static IDataProcessRequest GetNextRequest(int crawlId, IEnumerable<CrawlItem> items, CrawlInfoDto dto) =>
        new DataProcessRequest(crawlId, ProcessStep.Persist, items, dto);
}