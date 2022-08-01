using Crawler.Models;

namespace Crawler.Infrastructure;

public class ReportFormatter
{
    private const string SiteMapDiffsTitle = "Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site";
    private const string SiteCrawlDiffsTitle = "Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml";
    private const string CrawlTotalRowKey = "Urls(html documents) found after crawling a website:";
    private const string SiteMapTotalRowKey = "Urls found in sitemap:";

    public virtual IEnumerable<ReportSection> Prepare(IReadOnlyCollection<ResultItem> items)
    {
        var result = new List<ReportSection>
        {
            GetDiffSection(SiteMapDiffsTitle, new[] { string.Empty, "Url" }, items,
                x => x.IsFoundBySiteMap && !x.IsFoundByCrawler),
            GetDiffSection(SiteCrawlDiffsTitle, new[] { string.Empty, "Url" }, items,
                x => !x.IsFoundBySiteMap && x.IsFoundByCrawler),
            GetStatisticsSection("Timing", new[] { string.Empty, "Url", "Timing (ms)" }, items),
            GetFooterSection(items)
        };

        return result;
    }

    private static ReportSection GetDiffSection(string title, IEnumerable<string> rowHeader,
        IEnumerable<ResultItem> items, Func<ResultItem, bool> predicate)
    {
        var rows = GetDiffRows(items, predicate);

        return new ReportSection(title, rowHeader, rows);
    }

    private static IEnumerable<ReportRow> GetDiffRows(IEnumerable<ResultItem> items,
        Func<ResultItem, bool> predicate) =>
        items
            .Where(predicate)
            .Select((i, index) => new ReportRow(index + 1, i.Url, i.Duration));

    private static ReportSection GetStatisticsSection(string title, IEnumerable<string> rowHeader,
        IEnumerable<ResultItem> items)
    {
        var rows = GetStatisticsRows(items);
        return new ReportSection(title, rowHeader, rows);
    }

    private static IEnumerable<ReportRow> GetStatisticsRows(IEnumerable<ResultItem> items) =>
        items
            .Where(i => i.Duration > 0)
            .OrderBy(i => i.Duration)
            .Select((i, index) =>
            {
                var truncated = i.Url.Length < 100 ? i.Url : i.Url[..100];
                return new ReportRow(index + 1, truncated, i.Duration);
            });

    private static ReportSection GetFooterSection(IEnumerable<ResultItem> items)
    {
        var data = items.ToArray();
        var crawlTotalRow = GetTotalRow(CrawlTotalRowKey, data, x => x.IsFoundByCrawler);
        var siteMapTotalRow = GetTotalRow(SiteMapTotalRowKey, data, x => x.IsFoundBySiteMap);
        return new ReportSection("Summary", Array.Empty<string>(), new[] { crawlTotalRow, siteMapTotalRow });
    }

    private static ReportRow GetTotalRow(string key, IEnumerable<ResultItem> items,
        Func<ResultItem, bool> predicate) =>
        new(-1, key, items.Count(predicate));
}