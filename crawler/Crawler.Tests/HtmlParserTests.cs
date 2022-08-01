using crawler.Infrastructure;

namespace Crawler.Tests;

public class HtmlParserTests
{
    [Fact]
    public void GetLinks_EmptyContent_EmptyCollection()
    {
        var parser = new HtmlLinkParser();

        var actual = parser.GetLinks(string.Empty);

        Assert.Empty(actual);
    }

    [Fact]
    public void GetLinks_HtmlHasNoAnchors_EmptyCollection()
    {
        var parser = new HtmlLinkParser();

        var actual = parser.GetLinks("<html><body>The given content has no anchors elements</body></html>");

        Assert.Empty(actual);
    }

    [Fact]
    public void GetLinks_HtmlHasAnchors_CollectionWhichContainsNumberOfAnchors()
    {
        var parser = new HtmlLinkParser();

        var actual = parser.GetLinks("<html><body>The given content contains one anchor element <a href='www.test.com'>click me</a></body></html>")
            .ToArray();

        Assert.Single(actual);
        Assert.Equal("www.test.com", actual[0]);
    }
}