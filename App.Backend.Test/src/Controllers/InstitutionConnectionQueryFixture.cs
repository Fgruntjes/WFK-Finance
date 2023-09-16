using App.Backend.Controllers;
using App.Backend.Data.Entity;

namespace App.Backend.Test.Controllers;

public class InstitutionConnectionQueryFixture : AppFixture<InstitutionConnectionQuery>
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
            return OrganisationMatchEntities.First().Id;
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
        });

        for (int i = 0; i < 30; i++)
        {
            var entity = Database.InstitutionConnections.Add(new InstitutionConnectionEntity()
            {
                ExternalId = $"SomeExternalId-organisation-missmatch-{i}",
                ConnectUrl = new Uri($"https://www.example-organisation-missmatch-{i}.com/"),
                InstitutionId = institutionsAddResult.Entity.Id,
                OrganisationId = new Guid("71119dce-59d9-426a-8cd3-4f770b72f2ed"),
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
                OrganisationId = HttpContextAccessorExtension.OrganisationGuid,
            });
            OrganisationMatchEntities.Add(entity.Entity);
        }

        Database.SaveChanges();
    }
}
