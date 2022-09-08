// See https://aka.ms/new-console-template for more information

using Crawler.App.DTOs;
using Crawler.App.Handlers;
using Crawler.App.Infrastructure;
using Crawler.App.Infrastructure.Loaders;
using Crawler.App.Infrastructure.Parsers;
using Crawler.App.Services;
using Crawler.App.Services.Processors;
using Crawler.DataAccess;
using Crawler.DataAccess.Models;
using Crawler.DataAccess.Services.Handlers;
using Crawler.Entities.Handlers;
using Crawler.Presenter.Handlers;
using Crawler.Presenter.Services;
using Crawler.Presenter.Services.ReportServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.SetBasePath(Directory.GetCurrentDirectory());
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient<PageLoader>();
        services.AddSingleton<CrawlerScheduler>();
        services.AddSingleton<ConsoleReport>();
        services.AddSingleton<CrawlerUI>();
        services.AddSingleton<ReportFormatter>();
        services.AddSingleton<LinkRestorer>();
        services.AddSingleton<HtmlLinkParser>();
        services.AddSingleton<XmlLinkParser>();
        services.AddScoped<ConsoleReportBuilder>();
        services.AddScoped<CrawlDataService>();
        services.AddScoped<ILinkProcessor<HtmlLinkParser>, PageProcessor>();
        services.AddScoped<ILinkProcessor<XmlLinkParser>, SiteMapProcessor>();
        services.AddScoped<ILinkProcessor, PostProcessor>();
        services.AddScoped<SiteCrawlHandler<HtmlLinkParser>>();
        services.AddScoped<SiteCrawlHandler<XmlLinkParser>>();
        services.AddScoped<ReportHandler>();
        services.AddScoped<PostProcessHandler>();
        services.ConfigureDataAccess(context.Configuration, "CrawlDB");

        //Chain configuration
        services.AddTransient<ICrawlHandler<CrawlHandlerContext>>(x =>
        {
            var handler = x.GetRequiredService<PreProcessHandler>();

            var siteCrawl = x.GetRequiredService<SiteCrawlHandler<HtmlLinkParser>>();
            var siteMapCrawl = x.GetRequiredService<SiteCrawlHandler<XmlLinkParser>>();
            var postProcess = x.GetRequiredService<PostProcessHandler>();
            var persist = x.GetRequiredService<PersistHandler>();
            var report = x.GetRequiredService<ReportHandler>();

            handler
                .SetNext(siteCrawl)
                .SetNext(siteMapCrawl)
                .SetNext(postProcess)
                .SetNext(persist)
                .SetNext(report);
            return handler;
        });
    }).Build();

try
{
    using (var scope = host.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<CrawlerDbContext>();
        context.Database.Migrate();
    }

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