using System.Net;
using System.Net.Mime;
using Crawler.Infrastructure;
using Crawler.Tests.CrawlerMocks;
using Moq;
using Moq.Protected;

namespace Crawler.Tests;

public class PageLoaderTests
{
    [Fact]
    public async Task GetResponseAsync_AcceptedMediaType_ReturnCrawlResponse()
    {
        var handlerMock = CrawlerMocks.Mocks.CreateHttpMessageHandlerMock(
            Stubs.HtmlResponseStubWithAnchors,
            HttpStatusCode.OK,
            MediaTypeNames.Text.Html
        );
        var httpClient = new HttpClient(handlerMock.Object);
        var loader = new PageLoader(httpClient);

        var actual = await loader
            .GetResponseAsync(new Uri("https://test.com"), new[] { MediaTypeNames.Text.Xml },
                CancellationToken.None);

        handlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
        Assert.NotNull(actual);
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.True(actual.Duration >= 0);
        Assert.Equal(Stubs.HtmlResponseStubWithAnchors, actual.Content);
    }

    [Fact]
    public async Task GetResponseAsync_ExcludedMediaType_ReturnCrawlResponseWithEmptyContent()
    {
        var handlerMock = CrawlerMocks.Mocks.CreateHttpMessageHandlerMock(
            Stubs.XMLResponseStub,
            HttpStatusCode.OK,
            MediaTypeNames.Text.Xml
        );
        var httpClient = new HttpClient(handlerMock.Object);
        var loader = new PageLoader(httpClient);

        var actual = await loader
            .GetResponseAsync(new Uri("https://test.com/sitemap.xml"),
                new[] { MediaTypeNames.Text.Xml },
                CancellationToken.None);

        handlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());

        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.Equal(string.Empty, actual.Content);
    }
}