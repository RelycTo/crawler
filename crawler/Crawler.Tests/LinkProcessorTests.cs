using System.Net;
using System.Net.Mime;
using Crawler.Infrastructure;
using Crawler.Services;
using Crawler.Tests.CrawlerMocks;
using Moq;
using Moq.Protected;
using Shared.Models;

namespace Crawler.Tests;

public class LinkProcessorTests
{
    [Fact]
    public async Task LinkProcessorCrawledHtmlWithInternalLinksShouldReturnCrawlItemCollection()
    {
        var handlerMock = CrawlerMocks.Mocks.CreateHttpSequencedMessageHandlerMock(
            new[] { Stubs.HtmlResponseStubWithAnchors, Stubs.HtmlResponseStubWithoutAnchors },
            HttpStatusCode.OK,
            MediaTypeNames.Text.Html
        );
        var httpClient = new HttpClient(handlerMock.Object);
        var loader = new PageLoader(httpClient);
        var parser = new HtmlLinkParser();
        var linkRestorer = new LinkRestorer();
        var processor = new LinkProcessor(loader, parser, linkRestorer);
        var context = Stubs.GetCrawlContext(ProcessStep.Site, new[] { MediaTypeNames.Text.Xml });

        var actual = (await processor.ProcessAsync(context, CancellationToken.None)).ToArray();

        Assert.NotNull(actual);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(2), ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal(2, actual.Length);
        Assert.Collection(actual, item => Assert.True(item.Duration >= 0),
            item => Assert.True(item.Duration >= 0));
    }
}