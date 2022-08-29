using Crawler.App.Infrastructure.Parsers;

namespace Crawler.Tests.Crawler;

public class XmlParserTests
{
    [Fact]
    public void GetLinks_EmptyContent_EmptyCollection()
    {
        var parser = new XmlLinkParser();

        var actual = parser.GetLinks(string.Empty);

        Assert.Empty(actual);
    }

    [Fact]
    public void GetLinks_XMLHasNoLinks_EmptyCollection()
    {
        var parser = new XmlLinkParser();

        var actual =
            parser.GetLinks(
                @"<sitemapindex xmlns=""http://www.google.com/schemas/sitemap/0.84""><sitemap></sitemap></sitemapindex>");

        Assert.Empty(actual);
    }

    [Fact]
    public void GetLinks_XMLHasLinks_CollectionWhichContainsNumberOfLinks()
    {
        var parser = new XmlLinkParser();

        var actual = parser
            .GetLinks(
                @"<sitemapindex xmlns=""http://www.google.com/schemas/sitemap/0.84""><sitemap><loc>www.test.com</loc></sitemap></sitemapindex>")
            .ToArray();

        Assert.Single(actual);
        Assert.Equal("www.test.com", actual[0]);
    }
}