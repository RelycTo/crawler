using System.Net.Mime;
using Crawler.Infrastructure;
using Crawler.Interfaces.HandlerRequests;
using Crawler.Interfaces.Services;
using Crawler.Models;

namespace Crawler.Services.Handlers;

public class SiteCrawlHandler : AbstractCrawlHandler<ICrawlRequest>
{
    private readonly ILinkProcessor<HtmlLinkParser> _processor;

    public SiteCrawlHandler(ILinkProcessor<HtmlLinkParser> processor)
    {
        _processor = processor;
    }

    public override async Task Handle(ICrawlRequest request, CancellationToken token = default)
    {
        var crawled = await _processor.ProcessAsync(request, token);
        var nextRequest = GetNextRequest(request, crawled);
        await base.Handle(nextRequest, token);
    }

    private static ICrawlProcessRequest GetNextRequest(ICrawlRequest request, IEnumerable<CrawlItem> items)
    {
        if (request is ICrawlProcessRequest crawlRequest)
        {
            crawlRequest
                .SetStep(ProcessStep.SiteMap)
                .SetExcludedMediaTypes(new[]
                    { MediaTypeNames.Text.Html, MediaTypeNames.Text.Plain, MediaTypeNames.Text.RichText })
                .SetProcessedItems(new Dictionary<ProcessStep, IEnumerable<CrawlItem>> { { ProcessStep.Site, items } });
            return crawlRequest;
        }

        throw new InvalidOperationException($"Unsupported type of request: {request.GetType()}");
    }
}
