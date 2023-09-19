using App.Backend.Controllers;
using App.Backend.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Accounts;
using VMelnalksnis.NordigenDotNet.Requisitions;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionMutationFixture : AppFixture<InstitutionConnectionController>
{
    public Mock<INordigenClient> NordigenClientMoq { get; private set; }

    public Requisition NordigenRequisitionResult { get; private set; }

    public InstitutionConnectionEntity InstitutionConnectionEntity { get; private set; }

    public InstitutionEntity InstitutionEntity { get; private set; }

    public Guid InstitutionId => InstitutionEntity.Id;

    public Guid InstitutionConnectionDeleteId { get; internal set; }

    public InstitutionConnectionMutationFixture() : base()
    {
        InstitutionEntity = new InstitutionEntity
        {
            ExternalId = "SomeExternalId",
            Name = "Some Institution Name",
            CountryIso2 = "NL",
        };
        Database.Institutions.Add(InstitutionEntity);

        InstitutionConnectionEntity = new InstitutionConnectionEntity
        {
            Id = new Guid("3accd6ff-bdf3-450c-bc39-5fc40a4816aa"),
            OrganisationId = HttpContextAccessorExtension.OrganisationGuid,
            ConnectUrl = new Uri("https://www.example.com/connect-url/refresh"),
            ExternalId = "ed69f988-a1fb-4e89-8d56-66b42e43a675"
        };
        Database.InstitutionConnections.Add(InstitutionConnectionEntity);

        InstitutionConnectionDeleteId = new Guid("56612691-dcf7-44e2-b506-ba83b60de5a9");
        var institutionConnectionDeleteEntity = new InstitutionConnectionEntity
        {
            Id = InstitutionConnectionDeleteId,
            OrganisationId = HttpContextAccessorExtension.OrganisationGuid,
            ConnectUrl = new Uri("https://www.example.com/connect-url/refresh"),
            ExternalId = "ed69f988-a1fb-4e89-8d56-66b42e43a675"
        };
        Database.InstitutionConnections.Add(institutionConnectionDeleteEntity);

        NordigenClientMoq = new Mock<INordigenClient>();

        var requisitionsMock = new Mock<IRequisitionClient>();
        var accountsMock = new Mock<IAccountClient>();
        NordigenClientMoq
            .SetupGet(c => c.Requisitions)
            .Returns(requisitionsMock.Object);
        NordigenClientMoq
            .SetupGet(c => c.Accounts)
            .Returns(accountsMock.Object);

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

        NordigenRequisitionResult = new Requisition
        {
            Link = new Uri("https://www.example.com/connect-url/nordigen-requisition"),
            Id = new Guid("2ee63e58-5e7e-4bf2-820f-646f85876bac"),
            Accounts = nordigenAccounts.Select(a => a.Id).ToList(),
        };
        requisitionsMock
            .Setup(r => r.Post(It.IsAny<RequisitionCreation>()))
            .ReturnsAsync(NordigenRequisitionResult);
        requisitionsMock
            .Setup(r => r.Get(Guid.Parse(InstitutionConnectionEntity.ExternalId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(NordigenRequisitionResult);

        foreach (var account in nordigenAccounts)
        {
            accountsMock
                .Setup(a => a.Get(account.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(account);
        }

        Database.SaveChanges();
    }

    protected override void MockServices(IServiceCollection services)
    {
        services.AddScoped((_) => NordigenClientMoq.Object);
    }
}
