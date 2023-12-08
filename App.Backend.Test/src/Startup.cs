using App.Lib.Test;
using Microsoft.Extensions.DependencyInjection;

namespace App.Backend.Test;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureServices();
    }
}