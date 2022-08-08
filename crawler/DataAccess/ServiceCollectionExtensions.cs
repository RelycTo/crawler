using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class ServiceCollectionExtensions
{
    public static void ConfigureCrawlStorage(this IServiceCollection services, IConfiguration configuration,
        string connectionName)
    {
        var connection = configuration.ConnectionStringValidate(connectionName);
        services.AddDbContext<CrawlerDbContext>(options => options.UseSqlServer(connection));
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