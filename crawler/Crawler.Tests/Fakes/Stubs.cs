using System.Net;
using Crawler.Application.Models;
using Crawler.Domain.Models;
using Crawler.Domain.Models.Enums;

namespace Crawler.Tests.Fakes;

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

    public static IReadOnlyCollection<CrawlItemDto> CrawledSiteStubCollection = new[]
    {
        new CrawlItemDto(SourceType.Site, "https://test.com", 20, HttpStatusCode.OK),
        new CrawlItemDto(SourceType.Site, "https://test.com/api/", 25, HttpStatusCode.OK),
        new CrawlItemDto(SourceType.Site, "https://test.com/api/directory", 40, HttpStatusCode.OK)
    };

    public static IReadOnlyCollection<CrawlItemDto> CrawledSiteMapStubCollection = new[]
    {
        new CrawlItemDto(SourceType.SiteMap, "https://test.com", -1, HttpStatusCode.OK),
        new CrawlItemDto(SourceType.SiteMap, "https://test.com/api/", -1, HttpStatusCode.OK),
        new CrawlItemDto(SourceType.SiteMap, "https://test.com/api/explain", -1, HttpStatusCode.OK)
    };

    public static IReadOnlyCollection<CrawlDetailDto> ReportItems = new[]
    {
        new CrawlDetailDto
        {
            SourceType = SourceType.Both,
            Address = "https://test.com",
            Duration = 20,
            StatusCode = HttpStatusCode.OK
        },
        new CrawlDetailDto
        {
            SourceType = SourceType.Both,
            Address = "https://test.com/api",
            Duration = 50,
            StatusCode = HttpStatusCode.OK
        },
        new CrawlDetailDto
        {
            SourceType = SourceType.Site,
            Address = "https://test.com/api/directory",
            Duration = 40,
            StatusCode = HttpStatusCode.OK
        },
        new CrawlDetailDto
        {
            SourceType = SourceType.SiteMap,
            Address = "https://test.com/api/explain",
            Duration = 30,
            StatusCode = HttpStatusCode.OK
        }
    };

    public static IReadOnlyCollection<CrawlInfo> CrawlInfosStub = new[]
    {
        new CrawlInfo
        {
            Id = 1,
            Status = CrawlStatus.Success,
            TargetUrl = "https://test.com"
        },
        new CrawlInfo
        {
            Id = 2,
            Status = CrawlStatus.Failure,
            TargetUrl = "https://best.com"
        },
        new CrawlInfo
        {
            Id = 3,
            Status = CrawlStatus.Success,
            TargetUrl = "https://fest.com"
        }
    };

    public static IReadOnlyCollection<CrawlDetail> CrawlDetailsStub = new[]
    {
        new CrawlDetail
        {
            Id = 1,
            CrawlInfoId = 1,
            Address = "https://test.com",
            Duration = 20,
            SourceType = SourceType.Both,
            CreatedUtc = DateTime.UtcNow,
            StatusCode = HttpStatusCode.OK
        },
        new CrawlDetail
        {
            Id = 2,
            CrawlInfoId = 1,
            Address = "https://test.com/api",
            Duration = 30,
            SourceType = SourceType.Site,
            CreatedUtc = DateTime.UtcNow,
            StatusCode = HttpStatusCode.OK
        },
        new CrawlDetail
        {
            Id = 3,
            CrawlInfoId = 1,
            Address = "https://test.com/documents",
            Duration = 50,
            SourceType = SourceType.SiteMap,
            CreatedUtc = DateTime.UtcNow,
            StatusCode = HttpStatusCode.OK
        },
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