using Crawler.App.DTOs;

namespace Crawler.UI.Services;

public class ConsoleReport
{
    private readonly IEnumerable<ReportSection> _sections;

    public ConsoleReport(IEnumerable<ReportSection> sections)
    {
        _sections = sections;
    }

    public void CreateReport()
    {
        foreach (var section in _sections)
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