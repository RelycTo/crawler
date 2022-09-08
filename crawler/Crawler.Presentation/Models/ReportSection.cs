namespace Crawler.Presentation.Models;

public class ReportSection
{
    public ReportSection(string title, IEnumerable<string> rowHeader, IEnumerable<ReportRow> rows)
    {
        Title = title;
        RowHeader = rowHeader;
        Rows = rows;
    }

    public string Title { get; }
    public IEnumerable<string> RowHeader { get; }
    public IEnumerable<ReportRow> Rows { get; }
}