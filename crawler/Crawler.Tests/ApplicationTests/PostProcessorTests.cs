using System.Net;
using System.Net.Mime;
using Crawler.Application.Crawl.Processors;
using Crawler.Domain.Models.Enums;
using Crawler.Infrastructure.Http;
using Crawler.Tests.Fakes;
using Moq;
using Moq.Protected;
using Mocks = Crawler.Tests.Fakes.Mocks;

namespace Crawler.Tests.ApplicationTests;

public class PostProcessorTests
{
    [Fact]
    public async Task CrawledLinksFromSiteAndSiteMapShouldReturnMergedCrawlItemCollection()
    {
        var handlerMock = Mocks.CreateHttpMessageHandlerMock(
            Stubs.XMLResponseStub,
            HttpStatusCode.OK,
            MediaTypeNames.Text.Html
        );
        var httpClient = new HttpClient(handlerMock.Object);
        var loader = new PageLoader(httpClient);
        var processor = new PostProcessor(loader);

        var actual = (await processor.ProcessAsync(Stubs.CrawledSiteStubCollection, Stubs.CrawledSiteMapStubCollection,
            new[] { MediaTypeNames.Text.Xml },
            CancellationToken.None)).ToArray();

        Assert.NotNull(actual);
        handlerMock.Protected().Verify("SendAsync", Times.Once(), ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal(4, actual.Length);
        Assert.Collection(actual, item => Assert.Equal(SourceType.Both, item.SourceType),
            item => Assert.Equal(SourceType.Both, item.SourceType),
            item => Assert.Equal(SourceType.Site, item.SourceType),
            item => Assert.Equal(SourceType.SiteMap, item.SourceType));
    }
}