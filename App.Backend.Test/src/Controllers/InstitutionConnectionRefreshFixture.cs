using App.Lib.Test.Database;
using App.Lib.Data.Entity;
using Microsoft.Extensions.Logging;

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

        Database.SeedData(context =>
        {
            context.Institutions.Add(InstitutionEntity);
            context.InstitutionConnections.Add(InstitutionConnectionEntity);
            context.InstitutionConnections.Add(OrganisationMissmatchInstitutionConnectionEntity);
        });
    }
}
