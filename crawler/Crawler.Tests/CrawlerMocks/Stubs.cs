using System.Net;
using Shared.Models;

namespace Crawler.Tests.CrawlerMocks;

internal static class Stubs
{
    public const string HtmlResponseStubWithAnchors =
        @"<html><body>Some html text with two anchors <a href='some_path'>Click here<a> <a href='#bookmark2'>Bookmark<a><body></html>";

    public const string HtmlResponseStubWithoutAnchors =
        @"<html><body>HTML without any anchors<body></html>";


    public const string XMLResponseStub =
        @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes"" ?>
    <urlset xmlns = ""http://www.sitemaps.org/schemas/sitemap/0.9""
    xmlns:xhtml=""http://www.w3.org/1999/xhtml"" >
    <url>
    <loc>/api/</loc>
    <lastmod>2018-11-28T15:14:39+10:00</lastmod>
    </url>
    <url>
    <loc>/docs/</loc>
    <lastmod>2018-11-28T15:14:39+10:00</lastmod>
    </url>
    <url>
    <loc>/docs/getting-started/</loc>
    </url>
    <url>
    <loc>/api/query/</loc>
    </url>
    </urlset>";

    public static IReadOnlyCollection<CrawlItem> CrawledSiteStubCollection = new[]
    {
        new CrawlItem(SourceType.Site, "https://test.com", 20, HttpStatusCode.OK),
        new CrawlItem(SourceType.Site, "https://test.com/api/", 25, HttpStatusCode.OK),
        new CrawlItem(SourceType.Site, "https://test.com/api/directory", 40, HttpStatusCode.OK)
    };

    public static IReadOnlyCollection<CrawlItem> CrawledSiteMapStubCollection = new[]
    {
        new CrawlItem(SourceType.SiteMap, "https://test.com", -1, HttpStatusCode.OK),
        new CrawlItem(SourceType.SiteMap, "https://test.com/api/", -1, HttpStatusCode.OK),
        new CrawlItem(SourceType.SiteMap, "https://test.com/api/explain", -1, HttpStatusCode.OK)
    };

    public static IReadOnlyCollection<CrawlItem> ReportItems = new[]
    {
        new CrawlItem(SourceType.Both, "https://test.com", 20, HttpStatusCode.OK),
        new CrawlItem(SourceType.Both, "https://test.com/api", 50, HttpStatusCode.OK),
        new CrawlItem(SourceType.Site, "https://test.com/api/directory", 40, HttpStatusCode.OK),
        new CrawlItem(SourceType.SiteMap, "https://test.com/api/explain", 30, HttpStatusCode.OK)
    };

    public static CrawlHandlerContext GetCrawlContext(ProcessStep step, IEnumerable<string> excludedMediaTypes)
    {
        var uri = step == ProcessStep.SiteMap ? new Uri("https://text.com/sitemap.xml") : new Uri("https://text.com/");
        var context = CrawlHandlerContext.Create(uri, 1);
        context
            .SetStep(step)
            .Options
            .SetExcludedMediaTypes(excludedMediaTypes);

        return context;
    }

}
