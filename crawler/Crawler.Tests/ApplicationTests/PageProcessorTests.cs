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
        var options = new CrawlOptionsDto("http://test.com", "http://test.com", 1, ProcessStep.Site, new[] { MediaTypeNames.Text.Xml }); 

        var actual = (await processor.CrawlAsync(options, CancellationToken.None)).ToArray();

        Assert.NotNull(actual);
        handlerMock.Protected().Verify("SendAsync", Times.Exactly(2), ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
        Assert.Equal(2, actual.Length);
        Assert.Collection(actual, item => Assert.True(item.Duration >= 0),
            item => Assert.True(item.Duration >= 0));
        Assert.All(actual, item => Assert.Equal(SourceType.Site, item.SourceType));
    }
}