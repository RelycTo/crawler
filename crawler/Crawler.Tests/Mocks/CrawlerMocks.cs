using System.Net;
using System.Net.Http.Headers;
using Moq;
using Moq.Protected;

namespace Crawler.Tests.Mocks;

internal static class CrawlerMocks
{
    internal static Mock<HttpMessageHandler> CreateHttpMessageHandlerMock(
        string responseBody,
        HttpStatusCode responseStatus,
        string responseContentType)
    {
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        var response = new HttpResponseMessage
        {
            StatusCode = responseStatus,
            Content = new StringContent(responseBody),
        };

        response.Content.Headers.ContentType = new MediaTypeHeaderValue(responseContentType);

        httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        return httpMessageHandlerMock;
    }
}
