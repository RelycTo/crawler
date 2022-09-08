using Crawler.Application.Models;
using Crawler.Application.Services;
using Crawler.Application.Services.Repositories;
using Crawler.Tests.Fakes;
using Moq;

namespace Crawler.Tests.DataAccess;

public class CrawlDataServiceTests
{
    [Fact]
    public async Task GetCrawlInfosAsyncShouldReturnAllCrawlInfoItemsAsCrawlInfoDtoCollection()
    {
        var mockCrawlInfoRepository = new Mock<ICrawlInfoRepository>();
        var mockCrawlDetailsRepository = new Mock<ICrawlDetailsRepository>();

        mockCrawlInfoRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Stubs.CrawlInfosStub);

        var service = new CrawlDataService(mockCrawlInfoRepository.Object, mockCrawlDetailsRepository.Object);
        var actual = (await service.GetCrawlInfosAsync(CancellationToken.None)).ToArray();

        Assert.NotNull(actual);
        Assert.Equal(3, actual.Length);
        Assert.All(actual, item => Assert.IsAssignableFrom<CrawlInfoDto>(item));
        mockCrawlInfoRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCrawlDetailsAsyncShouldReturnAllCrawlDetailsByConcreteCrawlAsCrawlDetailDtoCollection()
    {
        var mockCrawlInfoRepository = new Mock<ICrawlInfoRepository>();
        var mockCrawlDetailsRepository = new Mock<ICrawlDetailsRepository>();

        mockCrawlDetailsRepository.Setup(r => r.GetCrawlDetailsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Stubs.CrawlDetailsStub);

        var service = new CrawlDataService(mockCrawlInfoRepository.Object, mockCrawlDetailsRepository.Object);
        var actual = (await service.GetCrawlsDetailsAsync(1, CancellationToken.None)).ToArray();

        Assert.NotNull(actual);
        Assert.Equal(3, actual.Length);
        Assert.All(actual, item => Assert.IsAssignableFrom<CrawlDetailDto>(item));
        mockCrawlDetailsRepository.Verify(r => r.GetCrawlDetailsAsync(1, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}