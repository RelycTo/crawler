using System.Net.Mime;
using Crawler.Application.Models;
using Crawler.Application.Services.Parsers;
using Crawler.Application.Services.Processors;
using Crawler.Domain.Handlers;
using Crawler.Domain.Models.Enums;

namespace Crawler.Application.Handlers;

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

    private static void UpdateContext(CrawlHandlerContext context, IEnumerable<CrawlItemDto> items)
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