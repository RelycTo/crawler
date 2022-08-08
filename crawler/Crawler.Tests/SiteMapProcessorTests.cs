using System.Net;
using System.Net.Mime;
using Crawler.Infrastructure;
using Crawler.Models;
using Crawler.Services;
using Crawler.Tests.CrawlerMocks;
using Moq;
using Moq.Protected;

namespace Crawler.Tests;

public class SiteMapProcessorTests
{
    [Fact]
    public async Task ProcessAsync_SiteMapCrawl_ShouldReturnCrawlItemCollection()
    {
        var handlerMock = CrawlerMocks.Mocks.CreateHttpMessageHandlerMock(
            Stubs.XMLResponseStub,
            HttpStatusCode.OK,
            MediaTypeNames.Text.Xml
        );
        var httpClient = new HttpClient(handlerMock.Object);
        var loader = new PageLoader(httpClient);
        var parser = new XmlLinkParser();
        var excludedMediaTypes = new[] { MediaTypeNames.Text.Html };
        var processor = new SiteMapProcessor(loader, parser);
        var uri = new Uri("https://test.com/sitemap.xml");

        var actual = Array.Empty<CrawlItem>(); //(await processor.ProcessAsync(uri, excludedMediaTypes, 1, CancellationToken.None)).ToArray();

        Assert.NotNull(actual);
        handlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal(4, actual.Length);
        Assert.Collection(actual, item => Assert.True(item.Duration >= -1),
            item => Assert.True(item.Duration >= -1),
            item => Assert.True(item.Duration >= -1),
            item => Assert.True(item.Duration >= -1));
    }
}