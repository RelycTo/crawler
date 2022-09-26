using Microsoft.Extensions.Configuration;

namespace Crawler.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    public static string ConnectionStringValidate(this IConfiguration configuration, string connectionName)
    {
        var connectionString = configuration.GetConnectionString(connectionName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException($"Mandatory connection string {connectionName} is not set or empty");
        }

        return connectionString;
    }
}
