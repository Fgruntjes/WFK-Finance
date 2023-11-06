using App.Backend.Controllers;
using App.Data.Entity;
using Moq;
using VMelnalksnis.NordigenDotNet;
using Xunit.Abstractions;

namespace App.Backend.Test.Controllers;

public class InstitutionGetFixture : AppFixture<InstitutionGetController>
{
    public Guid InstitutionId { get; private set; }
    public Mock<INordigenClient> NordigenClientMoq { get; private set; } = null!;

    public InstitutionGetFixture(IMessageSink logMessageSink) : base(logMessageSink)
    {
        var institutionEntity = new InstitutionEntity()
        {
            Name = "MyFakeName-NL",
            ExternalId = "SomeExternalId-NL",
            CountryIso2 = "NL"
        };

        Database.Institutions.Add(institutionEntity);
        Database.SaveChanges();

        InstitutionId = institutionEntity.Id;
    }
}
