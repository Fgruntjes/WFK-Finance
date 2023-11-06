using App.Backend.Controllers;
using App.Data.Entity;
using GraphQL.AspNet.Configuration;
using Xunit.Abstractions;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionGetFixture : AppFixture<InstitutionConnectionGetController>
{
    public Guid OrganisationMatchConnection { get; private set; }

    public Guid OrganisationMissmatchConnection { get; private set; }

    public InstitutionConnectionGetFixture(IMessageSink logMessageSink)
        : base(logMessageSink)
    {
        var MissmatchOrganisationId = new Guid("45b7202b-35ed-4557-add0-b4c2b7a86748");

        var institutionsAddResult = Database.Institutions.Add(new InstitutionEntity()
        {
            ExternalId = "SomeExternalId",
            Name = "Some Institution Name",
            CountryIso2 = "NL",
        });

        var missMatchEntity = Database.InstitutionConnections.Add(new InstitutionConnectionEntity()
        {
            ExternalId = $"SomeExternalId-organisation-missmatch-0",
            ConnectUrl = new Uri($"https://www.example-organisation-missmatch-0.com/"),
            InstitutionId = institutionsAddResult.Entity.Id,
            OrganisationId = MissmatchOrganisationId,
        });
        OrganisationMissmatchConnection = missMatchEntity.Entity.Id;

        var entity = Database.InstitutionConnections.Add(new InstitutionConnectionEntity()
        {
            ExternalId = $"SomeExternalId-organisation-match-0",
            ConnectUrl = new Uri($"https://www.example-organisation-match-0.com/"),
            InstitutionId = institutionsAddResult.Entity.Id,
            OrganisationId = OrganisationId,
        });
        OrganisationMatchConnection = entity.Entity.Id;

        for (int i = 0; i < 3; i++)
        {
            Database.InstitutionConnectionAccounts.Add(new InstitutionConnectionAccountEntity()
            {
                ExternalId = $"SomeExternalId-organisation-account-{i}",
                InstitutionConnectionId = OrganisationMatchConnection,
                Iban = $"IBAN{i}",
            });
        }

        Database.SaveChanges();
    }

    protected override void ConfigureGraphQL(SchemaOptions options)
    {
        base.ConfigureGraphQL(options);

        options.AddController<InstitutionConnectionExtensionController>();
    }
}
