using App.Backend.Controllers;
using App.Data.Entity;
using Xunit.Abstractions;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionListFixture : AppFixture<InstitutionConnectionListController>
{
    public InstitutionConnectionListFixture(IMessageSink logMessageSink)
        : base(logMessageSink)
    {
        var institutionsAddResult = Database.Institutions.Add(new InstitutionEntity()
        {
            ExternalId = "SomeExternalId",
            Name = "Some Institution Name",
            CountryIso2 = "NL",
        });

        for (int i = 0; i < 30; i++)
        {
            var entity = Database.InstitutionConnections.Add(new InstitutionConnectionEntity()
            {
                ExternalId = $"SomeExternalId-organisation-match-{i}",
                ConnectUrl = new Uri($"https://www.example-organisation-match-{i}.com/"),
                InstitutionId = institutionsAddResult.Entity.Id,
                OrganisationId = OrganisationId,
            });
        }

        Database.SaveChanges();
    }
}
