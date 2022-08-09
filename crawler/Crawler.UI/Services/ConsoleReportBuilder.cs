using Crawler.Models;
using Crawler.Shared.Interfaces;

namespace Crawler.UI.Services;

public class ConsoleReportBuilder
{
    private readonly ReportFormatter _formatter;
    private readonly ICrawlDataService _crawlDataService;

    public ConsoleReportBuilder(ICrawlDataService crawlDataService, ReportFormatter formatter)
    {
        _formatter = formatter;
        _crawlDataService = crawlDataService;
    }

    public async Task Build(int crawlId)
    {
        var details = await _crawlDataService.GetCrawlsDetailsAsync(crawlId);
        var sections = _formatter.Prepare(details.ToArray());
        CreateReport(sections);
    }

    private static void CreateReport(IEnumerable<ReportSection> sections)
    {
        foreach (var section in sections)
        {
            if (section?.Rows == null || !section.Rows.Any())
                continue;

            Console.WriteLine(section.Title);
            var index = 0;
            foreach (var column in section.RowHeader)
            {
                if (string.IsNullOrEmpty(column))
                    Console.Write("\t");
                else
                {
                    Console.Write(index % 2 == 0 ? $"|{column,-100}" : $"|{column,-10}");
                    index++;
                }
            }

            Console.WriteLine();
            foreach (var row in section.Rows)
            {
                Console.Write(row.Index > 0 ? $"{row.Index}\t" : "\t");
                Console.Write($"|{row.Key,-100}");
                if (section.RowHeader.Count() > 2 || row.Index < 0)
                    Console.Write($"|{row.Value,-10}");
                Console.WriteLine();
            }
        }
    }
}