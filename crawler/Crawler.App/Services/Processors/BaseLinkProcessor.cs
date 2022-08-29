using System.Collections.Concurrent;
using System.Net;
using Crawler.App.DTOs;
using Crawler.App.Infrastructure;
using Crawler.App.Infrastructure.Loaders;
using Crawler.App.Infrastructure.Parsers;
using Crawler.Entities.Models.Enums;

namespace Crawler.App.Services.Processors;

public abstract class BaseLinkProcessor<T> : ILinkProcessor<T> where T : ILinkParser
{
    private readonly PageLoader _pageLoader;
    private readonly ILinkParser _parser;
    protected readonly LinkRestorer LinkRestorer;
    protected readonly ConcurrentDictionary<string, CrawlItem> ProcessedLinks;
    protected readonly ConcurrentQueue<Uri> Queue;

    protected BaseLinkProcessor(PageLoader pageLoader, T parser, LinkRestorer linkRestorer)
    {
        _parser = parser;
        _pageLoader = pageLoader;
        LinkRestorer = linkRestorer;
        Queue = new ConcurrentQueue<Uri>();
        ProcessedLinks = new ConcurrentDictionary<string, CrawlItem>();
    }

    public async Task<IEnumerable<CrawlItem>> ProcessAsync(CrawlHandlerContext context, CancellationToken token = default)
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
            new CrawlItem(sourceType, uri.AbsoluteUri, response.Duration, response.StatusCode));
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