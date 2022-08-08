using Crawler.Models;
using Shared.Models;

namespace Crawler.UI.Report;

public class ReportFormatter
{
    private const string SiteMapDiffsTitle = "Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site";
    private const string SiteCrawlDiffsTitle = "Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml";
    private const string CrawlTotalRowKey = "Urls(html documents) found after crawling a website:";
    private const string SiteMapTotalRowKey = "Urls found in sitemap:";

    public virtual IEnumerable<ReportSection> Prepare(IReadOnlyCollection<CrawlItem> items)
    {
        var result = new List<ReportSection>
        {
            GetDiffSection(SiteMapDiffsTitle, new[] { string.Empty, "Url" }, items,
                x => x.SourceType == SourceType.SiteMap),
            GetDiffSection(SiteCrawlDiffsTitle, new[] { string.Empty, "Url" }, items,
                x => x.SourceType == SourceType.Site),
            GetStatisticsSection("Timing", new[] { string.Empty, "Url", "Timing (ms)" }, items),
            GetFooterSection(items)
        };

        return result;
    }

    private static ReportSection GetDiffSection(string title, IEnumerable<string> rowHeader,
        IEnumerable<CrawlItem> items, Func<CrawlItem, bool> predicate)
    {
        var rows = GetDiffRows(items, predicate);

        return new ReportSection(title, rowHeader, rows);
    }

    private static IEnumerable<ReportRow> GetDiffRows(IEnumerable<CrawlItem> items,
        Func<CrawlItem, bool> predicate) =>
        items
            .Where(predicate)
            .Select((i, index) => new ReportRow(index + 1, i.Url, i.Duration));

    private static ReportSection GetStatisticsSection(string title, IEnumerable<string> rowHeader,
        IEnumerable<CrawlItem> items)
    {
        var rows = GetStatisticsRows(items);
        return new ReportSection(title, rowHeader, rows);
    }

    private static IEnumerable<ReportRow> GetStatisticsRows(IEnumerable<CrawlItem> items) =>
        items
            .Where(i => i.Duration > 0)
            .OrderBy(i => i.Duration)
            .Select((i, index) =>
            {
                var truncated = i.Url.Length < 100 ? i.Url : i.Url[..100];
                return new ReportRow(index + 1, truncated, i.Duration);
            });

    private static ReportSection GetFooterSection(IEnumerable<CrawlItem> items)
    {
        var data = items.ToArray();
        var crawlTotalRow = GetTotalRow(CrawlTotalRowKey, data, x => new[] { SourceType.Site, SourceType.Both }.Contains(x.SourceType));
        var siteMapTotalRow = GetTotalRow(SiteMapTotalRowKey, data, x => new[] { SourceType.SiteMap, SourceType.Both }.Contains(x.SourceType));
        return new ReportSection("Summary", Array.Empty<string>(), new[] { crawlTotalRow, siteMapTotalRow });
    }

    private static ReportRow GetTotalRow(string key, IEnumerable<CrawlItem> items,
        Func<CrawlItem, bool> predicate) =>
        new(-1, key, items.Count(predicate));
}