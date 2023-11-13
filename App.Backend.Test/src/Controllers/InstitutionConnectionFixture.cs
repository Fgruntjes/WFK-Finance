using App.Backend.Test.Database;
using App.Data.Entity;
using Microsoft.Extensions.Logging;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionFixture : AppFixture
{
    public InstitutionEntity InstitutionEntity { get; private set; }
    public InstitutionConnectionEntity InstitutionConnectionEntity { get; private set; }

    public InstitutionConnectionFixture(DatabasePool databasePool, ILoggerProvider loggerProvider)
        : base(databasePool, loggerProvider)
    {
        InstitutionEntity = new InstitutionEntity()
        {
            ExternalId = "SomeExternalId",
            Name = "Some Institution Name",
            CountryIso2 = "NL",
        };

        InstitutionConnectionEntity = new InstitutionConnectionEntity()
        {
            ExternalId = "SomeExternalId-organisation-match-0",
            ConnectUrl = new Uri($"https://www.example-organisation-match-0.com/"),
            InstitutionId = InstitutionEntity.Id,
            OrganisationId = OrganisationId,
        };

        SeedData(context =>
        {
            context.Institutions.Add(InstitutionEntity);
            context.InstitutionConnections.Add(InstitutionConnectionEntity);
        });
    }
}
