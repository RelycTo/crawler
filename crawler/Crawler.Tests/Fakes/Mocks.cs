using System.Net;
using System.Net.Http.Headers;
using Moq;
using Moq.Protected;

namespace Crawler.Tests.Fakes;

internal static class Mocks
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

    internal static Mock<HttpMessageHandler> CreateHttpSequencedMessageHandlerMock(
        IEnumerable<string> responseBodies,
        HttpStatusCode responseStatus,
        string responseContentType)
    {
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        var responses = responseBodies
            .Select(b => new HttpResponseMessage
            {
                StatusCode = responseStatus,
                Content = new StringContent(b),
            }).ToArray();

        foreach (var response in responses)
        {
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(responseContentType);
        }

        httpMessageHandlerMock
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responses[0])
            .ReturnsAsync(responses[1])
            .ReturnsAsync(responses[1]);

        return httpMessageHandlerMock;
    }
}
