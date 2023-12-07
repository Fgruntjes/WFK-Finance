using App.Backend.Test.Database;
using App.Lib.Data.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Accounts;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionRefreshFixture : AppFixture
{
    public InstitutionEntity InstitutionEntity { get; }
    public InstitutionConnectionEntity InstitutionConnectionEntity { get; }
    public InstitutionConnectionEntity OrganisationMissmatchInstitutionConnectionEntity { get; }

    public InstitutionConnectionRefreshFixture(DatabasePool databasePool, ILoggerProvider loggerProvider)
        : base(databasePool, loggerProvider)
    {
        InstitutionEntity = new InstitutionEntity()
        {
            ExternalId = "e5ea445f-ef0b-4084-a347-29ac9f4b03ac",
            Name = "Some Institution Name",
            CountryIso2 = "NL",
        };

        InstitutionConnectionEntity = new InstitutionConnectionEntity()
        {
            ExternalId = "82185ec1-75d6-43dc-96ec-9d54ee7bc683",
            ConnectUrl = new Uri($"https://www.example-organisation-match-0.com/"),
            InstitutionId = InstitutionEntity.Id,
            OrganisationId = OrganisationId,
        };

        OrganisationMissmatchInstitutionConnectionEntity = new InstitutionConnectionEntity()
        {
            ExternalId = "45588298-f719-419f-b9d5-bfa8fc239a29",
            ConnectUrl = new Uri($"https://www.example-organisation-match-0.com/"),
            InstitutionId = InstitutionEntity.Id,
            OrganisationId = AltOrganisationId,
        };

        SeedData(context =>
        {
            context.Institutions.Add(InstitutionEntity);
            context.InstitutionConnections.Add(InstitutionConnectionEntity);
            context.InstitutionConnections.Add(OrganisationMissmatchInstitutionConnectionEntity);
        });

        var requisitionsMock = new Mock<IRequisitionClient>();
        var accountsMock = new Mock<IAccountClient>();
        WithMock<INordigenClient>(mock =>
        {
            mock
                .SetupGet(c => c.Requisitions)
                .Returns(requisitionsMock.Object);
            mock
                .SetupGet(c => c.Accounts)
                .Returns(accountsMock.Object);
        });

        var nordigenAccounts = new List<Account> {
            new() {
                Id = new Guid("93328c83-3cae-49f9-bbe6-3ff5dfc38359"),
                InstitutionId = InstitutionEntity.ExternalId,
                Iban = "IBAN-1",
            },
            new() {
                Id = new Guid("8e2ff322-1a0a-4bd7-a88c-728e0816c43f"),
                InstitutionId = InstitutionEntity.ExternalId,
                Iban = "IBAN-2",
            },
        };

        var externalGuid = Guid.Parse(InstitutionConnectionEntity.ExternalId);
        var nordigenRequisitionResult = new Requisition
        {
            Link = new Uri("https://www.example.com/connect-url/nordigen-requisition"),
            Id = externalGuid,
            Accounts = nordigenAccounts.Select(a => a.Id).ToList(),
        };

        requisitionsMock
            .Setup(r => r.Get(externalGuid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(nordigenRequisitionResult);

        foreach (var account in nordigenAccounts)
        {
            accountsMock
                .Setup(a => a.Get(account.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);
        }
    }
}
