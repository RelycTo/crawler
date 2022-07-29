using System.Net;
using System.Net.Mime;
using crawler.Services;
using Moq;
using Crawler.Tests.Mocks;
using Moq.Protected;

namespace Crawler.Tests;

public class PageLoaderTests
{
    [Fact]
    public async Task GetResponseAsync_AcceptedMediaType_ReturnCrawlResponse()
    {
        var handlerMock = CrawlerMocks.CreateHttpMessageHandlerMock(
            Stubs.HtmlResponseStub,
            HttpStatusCode.OK,
            "text/html"
        );
        var httpClient = new HttpClient(handlerMock.Object);
        var loader = new PageLoader(httpClient);

        var actual = await loader.GetResponseAsync(new Uri("https://test.com"), CancellationToken.None);

        handlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());

        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.True(actual.Duration >= 0);
        Assert.Equal(Stubs.HtmlResponseStub, actual.Content);
    }

    [Fact]
    public async Task GetResponseAsync_ExcludedMediaType_ReturnCrawlResponseWithEmptyContent()
    {
        var handlerMock = CrawlerMocks.CreateHttpMessageHandlerMock(
            Stubs.XMLResponseStub,
            HttpStatusCode.OK,
            "text/xml"
        );
        var httpClient = new HttpClient(handlerMock.Object);
        var loader = new PageLoader(httpClient)
            .SetExcludedMediaTypes(new[] { MediaTypeNames.Text.Xml });

        var actual = await loader.GetResponseAsync(new Uri("https://test.com/sitemap.xml"), CancellationToken.None);

        handlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());

        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.Equal(string.Empty, actual.Content);
    }
}