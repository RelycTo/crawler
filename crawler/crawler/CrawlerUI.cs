using crawler.Infrastructure;
using crawler.Services;

namespace crawler;

public class CrawlerUI
{
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
                
                var processorFactory = ProcessorFactory.Create(new PageLoader(new HttpClient()));
                var report = new ConsoleReport(new ReportFormatter());
                var dispatcher = new ProcessDispatcher(processorFactory, 10, report);
                
                await dispatcher.Run(input, token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}