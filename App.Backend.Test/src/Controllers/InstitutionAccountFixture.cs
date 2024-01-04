using App.Lib.Data.Entity;
using App.Lib.Test.Database;
using Microsoft.Extensions.Logging;

namespace App.Backend.Test.Controllers;

public class InstitutionAccountFixture : AppFixture
{
    public InstitutionAccountEntity InstitutionAccountEntity { get; }
    public InstitutionEntity InstitutionEntity { get; }

    public InstitutionAccountFixture(DatabasePool databasePool, ILoggerProvider loggerProvider)
        : base(databasePool, loggerProvider)
    {
        InstitutionEntity = new InstitutionEntity()
        {
            ExternalId = "SomeExternalId",
            Name = "Some Institution Name",
            CountryIso2 = "NL",
        };

        var institutionConnectionEntity = new InstitutionConnectionEntity()
        {
            ExternalId = "SomeExternalId-organisation-match-0",
            ConnectUrl = new Uri($"https://www.example-organisation-match-0.com/"),
            InstitutionId = InstitutionEntity.Id,
            OrganisationId = OrganisationId,
        };

        InstitutionAccountEntity = new InstitutionAccountEntity()
        {
            ExternalId = "SomeExternalId-organisation-match-0",
            Iban = "NL00BANK0123456789",
            InstitutionConnectionId = institutionConnectionEntity.Id,
        };

        Database.SeedData(context =>
        {
            context.Institutions.Add(InstitutionEntity);
            context.InstitutionConnections.Add(institutionConnectionEntity);
            context.InstitutionAccounts.Add(InstitutionAccountEntity);
        });
    }
}