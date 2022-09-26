using System.Collections.Concurrent;
using System.Net;
using Crawler.Application.Infrastructure;
using Crawler.Domain.Models.Enums;

namespace Crawler.Application.Crawl.Processors;

public abstract class BaseLinkProcessor<T> : ILinkProcessor<T> where T : ILinkParser
{
    private readonly IPageLoader _pageLoader;
    private readonly ILinkParser _parser;
    protected readonly LinkRestorer LinkRestorer;
    protected readonly ConcurrentDictionary<string, CrawlItemDto> ProcessedLinks;
    protected readonly ConcurrentQueue<Uri> Queue;

    protected BaseLinkProcessor(IPageLoader pageLoader, T parser, LinkRestorer linkRestorer)
    {
        _parser = parser;
        _pageLoader = pageLoader;
        LinkRestorer = linkRestorer;
        Queue = new ConcurrentQueue<Uri>();
        ProcessedLinks = new ConcurrentDictionary<string, CrawlItemDto>();
    }

    public async Task<IEnumerable<CrawlItemDto>> CrawlAsync(CrawlOptionsDto options, CancellationToken token = default)
    {
        var tasks = new List<Task>();
        Queue.Enqueue(options.Uri);
        for (var i = 0; i < options.TaskCount; i++)
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
                    if (!LinkRestorer.IsLinkAcceptable(currentLink, options.BaseUri) ||
                        ProcessedLinks.ContainsKey(currentLink.AbsoluteUri))
                    {
                        continue;
                    }

                    await ProcessLinkAsync(GetResourceType(options.Step), currentLink, options.ExcludedMediaTypes,
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
}