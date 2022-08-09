using Crawler.Shared.Handlers;
using Crawler.Shared.Models;
using Crawler.UI.Services;

namespace Crawler.UI.Handlers;

public class ReportHandler : AbstractCrawlHandler<CrawlHandlerContext>
{
    private readonly ConsoleReportBuilder _reportBuilder;

    public ReportHandler(ConsoleReportBuilder reportBuilder)
    {
        _reportBuilder = reportBuilder;
    }

    public override async Task Handle(CrawlHandlerContext context, CancellationToken token = default)
    {
        await _reportBuilder.Build(context.CrawlInfo.Id);
        await base.Handle(context, token);
    }
}
