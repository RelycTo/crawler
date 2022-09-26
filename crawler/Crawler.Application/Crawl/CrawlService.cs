using System.Net.Mime;
using Crawler.Application.Crawl.Processors;
using Crawler.Application.Data;
using Crawler.Application.Infrastructure;
using Crawler.Domain.Models.Enums;
using Microsoft.Extensions.Logging;

namespace Crawler.Application.Crawl;

public sealed class CrawlService
{
    private readonly CrawlDataService _dataService;
    private readonly ILinkProcessor<IHtmlParser> _pageProcessor;
    private readonly ILinkProcessor<IXmlParser> _sitemapProcessor;
    private readonly PostProcessor _postProcessor;
    private readonly ILogger<CrawlService> _logger;

    public CrawlService(CrawlDataService dataService,
        ILinkProcessor<IHtmlParser> pageProcessor,
        ILinkProcessor<IXmlParser> sitemapProcessor,
        PostProcessor postProcessor,
        ILogger<CrawlService> logger)
    {
        _dataService = dataService;
        _pageProcessor = pageProcessor;
        _sitemapProcessor = sitemapProcessor;
        _postProcessor = postProcessor;
        _logger = logger;
    }

    public ProcessStep Step { get; private set; }

    public async Task<CrawlInfoDto> RunAsync(string url, int taskCount, string sitemapPageName, CancellationToken token)
    {
        _logger.LogInformation("Crawl process started for url: {Url} with task count: {S}", url, taskCount.ToString());
        var crawlInfo = await StartProcess(url, token);

        ChangeStep(ProcessStep.Site);
        var siteCrawlResult = await CrawlSite(url, taskCount, Step, token);

        ChangeStep(ProcessStep.SiteMap);
        var siteMapCrawlResult = await CrawlSiteMap(url, sitemapPageName, taskCount, Step, token);

        ChangeStep(ProcessStep.PostProcess);
        var postProcessResult = await _postProcessor.ProcessAsync(siteCrawlResult, siteMapCrawlResult,
            GetExcludedMediaTypes(ProcessStep.PostProcess), token);

        ChangeStep(ProcessStep.Persist);
        await _dataService.SaveCrawlData(crawlInfo.Id, postProcessResult, token);

        ChangeStep(ProcessStep.Complete);

        _logger.LogInformation("Crawl process completed for info Id: {CrawlInfoId}", crawlInfo.Id);
        return crawlInfo;
    }

    private async Task<CrawlInfoDto> StartProcess(string url, CancellationToken token)
    {
        var crawlInfo = new CrawlInfoDto
        {
            Url = url,
            Status = CrawlStatus.InProgress
        };

        crawlInfo.Id = await _dataService.AddCrawlInfo(crawlInfo, token);

        ChangeStep(ProcessStep.Start);

        return crawlInfo;
    }

    private async Task<IEnumerable<CrawlItemDto>> CrawlSite(string url, int taskCount, ProcessStep step,
        CancellationToken token)
    {
        var options = new CrawlOptionsDto(url, url, taskCount, step, GetExcludedMediaTypes(step));
        return await _pageProcessor.CrawlAsync(options, token);
    }

    private async Task<IEnumerable<CrawlItemDto>> CrawlSiteMap(string url, string siteMap, int taskCount,
        ProcessStep step, CancellationToken token)
    {
        var siteMapUrl = url.TrimEnd('/') + '/' + siteMap;
        var options = new CrawlOptionsDto(siteMapUrl, url, taskCount, step, GetExcludedMediaTypes(step));
        return await _sitemapProcessor.CrawlAsync(options, token);
    }

    private void ChangeStep(ProcessStep step)
    {
        Step = step;
    }

    private static IReadOnlyCollection<string> GetExcludedMediaTypes(ProcessStep step)
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