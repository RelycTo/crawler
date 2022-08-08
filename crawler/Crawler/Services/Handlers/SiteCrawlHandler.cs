using System.Net.Mime;
using Crawler.Interfaces;
using Crawler.Interfaces.Services;
using Shared.Models;

namespace Crawler.Services.Handlers;

public class SiteCrawlHandler<TParser> : AbstractCrawlHandler<CrawlHandlerContext> where TParser: ILinkParser
{
    private readonly ILinkProcessor<TParser> _processor;

    public SiteCrawlHandler(ILinkProcessor<TParser> processor)
    {
        _processor = processor;
    }

    public override async Task Handle(CrawlHandlerContext context, CancellationToken token = default)
    {
        var crawled = await _processor.ProcessAsync(context, token);
        UpdateContext(context, crawled);
        await base.Handle(context, token);
    }

    private static void UpdateContext(CrawlHandlerContext context, IEnumerable<CrawlItem> items)
    {
        var currentStep = context.Step;
        var nextStep = currentStep + 1;
        context
            .SetStep(nextStep)
            .SetProcessedItems(currentStep, items)
            .Options
            .SetExcludedMediaTypes(GetExcludedMediaTypes(currentStep));
    }

    private static IEnumerable<string> GetExcludedMediaTypes(ProcessStep step)
    {
        if (step != ProcessStep.SiteMap)
        {
            return new[] { MediaTypeNames.Text.Xml };
        }

        return new[]
        {
            MediaTypeNames.Text.Html, MediaTypeNames.Text.Plain, MediaTypeNames.Text.RichText
        };
    }
}