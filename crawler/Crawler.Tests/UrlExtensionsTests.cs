using Crawler.Extensions;

namespace Crawler.Tests;

public class UrlExtensionsTests
{
    [Fact]
    public void IsLinkAcceptable_TheSameDomainUri_ShouldBeAcceptable()
    {
        var baseUri = new Uri("https://test.com");
        var checkUri = new Uri("https://test.com/query");

        var actual = checkUri.IsLinkAcceptable(baseUri);

        Assert.True(actual);
    }

    [Fact]
    public void IsLinkAcceptable_AnotherDomainUri_ShouldBeNotAcceptable()
    {
        var baseUri = new Uri("https://test.com");
        var checkUri = new Uri("https://anothertest.com/query");

        var actual = checkUri.IsLinkAcceptable(baseUri);

        Assert.False(actual);
    }

    [Theory]
    [InlineData("https://test.com/api/", "/", "https://test.com/")]
    [InlineData("https://test.com/api/", "query", "https://test.com/api/query")]
    [InlineData("https://test.com/api/", "/api/query", "https://test.com/api/query")]
    [InlineData("https://test.com/api/", "./query", "https://test.com/api/query")]
    [InlineData("https://test.com/api/", "../api/query", "https://test.com/api/query")]
    [InlineData("https://test.com/api/", "./../api/query", "https://test.com/api/query")]
    [InlineData("https://test.com/api/query/", "../../api/query", "https://test.com/api/query")]
    public void RestoreAbsolutePath_RelativePath_ShouldBeRestoredToAbsolutePath(string baseUrl, string linkToFix,
        string expected)
    {
        var parentUri = new Uri(baseUrl);

        var actual = linkToFix.RestoreAbsolutePath(parentUri);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RestoreAbsolutePath_RelativePathWithAnchor_ShouldBeRestoredToAbsolutePathWithoutAnchorPart()
    {
        var parentUri = new Uri("https://test.com/api");
        const string linkToFix = "/api/query#changelog";

        var actual = linkToFix.RestoreAbsolutePath(parentUri);

        Assert.Equal("https://test.com/api/query", actual);
    }

    [Fact]
    public void RestoreAbsolutePath_SiteMapLinkToResource_ShouldBeRestoredToAbsolutePath()
    {
        var parentUri = new Uri("https://test.com/sitemap.xml");
        const string linkToFix = "/api/query";

        var actual = linkToFix.RestoreAbsolutePath(parentUri);

        Assert.Equal("https://test.com/api/query", actual);
    }

    [Fact]
    public void RestoreAbsolutePath_SiteMapLinkToAnotherSiteMap_ShouldBeAnotherSiteMapPath()
    {
        var parentUri = new Uri("https://test.com/sitemap.xml");
        const string linkToFix = "https://test.com/forms/sitemap.xml";

        var actual = linkToFix.RestoreAbsolutePath(parentUri);

        Assert.Equal("https://test.com/forms/sitemap.xml", actual);
    }
}