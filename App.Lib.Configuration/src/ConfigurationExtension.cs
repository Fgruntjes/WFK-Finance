using App.Lib.Configuration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NodaTime;
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
                config.AddJsonFileTraverse("appsettings.local.json", !isProduction, !isProduction);
            })
            .ConfigureLogging(_ => { })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IClock>(SystemClock.Instance);
                services.AddSingleton(DateTimeZoneProviders.Tzdb);

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

    private static IConfigurationBuilder AddJsonFileTraverse(this IConfigurationBuilder builder, string fileName, bool optional, bool reloadOnChange)
    {
        var directory = Directory.GetCurrentDirectory();
        var fileProvider = new PhysicalFileProvider(directory);
        var fileInfo = fileProvider.GetFileInfo(fileName);

        while (!fileInfo.Exists && directory != null)
        {
            directory = Directory.GetParent(directory)?.FullName;
            if (directory == null) continue;

            fileProvider = new PhysicalFileProvider(directory);
            fileInfo = fileProvider.GetFileInfo(fileName);
        }

        if (fileInfo.Exists)
        {
            builder.AddJsonFile(fileProvider, fileName, optional, reloadOnChange);
        }
        else if (!optional)
        {
            throw new FileNotFoundException($"The configuration file '{fileName}' was not found and is not optional.");
        }

        return builder;
    }
}