using App.Lib.Data;
using App.Lib.Test;
using App.Lib.Test.Database;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace App.TransactionCategory.Test;

public class AppFixture : FunctionalTestFixture
{
    public IServiceProvider Services { get; }

    public AppFixture(DatabasePool databasePool, ILoggerProvider loggerProvider) : base(databasePool)
    {
        var hostBuilder = Host.CreateDefaultBuilder()
            .UseDatabase()
            .UseTransactionCategory()
            .ConfigureLogging(loggerProvider)
            .ConfigureDatabase(Database.ConnectionString)
            .ConfigureServices(services =>
            {
                services.MockSingleton<IClock>();
                services.MockScoped<IOrganisationIdProvider>();
            });
        var host = hostBuilder.Build();

        host.Services.WithMock<IOrganisationIdProvider>(mock =>
        {
            mock.Setup(m => m.GetOrganisationId())
                .Returns(OrganisationId);
        });

        Services = host.Services;
    }
}