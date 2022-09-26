using Crawler.Application.Crawl;
using Crawler.Application.Data;
using Crawler.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

namespace Crawler.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class CrawlInfoController : ControllerBase
{
    private readonly CrawlDataService _dataService;
    private readonly CrawlService _service;
    private readonly ILogger<CrawlInfoController> _logger;

    public CrawlInfoController(CrawlDataService dataService, CrawlService service,
        ILogger<CrawlInfoController> logger)
    {
        _logger = logger;
        _dataService = dataService;
        _service = service;
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
    /// Gets <see cref="CrawlInfoDto"/> by Id.
    /// </summary>
    /// <param name="id">Identifier of crawl info</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Collection of <see cref="CrawlInfoDto"/></returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CrawlInfoDto>> GetCrawlInfoAsync(int id, CancellationToken token = default)
    {
        return await _dataService.GetCrawlInfoAsync(id, token);
    }

    /// <summary>
    /// Handle crawl process.
    /// </summary>
    /// <param name="request">Crawl request model</param>
    /// <param name="token">Cancellation toke</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<CrawlInfoDto>> CrawlAsync([FromBody] CrawlProcessRequest request,
        CancellationToken token = default)
    {
        var crawlInfo = await _service.RunAsync(request.Url, request.TaskCount, request.SiteMapPageName, token);

        return CreatedAtAction(nameof(GetCrawlInfoAsync), new { id = crawlInfo.Id }, crawlInfo);
    }
}