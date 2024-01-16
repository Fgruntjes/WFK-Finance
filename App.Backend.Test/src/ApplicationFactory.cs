using App.Backend.Test.Auth;
using App.Institution.Interface;
using App.Lib.Test.Database;
using App.Lib.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.Backend.Test;

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
        // Override configurations
        builder.ConfigureLogging(_loggerProvider);
        builder.ConfigureDatabase(_database.ConnectionString);
        builder.ConfigureServiceBus();

        builder.ConfigureServices(services =>
        {
            // Configure Auth
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            // Setup mocks
            services.MockScoped<IInstitutionConnectionCreateService>();
            services.MockScoped<IInstitutionConnectionRefreshService>();
            services.MockScoped<IInstitutionSearchService>();
            services.MockScoped<ITransactionImportQueueService>();
        });

        return base.CreateHost(builder);
    }
}