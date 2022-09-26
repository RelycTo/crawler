using System.Net;
using System.Net.Mime;
using Crawler.Application.Crawl;
using Crawler.Application.Crawl.Processors;
using Crawler.Domain.Models.Enums;
using Crawler.Infrastructure.Http;
using Crawler.Infrastructure.Parsers;
using Crawler.Infrastructure.Utilities;
using Crawler.Tests.Fakes;
using Moq;
using Moq.Protected;
using Mocks = Crawler.Tests.Fakes.Mocks;

namespace Crawler.Tests.Crawler;

public class SiteMapProcessorTests
{
    [Fact]
    public async Task SiteMapCrawledSiteMapsWithInternalLinksShouldReturnSiteMapCrawlItemCollection()
    {
        var handlerMock = Mocks.CreateHttpMessageHandlerMock(
            Stubs.XMLResponseStub,
            HttpStatusCode.OK,
            MediaTypeNames.Text.Xml
        );
        var httpClient = new HttpClient(handlerMock.Object);
        var loader = new PageLoader(httpClient);
        var parser = new XmlLinkParser();
        var linkRestorer = new LinkRestorer();
        var processor = new SiteMapProcessor(loader, parser, linkRestorer);
        var options = new CrawlOptionsDto("http://test.com/sitemap.xml", "http://test.com", 1, ProcessStep.Site, new[] { MediaTypeNames.Text.Html });
        var actual = (await processor.CrawlAsync(options, CancellationToken.None)).ToArray();

        Assert.NotNull(actual);
        handlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal(4, actual.Length);
        Assert.Collection(actual, item => Assert.True(item.Duration >= -1),
            item => Assert.True(item.Duration >= -1),
            item => Assert.True(item.Duration >= -1),
            item => Assert.True(item.Duration >= -1));
        Assert.All(actual, item => Assert.Equal(SourceType.SiteMap, item.SourceType));
    }
}