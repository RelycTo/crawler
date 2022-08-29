﻿namespace Crawler.UI.Services;

public class CrawlerUI
{
    private const int MaxThreads = 10;

    public async Task RunAsync(CrawlerSheduler scheduler, CancellationToken token = default)
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

                await scheduler.RunAsync(input, MaxThreads, token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}