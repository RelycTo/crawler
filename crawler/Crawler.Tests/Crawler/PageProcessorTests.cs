using System.Net;
using System.Net.Mime;
using Crawler.App.Infrastructure;
using Crawler.App.Infrastructure.Loaders;
using Crawler.App.Infrastructure.Parsers;
using Crawler.App.Services.Processors;
using Crawler.Entities.Models.Enums;
using Crawler.Tests.Fakes;
using Moq;
using Moq.Protected;
using Mocks = Crawler.Tests.Fakes.Mocks;

namespace Crawler.Tests.Crawler;

public class PageProcessorTests
{
    [Fact]
    public async Task LinkProcessorCrawledHtmlWithInternalLinksShouldReturnCrawlItemSiteCollection()
    {
        var handlerMock = Mocks.CreateHttpSequencedMessageHandlerMock(
            new[] { Stubs.HtmlResponseStubWithAnchors, Stubs.HtmlResponseStubWithoutAnchors },
            HttpStatusCode.OK,
            MediaTypeNames.Text.Html
        );
        var httpClient = new HttpClient(handlerMock.Object);
        var loader = new PageLoader(httpClient);
        var parser = new HtmlLinkParser();
        var linkRestorer = new LinkRestorer();
        var processor = new PageProcessor(loader, parser, linkRestorer);
        var context = Stubs.GetCrawlContext(ProcessStep.Site, new[] { MediaTypeNames.Text.Xml });

        var actual = (await processor.ProcessAsync(context, CancellationToken.None)).ToArray();

        Assert.NotNull(actual);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(2), ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal(2, actual.Length);
        Assert.Collection(actual, item => Assert.True(item.Duration >= 0),
            item => Assert.True(item.Duration >= 0));
        Assert.All(actual, item => Assert.Equal(SourceType.Site, item.SourceType));
    }
}