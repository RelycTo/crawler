using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Crawler.Shared.Models;

namespace Crawler.DataAccess.Models;

public class CrawlInfo
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string TargetUrl { get; set; }
    public CrawlStatus Status { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedUtc { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedUtc { get; set; }
    public IEnumerable<CrawlDetail>? Details { get; set; }
}
