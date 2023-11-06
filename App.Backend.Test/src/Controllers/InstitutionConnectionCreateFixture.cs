using App.Backend.Controllers;
using App.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VMelnalksnis.NordigenDotNet;
using VMelnalksnis.NordigenDotNet.Requisitions;
using Xunit.Abstractions;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionCreateFixture : AppFixture<InstitutionConnectionCreateController>
{
    public Mock<INordigenClient> NordigenClientMoq { get; private set; }

    public Requisition NordigenRequisitionResult { get; private set; }

    public InstitutionEntity InstitutionEntity { get; private set; }

    public Guid InstitutionId => InstitutionEntity.Id;

    public InstitutionConnectionCreateFixture(IMessageSink logMessageSink)
        : base(logMessageSink)
    {
        InstitutionEntity = new InstitutionEntity
        {
            ExternalId = "SomeExternalId",
            Name = "Some Institution Name",
            CountryIso2 = "NL",
        };
        Database.Institutions.Add(InstitutionEntity);

        NordigenClientMoq = new Mock<INordigenClient>();
        var requisitionsMock = new Mock<IRequisitionClient>();
        NordigenClientMoq
            .SetupGet(c => c.Requisitions)
            .Returns(requisitionsMock.Object);

        NordigenRequisitionResult = new Requisition
        {
            Link = new Uri("https://www.example.com/connect-url/nordigen-requisition"),
            Id = new Guid("2ee63e58-5e7e-4bf2-820f-646f85876bac"),
        };
        requisitionsMock
            .Setup(r => r.Post(It.IsAny<RequisitionCreation>()))
            .ReturnsAsync(NordigenRequisitionResult);

        Database.SaveChanges();
    }

    protected override void RegisterMocks(IServiceCollection services)
    {
        services.AddScoped((_) => NordigenClientMoq.Object);
    }
}
