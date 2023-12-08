using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.Test;

internal class ApplicationFactory : WebApplicationFactory<Program>
{
    private readonly ILoggerProvider _loggerProvider;

    public ApplicationFactory(ILoggerProvider loggerProvider)
    {
        _loggerProvider = loggerProvider;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Override data protection configurations
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"DataProtection:Enabled", "false"},
            }!);
        });

        // Configure logging
        builder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.Services.AddSingleton(_loggerProvider);
        });

        return base.CreateHost(builder);
    }
}