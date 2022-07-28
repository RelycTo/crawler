namespace crawler.Extensions;

public static class UrlExtensions
{
    public static bool IsAcceptableSchema(this Uri uri) =>
        uri.Scheme is "http" or "https";

    public static bool IsLinkAcceptable(Uri uri, Uri baseUri)
    {
        return uri.IsAcceptableSchema() &&
               IsInternalLink(uri, baseUri);
    }

    public static string FixLink(this string link, Uri baseUri)
    {
        link = RemoveAnchor(link);
            
        /*
        if(baseUri.AbsoluteUri.EndsWith(".xml"))
            return baseUri.Scheme + "://" + baseUri.Host.TrimEnd('/') + link;
        */
        if(link.StartsWith('.'))
            return baseUri.AbsoluteUri + link.TrimEnd('.');
        if (!link.Contains("http") && !link.Contains("https"))
            return baseUri.Scheme + "://" + baseUri.Host + link.TrimStart('.');
        return link;
    }

    private static string RemoveAnchor(string link)
    {
        var anchorIndex = link.IndexOf('#');
        return anchorIndex == -1 ? link : link[..(anchorIndex - 1)];
    }

    private static bool IsInternalLink(Uri checkUri, Uri baseUri) =>
        checkUri.Host == baseUri.Host;
}

