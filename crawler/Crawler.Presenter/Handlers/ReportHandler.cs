using Crawler.App.DTOs;
using Crawler.Entities.Handlers;
using Crawler.Presenter.Services.ReportServices;

namespace Crawler.Presenter.Handlers;

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
