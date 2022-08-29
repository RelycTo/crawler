using Crawler.App.Services;
using Crawler.App.Services.Repositories;
using Crawler.DataAccess.Models;
using Crawler.DataAccess.Repositories;
using Crawler.DataAccess.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CrawlDataService = Crawler.DataAccess.Services.CrawlDataService;

namespace Crawler.DataAccess;

public static class ServiceCollectionExtensions
{
    public static void ConfigureDataAccess(this IServiceCollection services, IConfiguration configuration,
        string connectionName)
    {
        var connection = configuration.ConnectionStringValidate(connectionName);
        services.AddDbContext<CrawlerDbContext>(options => options.UseSqlServer(connection));
        services.AddTransient<ICrawlDetailsRepository, CrawlDetailsRepository>();
        services.AddTransient<ICrawlInfoRepository, CrawlInfoRepository>();
        services.AddScoped<ICrawlDataService, CrawlDataService>();
        services.AddScoped<PreProcessHandler>();
        services.AddScoped<PersistHandler>();
    }

    private static string ConnectionStringValidate(this IConfiguration configuration, string connectionName)
    {
        var connectionString = configuration.GetConnectionString(connectionName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException($"Mandatory connection string {connectionName} is not set or empty");
        }

        return connectionString;
    }
}