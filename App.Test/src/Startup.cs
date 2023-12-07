using Microsoft.Extensions.DependencyInjection;
using Xunit.DependencyInjection.Logging;

namespace App.Test;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(x => x.AddXunitOutput());
    }
}