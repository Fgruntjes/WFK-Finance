using App.Lib.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VMelnalksnis.NordigenDotNet;

namespace App.IntegrationTest.Tests;

public class BankConnectFixture<TestType> : AppFixture<TestType>, IAsyncLifetime
{
    private const string _nordigenTestInstitutionId = "SANDBOXFINANCE_SFIN0000";
    private readonly INordigenClient _nordigenClient;

    public BankConnectFixture(ILoggerProvider loggerProvider, TestContext testContext) : base(loggerProvider, testContext)
    {
        _nordigenClient = Services.GetRequiredService<INordigenClient>();
    }

    public new async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // Cleanup old connections
        await foreach (var requisition in _nordigenClient.Requisitions.Get())
        {
            if (requisition.InstitutionId == _nordigenTestInstitutionId)
            {
                await _nordigenClient.Requisitions.Delete(requisition.Id);
            }
        }

        // Inject database test institute
        Database.SeedData(context =>
        {
            // Ensure we start with a clean slate
            context.InstitutionAccounts
                .Where(e => e.InstitutionConnection.Institution.ExternalId == _nordigenTestInstitutionId)
                .ExecuteDelete();
            context.InstitutionConnections
                .Where(e => e.Institution.ExternalId == _nordigenTestInstitutionId)
                .ExecuteDelete();
            context.Institutions
                .Where(e => e.ExternalId == _nordigenTestInstitutionId)
                .ExecuteDelete();

            context.Institutions.Add(new InstitutionEntity
            {
                ExternalId = _nordigenTestInstitutionId,
                Name = "TEST_INSTITUTION",
                //Logo = "https://cdn.nordigen.com/logos/sandboxfinance.png",
                CountryIso2 = "NL",
            });
        });
    }
}
