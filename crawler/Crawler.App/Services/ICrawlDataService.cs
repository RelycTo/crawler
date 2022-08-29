﻿using Crawler.App.DTOs;

namespace Crawler.App.Services;

public interface ICrawlDataService
{
    Task<IEnumerable<CrawlInfoDto>> GetCrawlInfosAsync(CancellationToken token = default);
    Task<IEnumerable<CrawlDetailDto>> GetCrawlsDetailsAsync(int crawlId, CancellationToken token = default);
}