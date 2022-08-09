using System.Net;

namespace Crawler.Shared.Models;

public class CrawlDetailDto
{
    public int Id { get; set; }
    public string Address { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public SourceType SourceType { get; set; }
    public long Duration { get; set; }
    public DateTime CreatedUtc { get; set; }
    public int CrawlInfoId { get; set; }
}