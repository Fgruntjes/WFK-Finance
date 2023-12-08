using App.Lib.Test;
using Microsoft.Extensions.DependencyInjection;

namespace App.Lib.InstitutionConnection.Test;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.ConfigureServices();
    }
}