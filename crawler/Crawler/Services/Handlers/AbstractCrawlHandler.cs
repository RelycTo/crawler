using Crawler.Interfaces.Services;

namespace Crawler.Services.Handlers;

public abstract class AbstractCrawlHandler<T> : ICrawlHandler<T> where T : class
{
    protected ICrawlHandler<T> _next;
    public virtual async Task Handle(T request, CancellationToken token = default)
    {
        if (_next != null)
            await _next.Handle(request, token);
    }

    public ICrawlHandler<T> SetNext(ICrawlHandler<T> next) => _next = next;
}