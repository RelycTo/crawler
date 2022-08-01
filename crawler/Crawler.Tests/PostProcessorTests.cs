using System.Net;
using System.Net.Mime;
using Crawler.Infrastructure;
using Crawler.Services;
using Crawler.Tests.CrawlerMocks;
using Moq;
using Moq.Protected;

namespace Crawler.Tests;

public class PostProcessorTests
{
    [Fact]
    public async Task ProcessAsync_ProcessedAndNotProcessedCollections_ShouldReturnMergedDataWithActualTimings()
    {
        var handlerMock = CrawlerMocks.Mocks.CreateHttpMessageHandlerMock(
            Stubs.XMLResponseStub,
            HttpStatusCode.OK,
            MediaTypeNames.Text.Html
        );
        var httpClient = new HttpClient(handlerMock.Object);
        var loader = new PageLoader(httpClient);
        var processor = new PostProcessor(loader,
            Stubs.CrawledSiteStubCollection,
            Stubs.CrawledSiteMapStubCollection,
            new[] { MediaTypeNames.Text.Xml });

        var actual = (await processor.ProcessAsync(CancellationToken.None)).ToArray();

        Assert.NotNull(actual);
        handlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal(4, actual.Length);
        Assert.Collection(actual, item => Assert.True(item.IsProcessed),
            item => Assert.True(item.IsProcessed),
            item => Assert.True(item.IsProcessed),
            item => Assert.True(item.IsProcessed));
    }
}