namespace Crawler.Domain.Handlers;

public interface ICrawlHandler<T> where T : class
{
    Task Handle(T context, CancellationToken token = default);
    ICrawlHandler<T> SetNext(ICrawlHandler<T> next);
}