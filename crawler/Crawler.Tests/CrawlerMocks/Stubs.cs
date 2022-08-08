using Crawler.Models;

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

    public static IReadOnlyCollection<CrawlItem> CrawledSiteStubCollection = Array.Empty<CrawlItem>(); /*new[]
    {
        new CrawlItem("https://test.com", 20),
        new CrawlItem("https://test.com/api/", 25),
        new CrawlItem("https://test.com/api/directory", 40)
    };*/

    public static IReadOnlyCollection<CrawlItem> CrawledSiteMapStubCollection = Array.Empty<CrawlItem>(); /*new[]
    {
        new CrawlItem("https://test.com", -1),
        new CrawlItem("https://test.com/api/", -1),
        new CrawlItem("https://test.com/api/explain", -1)
    };*/

    public static IReadOnlyCollection<ResultItem> ResultItems = new[]
    {
        new ResultItem("https://test.com", 20, true, true),
        new ResultItem("https://test.com/api", 50, true, true),
        new ResultItem("https://test.com/api/directory", 40, true, false),
        new ResultItem("https://test.com/api/explain", 30, false, true),
    };
}
