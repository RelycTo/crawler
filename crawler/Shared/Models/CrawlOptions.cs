namespace Shared.Models;

public class CrawlOptions
{
    public CrawlOptions(Uri uri, int taskCount)
    {
        Uri = uri;
        TaskCount = taskCount;
        ExcludedMediaTypes = Array.Empty<string>();
    }

    public IReadOnlyCollection<string> ExcludedMediaTypes { get; private set; }
    public Uri Uri { get; }
    public int TaskCount { get; }

    public CrawlOptions SetExcludedMediaTypes(IEnumerable<string> mediaTypes)
    {
        ExcludedMediaTypes = mediaTypes.ToArray();
        return this;
    }
}
