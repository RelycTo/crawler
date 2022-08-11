using Crawler.Shared.Interfaces;
using Crawler.Shared.Models;

namespace Crawler.UI.Services;

public class CrawlerScheduler
{
    private readonly ICrawlHandler<CrawlHandlerContext> _handler;

    public CrawlerScheduler(ICrawlHandler<CrawlHandlerContext> handler)
    {
        _handler = handler;
    }

    public async Task Run(string url, int taskCount, CancellationToken token)
    {
        var context = CrawlHandlerContext.Create(new Uri(url), taskCount);
        await _handler.Handle(context, token);
    }
}