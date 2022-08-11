using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Crawler.Shared.Models;

namespace Crawler.DataAccess.Models;

public class CrawlInfo
{
    public int Id { get; set; }
    public string TargetUrl { get; set; }
    public CrawlStatus Status { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
    public IEnumerable<CrawlDetail>? Details { get; set; }
}
