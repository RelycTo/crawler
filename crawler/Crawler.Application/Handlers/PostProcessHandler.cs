using Crawler.Application.Models;
using Crawler.Application.Services.Processors;
using Crawler.Domain.Handlers;

namespace Crawler.Application.Handlers;

public class PostProcessHandler : AbstractCrawlHandler<CrawlHandlerContext>
{
    private readonly ILinkProcessor _processor;

    public PostProcessHandler(ILinkProcessor processor)
    {
        _processor = processor;
    }

    public override async Task Handle(CrawlHandlerContext context, CancellationToken token = default)
    {
        var processed = await _processor.ProcessAsync(context, token);
        UpdateContext(context, processed);
        await base.Handle(context, token);
    }

    private static void UpdateContext(CrawlHandlerContext context, IEnumerable<CrawlItemDto> items)
    {
        context
            .SetProcessedItems(context.Step, items)
            .SetStep(context.Step + 1);
    }
}