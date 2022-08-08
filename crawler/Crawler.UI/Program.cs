// See https://aka.ms/new-console-template for more information

using Crawler.Infrastructure;
using Crawler.Interfaces.Services;
using Crawler.Services;
using Crawler.Services.Handlers;
using Crawler.UI;
using Crawler.UI.Report;
using DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Models;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.SetBasePath(Directory.GetCurrentDirectory());
        /*
        context.Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    */
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient<PageLoader>();
        services.AddSingleton<CrawlerApp>();
        services.AddSingleton<CrawlerUI>();
        services.AddSingleton<ReportFormatter>();
        services.AddSingleton<LinkRestorer>();
        services.AddScoped<ConsoleReport>();
        services.AddSingleton<HtmlLinkParser>();
        services.AddSingleton<XmlLinkParser>();
        services.AddScoped<ILinkProcessor<HtmlLinkParser>, LinkProcessor>();
        services.AddScoped<ILinkProcessor<XmlLinkParser>, SiteMapProcessor>();
        services.AddScoped<ILinkProcessor, PostProcessor>();
        services.AddScoped<PreProcessHandler>();
        services.AddScoped<SiteCrawlHandler<HtmlLinkParser>>();
        services.AddScoped<SiteCrawlHandler<XmlLinkParser>>();
        services.AddScoped<PostProcessHandler>();
        services.AddScoped<PersistHandler>();

        services.ConfigureCrawlStorage(context.Configuration, "CrawlDB");
        services.AddTransient<ICrawlHandler<CrawlHandlerContext>>(x =>
        {
            var handler = x.GetRequiredService<PreProcessHandler>();

            var siteCrawl = x.GetRequiredService<ILinkProcessor<HtmlLinkParser>>();

            handler.SetNext(x.GetRequiredService<SiteCrawlHandler<HtmlLinkParser>>()
                .SetNext(x.GetRequiredService<SiteCrawlHandler<XmlLinkParser>>()
                    .SetNext(x.GetRequiredService<PostProcessHandler>()
                        .SetNext(x.GetRequiredService<PersistHandler>()))));

            return handler;
        });
    }).Build();

try
{
    var ui = host.Services.GetService<CrawlerUI>();
    if (ui == null)
        throw new InvalidOperationException("UI cannot be resolved");
    await ui.RunAsync();
}
catch (Exception e)
{
    Console.WriteLine("Application terminated unexpectedly.");
    throw;
}

