using Crawler.App.DTOs;
using Crawler.Entities.Models.Enums;
using Crawler.Presenter.Models;

namespace Crawler.Presenter.Services.ReportServices;

public class ReportFormatter
{
    private const string SiteMapDiffsTitle = "Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site";
    private const string SiteCrawlDiffsTitle = "Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml";
    private const string CrawlTotalRowKey = "Urls(html documents) found after crawling a website:";
    private const string SiteMapTotalRowKey = "Urls found in sitemap:";

    public virtual IEnumerable<ReportSection> Prepare(IReadOnlyCollection<CrawlDetailDto> items)
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
        IEnumerable<CrawlDetailDto> items, Func<CrawlDetailDto, bool> predicate)
    {
        var rows = GetDiffRows(items, predicate);

        return new ReportSection(title, rowHeader, rows);
    }

    private static IEnumerable<ReportRow> GetDiffRows(IEnumerable<CrawlDetailDto> items,
        Func<CrawlDetailDto, bool> predicate) =>
        items
            .Where(predicate)
            .Select((i, index) => new ReportRow(index + 1, i.Address, i.Duration));

    private static ReportSection GetStatisticsSection(string title, IEnumerable<string> rowHeader,
        IEnumerable<CrawlDetailDto> items)
    {
        var rows = GetStatisticsRows(items);
        return new ReportSection(title, rowHeader, rows);
    }

    private static IEnumerable<ReportRow> GetStatisticsRows(IEnumerable<CrawlDetailDto> items) =>
        items
            .Where(i => i.Duration > 0)
            .OrderBy(i => i.Duration)
            .Select((i, index) =>
            {
                var truncated = i.Address.Length < 100 ? i.Address : i.Address[..100];
                return new ReportRow(index + 1, truncated, i.Duration);
            });

    private static ReportSection GetFooterSection(IReadOnlyCollection<CrawlDetailDto> items)
    {
        var crawlTotalRow = GetTotalRow(CrawlTotalRowKey, items, x => new[] { SourceType.Site, SourceType.Both }.Contains(x.SourceType));
        var siteMapTotalRow = GetTotalRow(SiteMapTotalRowKey, items, x => new[] { SourceType.SiteMap, SourceType.Both }.Contains(x.SourceType));
        return new ReportSection("Summary", Array.Empty<string>(), new[] { crawlTotalRow, siteMapTotalRow });
    }

    private static ReportRow GetTotalRow(string key, IEnumerable<CrawlDetailDto> items,
        Func<CrawlDetailDto, bool> predicate) =>
        new(-1, key, items.Count(predicate));
}