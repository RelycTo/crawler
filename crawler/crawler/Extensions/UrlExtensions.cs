namespace Crawler.Extensions;

public static class UrlExtensions
{
    private const string SiteMap = "sitemap.xml";

    public static bool IsLinkAcceptable(this Uri uri, Uri baseUri)
    {
        return uri.IsAcceptableSchema() &&
               IsInternalLink(uri, baseUri);
    }

    public static string RestoreAbsolutePath(this string link, Uri parentUri)
    {
        if (parentUri.AbsoluteUri.EndsWith(SiteMap, StringComparison.InvariantCultureIgnoreCase))
            return RestoreSiteMapAbsolutePath(link, parentUri);
        link = RestoreRelative(link, parentUri);
        return RemoveAnchor(link);
    }

    private static string RestoreSiteMapAbsolutePath(string link, Uri parentUri)
    {
        if (link.EndsWith(SiteMap))
            return link;
        link = RestoreRelative(link, new Uri(parentUri.AbsoluteUri.TrimEnd(SiteMap.ToCharArray())));

        return RemoveAnchor(link);
    }

    private static string RestoreRelative(string link, Uri parentUri)
    {
        link = link.Trim();
        if (link.StartsWith("http://") || link.StartsWith("https://") || string.IsNullOrWhiteSpace(link))
            return link;

        if (link.StartsWith("./"))
            link = link[2..];

        if (link.StartsWith("../"))
        {
            link = RestoreLeveledRelative(link, parentUri);
            return $"{parentUri.Scheme}://{parentUri.Host}/{link}";
        }

        if (link.StartsWith('/'))
            return $"{parentUri.Scheme}://{parentUri.Host}{link}";


        return $"{parentUri.AbsoluteUri.TrimEnd('/')}/{link.TrimStart('/')}";
    }

    private static string RestoreLeveledRelative(string link, Uri parentUri)
    {
        var linkParts = link.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var depth = linkParts.Count(linkPart => linkPart.Equals(".."));
        var parentUrlParts = parentUri.LocalPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var partLength = parentUrlParts.Length;

        parentUrlParts = GetRelativePathParts(parentUrlParts, depth, x => x < partLength - depth).ToArray();
        linkParts = GetRelativePathParts(linkParts, depth, x => x >= depth).ToArray();
        
        return string.Join("/", parentUrlParts.Union(linkParts));
    }

    private static IEnumerable<string> GetRelativePathParts(IEnumerable<string> pathParts, int depth, Func<int, bool> predicate) =>
        pathParts
            .Select((p, i) => new { Part = p, Index = i })
            .Where(p => predicate(p.Index))
            .Select(p => p.Part);

    private static bool IsAcceptableSchema(this Uri uri) =>
        uri.Scheme is "http" or "https";

    private static string RemoveAnchor(string link)
    {
        var anchorIndex = link.IndexOf('#');
        return anchorIndex == -1 ? link : link[..anchorIndex];
    }

    private static bool IsInternalLink(Uri checkUri, Uri baseUri) =>
        checkUri.Host == baseUri.Host;
}