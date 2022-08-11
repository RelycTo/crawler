// See https://aka.ms/new-console-template for more information

using Crawler.DataAccess;
using Crawler.DataAccess.Models;
using Crawler.DataAccess.Services.Handlers;
using Crawler.Infrastructure;
using Crawler.Interfaces.Services;
using Crawler.Services;
using Crawler.Services.Handlers;
using Crawler.Services.Processors;
using Crawler.Shared.Interfaces;
using Crawler.Shared.Models;
using Crawler.UI;
using Crawler.UI.Handlers;
using Crawler.UI.Services;
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
        services.AddSingleton<CrawlerUI>();
        services.AddSingleton<ReportFormatter>();
        services.AddSingleton<LinkRestorer>();
        services.AddScoped<ConsoleReportBuilder>();
        services.AddSingleton<HtmlLinkParser>();
        services.AddSingleton<XmlLinkParser>();
        services.AddScoped<ILinkProcessor<HtmlLinkParser>, LinkProcessor>();
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