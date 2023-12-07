using App.Lib.Configuration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sentry;

namespace App.Lib.Configuration;

public static class ConfigurationExtension
{
    public static IHostBuilder UseConfiguration(this IHostBuilder builder)
    {
        builder.UseOptions<AppOptions>(AppOptions.Section);
        var sentryOptions = builder.UseOptions<SentryOptions>("Sentry");

        return builder
            .ConfigureAppConfiguration((context, config) =>
            {
                var isProduction = context.HostingEnvironment.IsProduction();
                config.AddJsonFile("appsettings.local.json", !isProduction, !isProduction);
            })
            .ConfigureLogging(_ => { })
            .ConfigureServices((hostContext, _) =>
            {
                if (!hostContext.HostingEnvironment.IsDevelopment())
                {
                    SentrySdk.Init(sentryOptions);
                }
            });
    }
    public static TOptions UseOptions<TOptions>(this IHostBuilder builder, string sectionName)
        where TOptions : class, new()
    {
        var options = new TOptions();

        builder
            .ConfigureServices((context, services) =>
            {
                context.Configuration.GetSection(sectionName)
                    .Bind(options);
                services.AddOptions<TOptions>()
                    .BindConfiguration(sectionName)
                    .ValidateDataAnnotations();
            });

        return options;
    }
}