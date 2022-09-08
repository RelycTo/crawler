using System.Collections.Concurrent;
using System.Net;
using Crawler.Application.Models;
using Crawler.Application.Services.Loaders;
using Crawler.Application.Services.Parsers;
using Crawler.Application.Services.Utilities;
using Crawler.Domain.Models.Enums;

namespace Crawler.Application.Services.Processors;

public abstract class BaseLinkProcessor<T> : ILinkProcessor<T> where T : ILinkParser
{
    private readonly IPageLoader _pageLoader;
    private readonly ILinkParser _parser;
    protected readonly ILinkRestorer LinkRestorer;
    protected readonly ConcurrentDictionary<string, CrawlItemDto> ProcessedLinks;
    protected readonly ConcurrentQueue<Uri> Queue;

    protected BaseLinkProcessor(IPageLoader pageLoader, T parser, ILinkRestorer linkRestorer)
    {
        _parser = parser;
        _pageLoader = pageLoader;
        LinkRestorer = linkRestorer;
        Queue = new ConcurrentQueue<Uri>();
        ProcessedLinks = new ConcurrentDictionary<string, CrawlItemDto>();
    }

    public async Task<IEnumerable<CrawlItemDto>> ProcessAsync(CrawlHandlerContext context, CancellationToken token = default)
    {
        var tasks = new List<Task>();
        var uri = GetStepUri(context);
        Queue.Enqueue(uri);
        for (var i = 0; i < context.Options.TaskCount; i++)
        {
            var isFirst = i == 0;
            tasks.Add(Task.Run(async () =>
            {
                if (!isFirst)
                {
                    await Task.Delay(3000, token);
                }

                while (Queue.TryDequeue(out var currentLink))
                {
                    if (!LinkRestorer.IsLinkAcceptable(currentLink, context.Options.Uri) ||
                        ProcessedLinks.ContainsKey(currentLink.AbsoluteUri))
                    {
                        continue;
                    }

                    await ProcessLinkAsync(GetResourceType(context.Step), currentLink, context.Options.ExcludedMediaTypes,
                        token);
                }
            }, token));
        }

        await Task.WhenAll(tasks);

        return ProcessedLinks.Values.Where(v => !v.Url.EndsWith(".xml"));
    }

    private async Task ProcessLinkAsync(SourceType sourceType, Uri uri, IEnumerable<string> excludedMediaTypes,
        CancellationToken token)
    {
        var response = await _pageLoader.GetResponseAsync(uri, excludedMediaTypes, token);

        ProcessedLinks.TryAdd(uri.AbsoluteUri,
            new CrawlItemDto(sourceType, uri.AbsoluteUri, response.Duration, response.StatusCode));
        if (response.Duration >= 0)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var links = _parser.GetLinks(response.Content);
                PopulateLinks(links, uri);
            }
        }
    }

    protected abstract void PopulateLinks(IEnumerable<string> links, Uri baseUri);

    private static SourceType GetResourceType(ProcessStep step) =>
        step switch
        {
            ProcessStep.Site => SourceType.Site,
            ProcessStep.SiteMap => SourceType.SiteMap,
            _ => throw new ArgumentOutOfRangeException(nameof(step), step, $"Unhandled process step occurred: {step}")
        };

    private static Uri GetStepUri(CrawlHandlerContext context)
    {
        return context.Step switch
        {
            ProcessStep.Site => context.Options.Uri,
            ProcessStep.SiteMap => context.Options.SiteMapUri,
            _ => throw new InvalidOperationException($"Wrong step {context.Step} for crawl phase occurred")
        };
    }
}