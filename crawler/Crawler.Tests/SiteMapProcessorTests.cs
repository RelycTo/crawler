using System.Net;
using System.Net.Mime;
using Crawler.Infrastructure;
using Crawler.Services;
using Crawler.Tests.CrawlerMocks;
using Moq;
using Moq.Protected;
using Shared.Models;

namespace Crawler.Tests;

public class SiteMapProcessorTests
{
    [Fact]
    public async Task SiteMapCrawledSiteMapsWithInternalLinksShouldReturnCrawlItemCollection()
    {
        var handlerMock = CrawlerMocks.Mocks.CreateHttpMessageHandlerMock(
            Stubs.XMLResponseStub,
            HttpStatusCode.OK,
            MediaTypeNames.Text.Xml
        );
        var httpClient = new HttpClient(handlerMock.Object);
        var loader = new PageLoader(httpClient);
        var parser = new XmlLinkParser();
        var linkRestorer = new LinkRestorer();
        var processor = new SiteMapProcessor(loader, parser, linkRestorer);
        var context = Stubs.GetCrawlContext(ProcessStep.SiteMap, new[] { MediaTypeNames.Text.Html });
        context.SetStep(ProcessStep.SiteMap);
        var actual = (await processor.ProcessAsync(context, CancellationToken.None)).ToArray();

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