using App.Backend.Controllers;
using App.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VMelnalksnis.NordigenDotNet;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionQueryFixture : AppFixture<InstitutionConnectionController>
{
    public IList<InstitutionConnectionEntity> OrganisationMatchEntities { get; private set; }
    public IList<InstitutionConnectionEntity> OrganisationMissmatchEntities { get; private set; }

    public Guid FirstOrganisationMatchConnection
    {
        get
        {
            return OrganisationMatchEntities.First().Id;
        }
    }

    public Guid FirstOrganisationMissmatchConnection
    {
        get
        {
            return OrganisationMissmatchEntities.First().Id;
        }
    }

    public InstitutionConnectionQueryFixture() : base()
    {
        OrganisationMatchEntities = new List<InstitutionConnectionEntity>();
        OrganisationMissmatchEntities = new List<InstitutionConnectionEntity>();

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
                ExternalId = $"SomeExternalId-organisation-missmatch-{i}",
                ConnectUrl = new Uri($"https://www.example-organisation-missmatch-{i}.com/"),
                InstitutionId = institutionsAddResult.Entity.Id,
                OrganisationId = OrganisationId,
            });
            OrganisationMissmatchEntities.Add(entity.Entity);
        }

        for (int i = 0; i < 30; i++)
        {
            var entity = Database.InstitutionConnections.Add(new InstitutionConnectionEntity()
            {
                ExternalId = $"SomeExternalId-organisation-match-{i}",
                ConnectUrl = new Uri($"https://www.example-organisation-match-{i}.com/"),
                InstitutionId = institutionsAddResult.Entity.Id,
                OrganisationId = OrganisationId,
            });
            OrganisationMatchEntities.Add(entity.Entity);
        }

        for (int i = 0; i < 3; i++)
        {
            Database.InstitutionConnectionAccounts.Add(new InstitutionConnectionAccountEntity()
            {
                ExternalId = $"SomeExternalId-organisation-account-{i}",
                InstitutionConnectionId = FirstOrganisationMatchConnection,
                Iban = $"IBAN{i}",
            });
        }

        Database.SaveChanges();
    }

    protected override void RegisterMocks(IServiceCollection services)
    {
        services.AddScoped((_) => new Mock<INordigenClient>().Object);
    }
}
