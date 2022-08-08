using Crawler.Interfaces.Services;
using Shared.Models;

namespace Crawler.Services.Handlers;

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

    private static void UpdateContext(CrawlHandlerContext context, IEnumerable<CrawlItem> items)
    {
        context
            .SetStep(context.Step + 1)
            .SetProcessedItems(context.Step, items);
    }
}