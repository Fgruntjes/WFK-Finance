using App.Lib.Data;
using App.Lib.Test;
using App.Lib.Test.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                services.MockTransient<INordigenClient>();
                services.MockTransient<IRequisitionClient>();
                services.MockTransient<IAccountClient>();
                services.MockScoped<IOrganisationIdProvider>();
            });
        var host = hostBuilder.Build();

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