using App.Backend.Controllers;
using App.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Accounts;
using VMelnalksnis.NordigenDotNet.Requisitions;
using Xunit.Abstractions;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionRefreshFixture : AppFixture<InstitutionConnectionRefreshController>
{
    private readonly Mock<INordigenClient> _nordigenClientMoq;

    public InstitutionConnectionEntity InstitutionConnectionEntity { get; private set; }

    public InstitutionConnectionRefreshFixture(IMessageSink logMessageSink)
        : base(logMessageSink)
    {
        var institutionEntity = new InstitutionEntity
        {
            ExternalId = "SomeExternalId",
            Name = "Some Institution Name",
            CountryIso2 = "NL",
        };
        Database.Institutions.Add(institutionEntity);

        InstitutionConnectionEntity = new InstitutionConnectionEntity
        {
            Id = new Guid("3accd6ff-bdf3-450c-bc39-5fc40a4816aa"),
            OrganisationId = OrganisationId,
            ConnectUrl = new Uri("https://www.example.com/connect-url/refresh"),
            ExternalId = "ed69f988-a1fb-4e89-8d56-66b42e43a675"
        };
        Database.InstitutionConnections.Add(InstitutionConnectionEntity);

        _nordigenClientMoq = new Mock<INordigenClient>();

        var requisitionsMock = new Mock<IRequisitionClient>();
        var accountsMock = new Mock<IAccountClient>();
        _nordigenClientMoq
            .SetupGet(c => c.Requisitions)
            .Returns(requisitionsMock.Object);
        _nordigenClientMoq
            .SetupGet(c => c.Accounts)
            .Returns(accountsMock.Object);

        var nordigenAccounts = new List<Account> {
            new() {
                Id = new Guid("93328c83-3cae-49f9-bbe6-3ff5dfc38359"),
                InstitutionId = institutionEntity.ExternalId,
                Iban = "IBAN-1",
            },
            new() {
                Id = new Guid("8e2ff322-1a0a-4bd7-a88c-728e0816c43f"),
                InstitutionId = institutionEntity.ExternalId,
                Iban = "IBAN-2",
            },
        };

        var nordigenRequisitionResult = new Requisition
        {
            Link = new Uri("https://www.example.com/connect-url/nordigen-requisition"),
            Id = new Guid("2ee63e58-5e7e-4bf2-820f-646f85876bac"),
            Accounts = nordigenAccounts.Select(a => a.Id).ToList(),
        };

        requisitionsMock
            .Setup(r => r.Get(Guid.Parse(InstitutionConnectionEntity.ExternalId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(nordigenRequisitionResult);

        foreach (var account in nordigenAccounts)
        {
            accountsMock
                .Setup(a => a.Get(account.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);
        }

        Database.SaveChanges();
    }

    protected override void RegisterMocks(IServiceCollection services)
    {
        services.AddScoped((_) => _nordigenClientMoq.Object);
    }
}
