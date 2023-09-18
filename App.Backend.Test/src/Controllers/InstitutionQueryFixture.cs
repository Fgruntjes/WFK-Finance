using App.Backend.Controllers;
using App.Backend.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VMelnalksnis.NordigenDotNet;

namespace App.Backend.Test.Controllers;

public class InstitutionQueryFixture : AppFixture<InstitutionQuery>
{
    public Guid InstitutionIdNldLinked { get; private set; }
    public Mock<INordigenClient> NordigenClientMoq { get; private set; }

    public InstitutionQueryFixture() : base()
    {
        var countryNld = new CountryEntity { Iso2 = "nld" };
        var countryUsa = new CountryEntity { Iso2 = "usa" };
        var institutionNldLinked = new InstitutionEntity()
        {
            Name = "MyFakeName-nld",
            ExternalId = "SomeExternalId-nld",
            Countries = new List<CountryEntity>() { countryNld }
        };
        var institutionUsaLinked = new InstitutionEntity()
        {
            Name = "MyFakeName-usa",
            ExternalId = "SomeExternalId-usa",
            Countries = new List<CountryEntity>() { countryUsa }
        };

        Database.Countries.Add(countryNld);
        Database.Countries.Add(countryUsa);
        Database.Institutions.Add(institutionNldLinked);
        Database.Institutions.Add(institutionUsaLinked);
        Database.SaveChanges();

        InstitutionIdNldLinked = institutionNldLinked.Id;
    }

    protected override void MockServices(IServiceCollection services)
    {
        NordigenClientMoq = new Mock<INordigenClient>();

        services.AddScoped((_) => NordigenClientMoq.Object);
    }
}
