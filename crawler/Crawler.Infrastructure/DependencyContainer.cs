using Crawler.Application.Handlers;
using Crawler.Application.Models;
using Crawler.Application.Services;
using Crawler.Application.Services.Loaders;
using Crawler.Application.Services.Parsers;
using Crawler.Application.Services.Processors;
using Crawler.Application.Services.Repositories;
using Crawler.Application.Services.Utilities;
using Crawler.Domain.Handlers;
using Crawler.Infrastructure.Loaders;
using Crawler.Infrastructure.Parsers;
using Crawler.Infrastructure.Utilities;
using Crawler.Persistence.Handlers;
using Crawler.Persistence.Models;
using Crawler.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Crawler.Infrastructure;

public class DependencyContainer
{
    public static void AddServices(IServiceCollection services)
    {
        services.AddHttpClient<IPageLoader, PageLoader>();
        services.AddSingleton<ILinkRestorer, LinkRestorer>();
        services.AddSingleton<IHtmlParser, HtmlLinkParser>();
        services.AddSingleton<IXmlParser, XmlLinkParser>();
        services.AddScoped<CrawlDataService>();
        services.AddScoped<ILinkProcessor<IHtmlParser>, PageProcessor>();
        services.AddScoped<ILinkProcessor<IXmlParser>, SiteMapProcessor>();
        services.AddScoped<ILinkProcessor, PostProcessor>();
        services.AddScoped<SiteCrawlHandler<IHtmlParser>>();
        services.AddScoped<SiteCrawlHandler<IXmlParser>>();
        services.AddScoped<PostProcessHandler>();

        services.AddTransient<ICrawlDetailsRepository, CrawlDetailsRepository>();
        services.AddTransient<ICrawlInfoRepository, CrawlInfoRepository>();
        services.AddScoped<PreProcessHandler>();
        services.AddScoped<PersistHandler>();


        //Chain configuration
        services.AddTransient<ICrawlHandler<CrawlHandlerContext>>(x =>
        {
            var handler = x.GetRequiredService<PreProcessHandler>();

            var siteCrawl = x.GetRequiredService<SiteCrawlHandler<IHtmlParser>>();
            var siteMapCrawl = x.GetRequiredService<SiteCrawlHandler<IXmlParser>>();
            var postProcess = x.GetRequiredService<PostProcessHandler>();
            var persist = x.GetRequiredService<PersistHandler>();

            handler
                .SetNext(siteCrawl)
                .SetNext(siteMapCrawl)
                .SetNext(postProcess)
                .SetNext(persist);
            return handler;
        });
    }

    public static void AddDbContext(IServiceCollection services, string connectionString)
    {

        services.AddDbContext<CrawlerDbContext>(options => options.UseSqlServer(connectionString));
    }
}
