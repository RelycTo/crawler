namespace Crawler.Shared.Interfaces;

public interface ICrawlerScheduler
{
    Task RunAsync(string url, int taskCount, CancellationToken token = default);
}
