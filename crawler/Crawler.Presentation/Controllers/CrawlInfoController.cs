using Crawler.Application.Models;
using Crawler.Application.Services;
using Crawler.Domain.Handlers;
using Crawler.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crawler.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class CrawlInfoController : ControllerBase
{
    private readonly CrawlDataService _dataService;
    private readonly ILogger<CrawlInfoController> _logger;
    private readonly ICrawlHandler<CrawlHandlerContext> _processHandler;

    public CrawlInfoController(CrawlDataService dataService, ICrawlHandler<CrawlHandlerContext> processHandler,
        ILogger<CrawlInfoController> logger)
    {
        _logger = logger;
        _dataService = dataService;
        _processHandler = processHandler;
    }

    /// <summary>
    /// Get collection of <see cref="CrawlInfoDto"/>.
    /// </summary>
    /// <param name="token">Cancellation token</param>
    /// <returns>Collection of <see cref="CrawlInfoDto"/></returns>
    [HttpGet]
    public async Task<IEnumerable<CrawlInfoDto>> GetCrawlInfosAsync(CancellationToken token = default)
    {
        return await _dataService.GetCrawlInfosAsync(token);
    }

    /// <summary>
    /// Handle crawl process.
    /// </summary>
    /// <param name="request">Crawl request model</param>
    /// <param name="token">Cancellation toke</param>
    /// <returns></returns>
    [HttpPost]
    public async Task ProcessAsync([FromBody] CrawlProcessRequest request, CancellationToken token = default)
    {
        var context = CrawlHandlerContext.Create(request.ProcessUri, request.TaskCount, request.SiteMapPageName);
        await _processHandler.Handle(context, token);
    }
}