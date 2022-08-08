using System.Net;
using System.Net.Mime;
using Crawler.Infrastructure;
using Crawler.Models;
using Crawler.Services;
using Crawler.Tests.CrawlerMocks;
using Moq;
using Moq.Protected;

namespace Crawler.Tests;

public class LinkProcessorTests
{
    [Fact]
    public async Task ProcessAsync_SiteCrawl_ShouldReturnCrawlItemCollection()
    {
        var handlerMock = CrawlerMocks.Mocks.CreateHttpSequencedMessageHandlerMock(
            new[] { Stubs.HtmlResponseStubWithAnchors, Stubs.HtmlResponseStubWithoutAnchors },
            HttpStatusCode.OK,
            MediaTypeNames.Text.Html
        );
        var httpClient = new HttpClient(handlerMock.Object);
        var loader = new PageLoader(httpClient);
        var parser = new HtmlLinkParser();
        var excludedMediaTypes = new[] { MediaTypeNames.Text.Xml };
        var processor = new LinkProcessor(loader, parser);
        var uri = new Uri("https://test.com");

        var actual = Array.Empty<CrawlItem>(); //(await processor.ProcessAsync(uri, excludedMediaTypes, 1, CancellationToken.None)).ToArray();

        Assert.NotNull(actual);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(2), ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal(2, actual.Length);
        Assert.Collection(actual, item => Assert.True(item.Duration >= 0),
            item => Assert.True(item.Duration >= 0));
    }
}