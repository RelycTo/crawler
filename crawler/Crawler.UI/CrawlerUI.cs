using Crawler.Services;
using Crawler.UI.Services;

namespace Crawler.UI;

public class CrawlerUI
{
    private const int MaxThreads = 10;
    private readonly CrawlerScheduler _crawler;

    public CrawlerUI(CrawlerScheduler crawler)
    {
        _crawler = crawler;
    }

    public async Task RunAsync(CancellationToken token = default)
    {
        while (true)
        {
            try
            {
                Console.WriteLine("Enter URL to process or press enter to exit:");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    break;

                if (!Uri.IsWellFormedUriString(input, UriKind.Absolute))
                    throw new ArgumentException("Input URL is not in the correct format.");

                await _crawler.Run(input, MaxThreads, token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}