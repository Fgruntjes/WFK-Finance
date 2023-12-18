using App.Lib.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VMelnalksnis.NordigenDotNet;

namespace App.IntegrationTest.Tests;

public class NordigenFixture : AppFixture, IAsyncLifetime
{
    private const string _nordigenTestInstitutionId = "SANDBOXFINANCE_SFIN0000";
    private readonly INordigenClient _nordigenClient;

    public NordigenFixture(ILoggerProvider loggerProvider) : base(loggerProvider)
    {
        _nordigenClient = Services.GetRequiredService<INordigenClient>();
    }

    public async Task InitializeAsync()
    {
        // Cleanup old connections
        await foreach (var requisition in _nordigenClient.Requisitions.Get())
        {
            if (requisition.InstitutionId == "SANDBOXFINANCE_SFIN0000")
            {
                await _nordigenClient.Requisitions.Delete(requisition.Id);
            }
        }

        // Inject database test institute
        Database.SeedData(context =>
        {
            // Ensure we start with a clean slate
            context.InstitutionConnectionAccounts.ExecuteDelete();
            context.InstitutionConnections.ExecuteDelete();
            context.Institutions.ExecuteDelete();

            context.Institutions.Add(new InstitutionEntity
            {
                ExternalId = _nordigenTestInstitutionId,
                Name = "TEST_INSTITUTION",
                //Logo = "https://cdn.nordigen.com/logos/sandboxfinance.png",
                CountryIso2 = "NL",
            });
        });
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
