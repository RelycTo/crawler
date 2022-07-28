using crawler.Models;

namespace crawler.Extensions;

public static class CollectionExtensions
{
    public static IEnumerable<ResultItem> Merge(this IEnumerable<CrawlItem> main, IReadOnlyCollection<CrawlItem> additional)
    {
        var mergeSet = new HashSet<string>();
        foreach (var item in main)
        {
            if (mergeSet.Contains(item.Url))
                continue;

            mergeSet.Add(item.Url);
            if (additional.Contains(item))
            {
                yield return new ResultItem(item.Url, item.Duration, true, true);
            }
            else
            {
                yield return new ResultItem(item.Url, item.Duration, true, false);
            }
        }

        foreach (var item in additional)
        {
            if (mergeSet.Contains(item.Url))
                continue;
            mergeSet.Add(item.Url);
            yield return new ResultItem(item.Url, item.Duration, false, true);
        }
    }
}