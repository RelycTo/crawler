using Crawler.App.DTOs;
using Crawler.Entities.Handlers;

namespace Crawler.Presenter.Services;

public class CrawlerScheduler
{
    private readonly ICrawlHandler<CrawlHandlerContext> _handler;

    public CrawlerScheduler(ICrawlHandler<CrawlHandlerContext> handler)
    {
        _handler = handler;
    }

    public async Task RunAsync(string url, int taskCount, CancellationToken token = default)
    {
        var context = CrawlHandlerContext.Create(new Uri(url), taskCount);
        await _handler.Handle(context, token);
    }
}