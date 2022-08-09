using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Crawler.Shared.Models;

namespace Crawler.DataAccess.Models;

public class CrawlDetail
{
    [Key] public int Id { get; set; }
    [Required] public string Address { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    [Required] public SourceType SourceType { get; set; }
    public long Duration { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime CreatedUtc { get; set; }

    public CrawlInfo CrawlInfo { get; set; }
    public int CrawlInfoId { get; set; }
}