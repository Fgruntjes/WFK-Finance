using App.Lib.Test.Database;
using Microsoft.Extensions.DependencyInjection;
using Xunit.DependencyInjection.Logging;

namespace App.Lib.Test;

public static class StartupServiceCollectionExtension
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.RegisterDatabaseContainer();
        services.AddLogging(x => x.AddXunitOutput());
    }
}