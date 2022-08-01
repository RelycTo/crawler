namespace crawler.Models;

public class ReportRow
{
    public ReportRow(int index, string key, long value)
    {
        Index = index;
        Key = key;
        Value = value;
    }

    public int Index { get; }
    public string Key { get; }
    public long Value { get; }
}
