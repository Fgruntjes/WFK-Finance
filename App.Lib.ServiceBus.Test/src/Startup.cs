using App.Lib.Test;
using Microsoft.Extensions.DependencyInjection;
using Xunit.DependencyInjection.Logging;

namespace App.Lib.ServiceBus.Test;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureServices();
        services.AddLogging(x => x.AddXunitOutput());
    }
}