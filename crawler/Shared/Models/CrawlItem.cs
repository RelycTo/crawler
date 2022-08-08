using System.Net;

namespace Shared.Models
{
    public class CrawlItem
    {
        public CrawlItem(SourceType sourceType, string url, long duration, HttpStatusCode statusCode)
        {
            Url = url;
            SourceType = sourceType;
            Uri = new Uri(url);
            Duration = duration;
            StatusCode = statusCode;
        }

        public SourceType SourceType { get; }
        public string Url { get; }
        public Uri Uri { get; }
        public long Duration { get; }
        public HttpStatusCode StatusCode { get; }
    }
}
