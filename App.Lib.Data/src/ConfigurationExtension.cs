using App.Lib.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace App.Lib.Data;

public static class ConfigurationExtension
{
    public static IHostBuilder UseDatabase(this IHostBuilder builder)
    {
        return builder.ConfigureServices((hostContext, services) =>
        {
            var connectionString = hostContext.Configuration.GetConnectionString("Database")
                ?? throw new Exception("Missing 'ConnectionStrings::Database' setting.");

            services.AddSqlServer<DatabaseContext>(connectionString, optionsBuilder =>
            {
                optionsBuilder.ConfigureDatabaseOptions();
            });

            services
                .AddHealthChecks()
                .AddSqlServer(connectionString: connectionString, tags: new[] { AppHealthCheckExtension.ReadinessTag });
        });
    }

    public static void ConfigureDatabaseOptions(this SqlServerDbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNodaTime();
    }
}
