﻿using Crawler.Entities.Models.Enums;

namespace Crawler.Entities.Models;

public class CrawlInfo
{
    public int Id { get; set; }
    public string TargetUrl { get; set; }
    public CrawlStatus Status { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
    public IEnumerable<CrawlDetail>? Details { get; set; }
}