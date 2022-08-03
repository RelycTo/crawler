// See https://aka.ms/new-console-template for more information

using Crawler;
using Crawler.Infrastructure;
using Crawler.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


static void ConfigureService(IServiceCollection services)
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