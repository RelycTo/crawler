using Crawler.Application.Crawl;
using Crawler.Application.Crawl.Processors;
using Crawler.Application.Data;
using Crawler.Application.Data.Repositories;
using Crawler.Application.Infrastructure;
using Crawler.Infrastructure.Extensions;
using Crawler.Infrastructure.Http;
using Crawler.Infrastructure.Parsers;
using Crawler.Persistence.Models;
using Crawler.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crawler.Infrastructure;

public class DependencyContainer
{
    public static void AddServices(IServiceCollection services)
    {
        services.AddHttpClient<IPageLoader, PageLoader>();
        services.AddSingleton<LinkRestorer>();
        services.AddSingleton<IHtmlParser, HtmlLinkParser>();
        services.AddSingleton<IXmlParser, XmlLinkParser>();
        services.AddScoped<CrawlDataService>();
        services.AddScoped<ILinkProcessor<IHtmlParser>, PageProcessor>();
        services.AddScoped<ILinkProcessor<IXmlParser>, SiteMapProcessor>();
        services.AddScoped<PostProcessor>();
        services.AddScoped<CrawlService>();

        services.AddTransient<ICrawlDetailsRepository, CrawlDetailsRepository>();
        services.AddTransient<ICrawlInfoRepository, CrawlInfoRepository>();
    }

    public static void AddDbContext(IServiceCollection services, IConfiguration configuration, string connection)
    {
        var connectionString = configuration.ConnectionStringValidate(connection);

        services.AddDbContext<CrawlerDbContext>(options => options.UseSqlServer(connectionString));
    }

    public static void EnsureMigration(IServiceProvider provider)
    {
        using var scope = provider.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<CrawlerDbContext>();
        context.Database.Migrate();
    }
}