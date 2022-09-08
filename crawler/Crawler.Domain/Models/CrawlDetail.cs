﻿using System.Net;
using Crawler.Domain.Models.Enums;

namespace Crawler.Domain.Models;

public class CrawlDetail
{
    public int Id { get; set; }
    public string Address { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public SourceType SourceType { get; set; }
    public long Duration { get; set; }
    public DateTime CreatedUtc { get; set; }
    public CrawlInfo CrawlInfo { get; set; }
    public int CrawlInfoId { get; set; }
}