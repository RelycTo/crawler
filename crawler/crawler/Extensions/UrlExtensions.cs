namespace crawler.Extensions;

public static class UrlExtensions
{
    private static readonly HashSet<string> AcceptablePrefixes = new()
    {
        "http:",
        "https:"
    };

    public static string FixUrl(this string url) => url.Replace("www.", "");

    public static bool IsAcceptablePrefix(this string link, IEnumerable<string> prefixes) =>
        prefixes.Any(link.StartsWith);

    public static bool IsLinkAcceptable(string link, string baseUrl) =>
        IsLinkAcceptable(link, baseUrl, AcceptablePrefixes);
    public static bool IsLinkAcceptable(string link, string baseUrl, IEnumerable<string> prefixes)
    {
        return !string.IsNullOrWhiteSpace(link) &&
               link.IsAcceptablePrefix(prefixes) &&
               IsInternalLink(new Uri(link.FixUrl()), new Uri(baseUrl.FixUrl()));
    }

    private static bool IsInternalLink(Uri checkUri, Uri baseUri) =>
        checkUri.Host.TrimEnd('/') == baseUri.Host.TrimEnd('/');
}

