using Crawler.Interfaces.Services;
using Shared.Models;

namespace Crawler.Services;

public class CrawlerApp
{
    private readonly ICrawlHandler<CrawlHandlerContext> _handler;

    public CrawlerApp(ICrawlHandler<CrawlHandlerContext> handler)
    {
        _handler = handler;
    }

    public async Task Run(string url, int taskCount, CancellationToken token)
    {
        var context = CrawlHandlerContext.Create(new Uri(url), taskCount);
        await _handler.Handle(context, token);
    }
}