namespace crawler.Models
{
    public class ResultItem
    {
        public ResultItem(string url, long duration, bool foundBySiteMap)
        {
            Url = url;
            Uri = new Uri(url);
            Duration = duration;
            FoundByCrawler = !foundBySiteMap;
            FoundBySiteMap = foundBySiteMap;
            Errors = Array.Empty<string>();
        }

        public ResultItem(string url, bool foundBySiteMap, params string[] errors)
        {
            Url = url;
            Uri = new Uri(url);
            Errors = errors;
            FoundByCrawler = !foundBySiteMap;
            FoundBySiteMap = foundBySiteMap;
        }

        public string Url { get; }
        public Uri Uri { get; }
        public long Duration { get; }
        public string[] Errors { get; }

        public bool FoundByCrawler { get; }
        public bool FoundBySiteMap { get; private set; }

        public void UpdateFoundBySiteMapFlag(bool value) => FoundBySiteMap = value;
    }
}
