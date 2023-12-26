using App.Lib.Configuration;
using App.Lib.Data;
using App.Lib.ServiceBus;
using App.Lib.ServiceBus.Messages;
using App.Lib.Test;
using App.Lib.Test.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Accounts;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Lib.InstitutionConnection.Test;

public class AppFixture : FunctionalTestFixture
{
    public IServiceProvider Services { get; }

    public AppFixture(DatabasePool databasePool, ILoggerProvider loggerProvider) : base(databasePool)
    {
        var hostBuilder = Host.CreateDefaultBuilder()
            .UseDatabase()
            .UseInstitutionConnectionClient()
            .ConfigureLogging(loggerProvider)
            .ConfigureDatabase(Database.ConnectionString)
            .ConfigureServices(services =>
            {
                services.MockSingleton<IClock>();
                services.MockTransient<INordigenClient>();
                services.MockTransient<IRequisitionClient>();
                services.MockTransient<IAccountClient>();
                services.MockScoped<IOrganisationIdProvider>();
                services.MockSingleton<IServiceBus>();
            });
        var host = hostBuilder.Build();

        host.Services.WithMock<IServiceBus>(mock =>
        {
            mock.Setup(m => m.Send(
                It.IsAny<InstitutionAccountTransactionImportJob>(),
                It.IsAny<CancellationToken>()));
        });

        host.Services.WithMock<INordigenClient>(mock =>
        {
            mock
                .SetupGet(c => c.Requisitions)
                .Returns(host.Services.GetRequiredService<IRequisitionClient>());

            mock
                .SetupGet(c => c.Accounts)
                .Returns(host.Services.GetRequiredService<IAccountClient>());
        });
        host.Services.WithMock<IOrganisationIdProvider>(mock =>
        {
            mock.Setup(m => m.GetOrganisationId())
                .Returns(OrganisationId);
        });

        Services = host.Services;
    }
}