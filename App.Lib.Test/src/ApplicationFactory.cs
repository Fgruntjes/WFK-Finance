using App.Backend.Test.Auth;
using App.Backend.Test.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using VMelnalksnis.NordigenDotNet;

namespace App.Lib.Test;

internal class ApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PooledDatabase _database;
    private readonly ILoggerProvider _loggerProvider;

    public ApplicationFactory(PooledDatabase database, ILoggerProvider loggerProvider)
    {
        _database = database;
        _loggerProvider = loggerProvider;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Configure logging
        builder.ConfigureLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.Services.AddSingleton(_loggerProvider);
        });

        // Override configurations
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                {"ConnectionStrings:Database", _database.ConnectionString},
                {"DataProtection:Disabled", "true"},
            });
        });

        builder.ConfigureServices(services =>
        {
            // Configure Auth
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            // Setup mocks
            // We configure as production when running integration tests due to
            services.AddSingleton<IHostEnvironment>(new HostingEnvironment { EnvironmentName = Environments.Production });
            services.MockScoped<INordigenClient>();
        });

        return base.CreateHost(builder);
    }
}