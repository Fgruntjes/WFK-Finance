using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.Lib.Test;

public static class HostBuilderExtension
{
    public static IHostBuilder ConfigureLogging(this IHostBuilder builder, ILoggerProvider loggerProvider)
    {
        return builder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(LogLevel.Trace);
            loggingBuilder.Services.AddSingleton(loggerProvider);
        });
    }

    public static IHostBuilder ConfigureDatabase(this IHostBuilder builder, string connectionString)
    {
        return builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:Database", connectionString},
            });
        });
    }

    public static IHostBuilder ConfigureServiceBus(this IHostBuilder builder)
    {
        return builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:ServiceBus", "inmemory://"},
                {"ServiceBus:QuitWhenIdle", "false"}
            });
        });
    }
}