namespace Crawler.Interfaces.Services;

public interface ICrawlHandler<T> where T : class
{
    Task Handle(T request, CancellationToken token = default);
    ICrawlHandler<T> SetNext(ICrawlHandler<T> next);
}