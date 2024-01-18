using Microsoft.Extensions.DependencyInjection;
using Xunit.DependencyInjection.Logging;

namespace App.IntegrationTest;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<TestContext>();
        services.AddLogging(x => x.AddXunitOutput());
    }
}
