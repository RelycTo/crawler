namespace Crawler.Entities.Handlers;

public abstract class AbstractCrawlHandler<T> : ICrawlHandler<T> where T : class
{
    protected ICrawlHandler<T> _next;

    public virtual async Task Handle(T context, CancellationToken token = default)
    {
        if (_next != null)
        {
            await _next.Handle(context, token);
        }
    }

    public ICrawlHandler<T> SetNext(ICrawlHandler<T> next) => _next = next;
}