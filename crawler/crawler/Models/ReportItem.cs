namespace crawler.Models
{
    public class ReportItem
    {
        public string Title { get; set; }
        public string RowHeader { get; set; }
        public Dictionary<string, string> Rows { get; set; }
    }
}
