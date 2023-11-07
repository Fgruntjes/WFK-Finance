using App.Backend.Test.Database;
using App.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit.DependencyInjection.Logging;

namespace App.Backend.Test;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.RegisterDatabaseContainer();
        services.AddLogging(x => x.AddXunitOutput());

        services.AddScoped<AppFixture>();
        services.RegisterMigrationInitializer<DatabaseContext>();
    }
}