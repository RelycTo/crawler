using Crawler.Presentation.Services.ReportServices;
using Crawler.Tests.Fakes;

namespace Crawler.Tests.UI;

public class ReportFormatterTests
{
    [Fact]
    public void Prepare_ResultItemsCollection_ShouldReturnReportSectionsCollection()
    {
        var formatter = new ReportFormatter();
        var actual = formatter.Prepare(Stubs.ReportItems).ToArray();

        Assert.NotNull(actual);
        Assert.Equal(4, actual.Length);
        Assert.Collection(actual, item =>
            {
                Assert.StartsWith("Urls FOUNDED IN SITEMAP.XML", item.Title);
                Assert.True(item.Rows.Count() == 1);
            },
            item =>
            {
                Assert.StartsWith("Urls FOUNDED BY CRAWLING", item.Title);
                Assert.True(item.Rows.Count() == 1);
            },
            item =>
            {
                Assert.Equal("Timing", item.Title);
                Assert.True(item.Rows.Count() == 4);
            },
            item =>
            {
                Assert.Equal("Summary", item.Title);
                Assert.True(item.Rows.Count() == 2);
                Assert.Equal(3, item.Rows.First().Value);
                Assert.Equal(3, item.Rows.Last().Value);
            });
    }
}