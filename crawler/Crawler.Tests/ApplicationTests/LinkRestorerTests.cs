using Crawler.Application.Infrastructure;

namespace Crawler.Tests.ApplicationTests;

public class LinkRestorerTests
{
    [Fact]
    public void SameDomainUriShouldBeAcceptable()
    {
        var baseUri = new Uri("https://test.com");
        var checkUri = new Uri("https://test.com/query");
        var restorer = new LinkRestorer();

        var actual = restorer.IsLinkAcceptable(checkUri, baseUri);

        Assert.True(actual);
    }

    [Fact]
    public void DifferentDomainsUriShouldBeNotAcceptable()
    {
        var baseUri = new Uri("https://test.com");
        var checkUri = new Uri("https://anothertest.com/query");
        var restorer = new LinkRestorer();

        var actual = restorer.IsLinkAcceptable(checkUri, baseUri);

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
    public void RelativePathShouldBeRestoredToAbsolutePath(string baseUrl, string linkToFix, string expected)
    {
        var parentUri = new Uri(baseUrl);
        var restorer = new LinkRestorer();

        var actual = restorer.RestoreAbsolutePath(linkToFix, parentUri);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RelativePathWithAnchorShouldBeRestoredToAbsolutePathWithoutAnchorPart()
    {
        var parentUri = new Uri("https://test.com/api");
        const string linkToRestore = "/api/query#changelog";
        var restorer = new LinkRestorer();

        var actual = restorer.RestoreAbsolutePath(linkToRestore, parentUri);

        Assert.Equal("https://test.com/api/query", actual);
    }

    [Fact]
    public void SiteMapInternalLinkShouldBeRestoredToAbsolutePath()
    {
        var parentUri = new Uri("https://test.com/sitemap.xml");
        const string linkToRestore = "/api/query";
        var restorer = new LinkRestorer();

        var actual = restorer.RestoreAbsolutePath(linkToRestore, parentUri);

        Assert.Equal("https://test.com/api/query", actual);
    }

    [Fact]
    public void SiteMapLinkToInternalSiteMapLinkShouldBeRestoredToAbsolutePath()
    {
        var parentUri = new Uri("https://test.com/sitemap.xml");
        const string linkToRestore = "https://test.com/forms/sitemap.xml";
        var restorer = new LinkRestorer();

        var actual = restorer.RestoreAbsolutePath(linkToRestore, parentUri);

        Assert.Equal("https://test.com/forms/sitemap.xml", actual);
    }
}