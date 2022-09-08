﻿namespace Crawler.Application.Models;

public class CrawlOptionsDto
{
    public CrawlOptionsDto(Uri uri, Uri siteMapUri, int taskCount)
    {
        Uri = uri;
        TaskCount = taskCount;
        SiteMapUri = siteMapUri;
        ExcludedMediaTypes = Array.Empty<string>();
    }

    public IReadOnlyCollection<string> ExcludedMediaTypes { get; private set; }
    public Uri Uri { get; }
    public Uri SiteMapUri { get; }
    public int TaskCount { get; }

    public CrawlOptionsDto SetExcludedMediaTypes(IEnumerable<string> mediaTypes)
    {
        ExcludedMediaTypes = mediaTypes.ToArray();
        return this;
    }
}