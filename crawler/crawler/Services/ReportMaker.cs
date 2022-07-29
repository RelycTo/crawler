using crawler.Models;

namespace crawler.Services;

public class ReportMaker
{
    private readonly IEnumerable<ResultItem> _items;

    public ReportMaker(IEnumerable<ResultItem> items)
    {
        _items = items;
    }

    public void Print()
    {
        var sections = GetReportSections();

        foreach (var section in sections)
        {
            Console.WriteLine(section.Title);
            Console.WriteLine($"\t{section.RowHeader}");
            foreach (var row in section.Rows)
            {
                Console.WriteLine($"{row.Key}\t{row.Value}");
            }
        }
    }
    private IEnumerable<ReportItem> GetReportSections()
    {
        var result = new List<ReportItem>();
        var siteMapLinks = GetDiffData(_items, x => x.IsFoundBySiteMap && !x.IsFoundByCrawler);
        result.Add(GetReportSection(siteMapLinks,
            "Urls FOUNDED IN SITEMAP.XML but not founded after crawling a web site",
            "Url"));

        var crawlLinks = GetDiffData(_items, x => !x.IsFoundBySiteMap && x.IsFoundByCrawler);
        result.Add(GetReportSection(crawlLinks,
            "Urls FOUNDED BY CRAWLING THE WEBSITE but not in sitemap.xml",
            "Url"));

        var statistics = GetStatisticData(_items);
        result.Add(GetReportSection(statistics, "Timing", "Url\t\tTiming(ms)"));
        result.Add(GetFooterReportSection(new Dictionary<string, string>
        {
            { "Urls(html documents) found after crawling a website:", _items.Count(i => i.IsFoundByCrawler).ToString() }
        }));
        result.Add(GetFooterReportSection(new Dictionary<string, string>
        {
            { "Urls found in sitemap:", _items.Count(i => i.IsFoundBySiteMap).ToString() }
        }));
        return result;
    }

    private static Dictionary<string, string> GetDiffData(IEnumerable<ResultItem> items,
        Func<ResultItem, bool> predicate) =>
        items
            .Where(predicate)
            .Select((i, index) => new { i.Url, Index = index + 1 })
            .ToDictionary(k => k.Index.ToString(), v => v.Url);

    private static Dictionary<string, string> GetStatisticData(IEnumerable<ResultItem> items) =>
        items
            .Where(i => i.Duration > 0)
            .OrderBy(i => i.Duration)
            .Select((i, index) => new { Url = i.Url, Duration = i.Duration, Index = index + 1 })
            .ToDictionary(k => $"{k.Index}\t{k.Url}", v => v.Duration.ToString());

    private static ReportItem GetReportSection(Dictionary<string, string> rows, string title, string rowHeader) =>
        new()
        {
            Title = title,
            RowHeader = rowHeader,
            Rows = rows
        };

    private static ReportItem GetFooterReportSection(Dictionary<string, string> rows) =>
        new()
        {
            Title = string.Empty,
            RowHeader = string.Empty,
            Rows = rows
        };
}