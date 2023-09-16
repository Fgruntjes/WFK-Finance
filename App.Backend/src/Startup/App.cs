using App.Backend.Service;
using NodaTime;

namespace App.Backend.Startup;

public static class App
{
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddHttpContextAccessor();
        services.AddSingleton<IClock>(SystemClock.Instance);
        services.AddSingleton(DateTimeZoneProviders.Tzdb);
        services.Scan(scan => scan.FromAssemblyOf<Program>()
            .AddClasses(classes => classes.InNamespaceOf<InstitutionConnectionCreateService>())
                .AsSelf()
                .WithScopedLifetime());

    }
}