namespace Crawler.Models
{
    public class CrawlItem
    {
        public CrawlItem(string url, long duration)
        {
            Url = url;
            Uri = new Uri(url);
            Duration = duration;
            Errors = Array.Empty<string>();
        }

        public CrawlItem(string url, params string[] errors)
        {
            Url = url;
            Uri = new Uri(url);
            Errors = errors;
        }

        public string Url { get; }
        public Uri Uri { get; }
        public long Duration { get; }
        public string[] Errors { get; }
    }
}
