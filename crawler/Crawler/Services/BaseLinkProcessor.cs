using System.Collections.Concurrent;
using System.Net;
using Crawler.Extensions;
using Crawler.Infrastructure;
using Crawler.Interfaces.HandlerRequests;
using Crawler.Interfaces.Services;
using Crawler.Models;
using Shared.Models;

namespace Crawler.Services;

public abstract class BaseLinkProcessor<T> : ILinkProcessor<T> where T : ILinkParser
{
    private readonly PageLoader _pageLoader;
    private readonly ILinkParser _parser;
    protected readonly ConcurrentDictionary<string, CrawlItem> ProcessedLinks;
    protected readonly ConcurrentQueue<Uri> Queue;

    protected BaseLinkProcessor(PageLoader pageLoader, T parser)
    {
        _parser = parser;
        _pageLoader = pageLoader;
        Queue = new ConcurrentQueue<Uri>();
        ProcessedLinks = new ConcurrentDictionary<string, CrawlItem>();
    }

    public async Task<IEnumerable<CrawlItem>> ProcessAsync(ICrawlRequest request, CancellationToken token = default)
    {
        if (request is ICrawlProcessRequest crawlRequest)
            return await ProcessAsync(crawlRequest, token);
        throw new InvalidOperationException($"Unsupported type of request: {request.GetType()}");
    }

    private async Task<IEnumerable<CrawlItem>> ProcessAsync(ICrawlProcessRequest request,
        CancellationToken token = default)
    {
        var tasks = new List<Task>();
        Queue.Enqueue(request.Uri);
        for (var i = 0; i < request.TaskCount; i++)
        {
            var isFirst = i == 0;
            tasks.Add(Task.Run(async () =>
            {
                if (!isFirst)
                    await Task.Delay(3000, token);
                while (Queue.TryDequeue(out var currentLink))
                {
                    if (!currentLink.IsLinkAcceptable(request.Uri) ||
                        ProcessedLinks.ContainsKey(currentLink.AbsoluteUri))
                        continue;
                    await ProcessLinkAsync(GetResourceType(request.Step), currentLink, request.ExcludedMediaTypes,
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
            ProcessStep.Start => throw new InvalidOperationException($"Unhandled process step occurred: {step}"),
            ProcessStep.PostProcess => throw new InvalidOperationException($"Unhandled process step occurred: {step}"),
            ProcessStep.Transform => throw new InvalidOperationException($"Unhandled process step occurred: {step}"),
            ProcessStep.Persist => throw new InvalidOperationException($"Unhandled process step occurred: {step}"),
            _ => throw new ArgumentOutOfRangeException(nameof(step), step, $"Unhandled process step occurred: {step}")
        };
}