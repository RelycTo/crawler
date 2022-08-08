// See https://aka.ms/new-console-template for more information

using Crawler;
using Crawler.Infrastructure;
using Crawler.Interfaces.HandlerRequests;
using Crawler.Interfaces.Services;
using Crawler.Services;
using Crawler.Services.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


static void ConfigureService( /*IConfiguration configuration, */ IServiceCollection services)
{
    services.AddHttpClient<PageLoader>();
    services.AddSingleton<CrawlerUI>();
    services.AddSingleton<ProcessDispatcher>();
    services.AddSingleton<ReportFormatter>();
    services.AddScoped<ConsoleReport>();
    services.AddSingleton<HtmlLinkParser>();
    services.AddSingleton<XmlLinkParser>();
    services.AddScoped<ILinkProcessor<HtmlLinkParser>, LinkProcessor>();
    services.AddScoped<ILinkProcessor<XmlLinkParser>, SiteMapProcessor>();
    services.AddScoped<PostProcessor>();
    services.AddScoped<PreProcessHandler>();
    services.AddScoped<SiteCrawlHandler>();
    services.AddScoped<SiteMapCrawlHandler>();
    services.AddScoped<PostProcessCrawlHandler>();

    //services.ConfigureCrawlStorage(configuration, "CrawlDB");
    services.AddTransient<ICrawlHandler<ICrawlRequest>>(x =>
    {
        var handler = x.GetRequiredService<PreProcessHandler>();
        handler.SetNext(x.GetRequiredService<SiteCrawlHandler>()
            .SetNext(x.GetRequiredService<SiteMapCrawlHandler>()
                .SetNext(x.GetRequiredService<PostProcessCrawlHandler>()
                    .SetNext(x.GetRequiredService<CrawlPersistHandler>()))));

        return handler;
    });
}

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((_, builder) => builder.SetBasePath(Directory.GetCurrentDirectory()))
    .ConfigureServices((_, services) =>
    {
        ConfigureService(services);
    }).Build();

var ui = host.Services.GetService<CrawlerUI>();
if(ui == null)
    throw new InvalidOperationException("UI cannot be resolved");
await ui.RunAsync();