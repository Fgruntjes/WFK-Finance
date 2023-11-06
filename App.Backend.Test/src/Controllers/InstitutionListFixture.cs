using App.Backend.Controllers;
using App.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VMelnalksnis.NordigenDotNet;
using Xunit.Abstractions;

namespace App.Backend.Test.Controllers;

public class InstitutionListFixture : AppFixture<InstitutionListController>
{
    public Guid InstitutionIdNldLinked { get; private set; }
    public Mock<INordigenClient> NordigenClientMoq { get; private set; } = null!;

    public InstitutionListFixture(IMessageSink logMessageSink) : base(logMessageSink)
    {
        var institutionNldLinked = new InstitutionEntity()
        {
            Name = "MyFakeName-NL",
            ExternalId = "SomeExternalId-NL",
            CountryIso2 = "NL"
        };
        var institutionUsaLinked = new InstitutionEntity()
        {
            Name = "MyFakeName-US",
            ExternalId = "SomeExternalId-US",
            CountryIso2 = "US"
        };

        Database.Institutions.Add(institutionNldLinked);
        Database.Institutions.Add(institutionUsaLinked);
        Database.SaveChanges();

        InstitutionIdNldLinked = institutionNldLinked.Id;
    }

    protected override void RegisterMocks(IServiceCollection services)
    {
        NordigenClientMoq = new Mock<INordigenClient>();

        services.AddScoped((_) => NordigenClientMoq.Object);
    }
}
