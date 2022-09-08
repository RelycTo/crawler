using Crawler.Application.Models;
using Crawler.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Crawler.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrawlDetailsController : ControllerBase
    {
        private readonly ILogger<CrawlDetailsController> _logger;
        private readonly CrawlDataService _dataService;

        public CrawlDetailsController(CrawlDataService dataService, ILogger<CrawlDetailsController> logger)
        {
            _logger = logger;
            _dataService = dataService;
        }

        /// <summary>
        /// Gets collection of <see cref="CrawlDetailDto"/>
        /// </summary>
        /// <param name="crawlId">Id of crawled process</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>Collection of <see cref="CrawlDetailDto"/></returns>
        [HttpGet("{crawlId}")]
        public async Task<IEnumerable<CrawlDetailDto>> GetDetailsAsync(int crawlId, CancellationToken token = default)
        {
            return await _dataService.GetCrawlsDetailsAsync(crawlId, token);
        }
    }
}