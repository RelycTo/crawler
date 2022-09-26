using Crawler.Domain.Models.Enums;

namespace Crawler.Application.Data;

public class CrawlInfoDto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public CrawlStatus Status { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
}
