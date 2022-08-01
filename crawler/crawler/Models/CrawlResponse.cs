using System.Net;

namespace Crawler.Models;

public class CrawlResponse
{
    public CrawlResponse(Uri address, string content, HttpStatusCode statusCode, long duration)
    {
        Address = address;
        Content = content;
        StatusCode = statusCode;
        Duration = duration;
    }

    public Uri Address { get; }
    public string Content { get; }
    public HttpStatusCode StatusCode { get; }
    public long Duration { get; }
}
