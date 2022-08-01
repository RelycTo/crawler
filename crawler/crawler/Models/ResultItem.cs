namespace Crawler.Models
{
    public class ResultItem
    {
        public ResultItem(string url, long duration, bool isFoundByCrawler, bool isFoundBySiteMap)
        {
            Url = url;
            Duration = duration;
            IsFoundByCrawler = isFoundByCrawler;
            IsFoundBySiteMap = isFoundBySiteMap;
        }

        public string Url { get; }
        public long Duration { get; }
        public bool IsFoundByCrawler { get; }
        public bool IsFoundBySiteMap { get; set; }
        public bool IsProcessed => Duration > 0;
    }
}