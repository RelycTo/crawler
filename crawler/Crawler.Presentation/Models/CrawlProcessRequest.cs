using System.ComponentModel.DataAnnotations;

namespace Crawler.Presentation.Models;

public class CrawlProcessRequest
{
    [Required]
    [Url]
    public string Url { get; set; }

    public int TaskCount { get; set; } = 10;
    public string SiteMapPageName { get; set; } = "sitemap.xml";

    public Uri ProcessUri => new(Url);
}
