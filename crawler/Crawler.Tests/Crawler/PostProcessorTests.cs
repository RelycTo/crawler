using System.Net;
using System.Net.Mime;
using Crawler.App.Infrastructure.Loaders;
using Crawler.App.Services.Processors;
using Crawler.Entities.Models.Enums;
using Crawler.Tests.Fakes;
using Moq;
using Moq.Protected;
using Mocks = Crawler.Tests.Fakes.Mocks;

namespace Crawler.Tests.Crawler;

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
        var context = Stubs.GetCrawlContext(ProcessStep.PostProcess, new[] { MediaTypeNames.Text.Xml });
        context.SetProcessedItems(ProcessStep.Site, Stubs.CrawledSiteStubCollection)
            .SetProcessedItems(ProcessStep.SiteMap, Stubs.CrawledSiteMapStubCollection);
        var processor = new PostProcessor(loader);

        var actual = (await processor.ProcessAsync(context, CancellationToken.None)).ToArray();

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