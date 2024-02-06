using App.Lib.Data.Entity;
using Bogus;

namespace App.Lib.Test.Data;

public class FakeInstitutionConnectionBuilder : FakeEntityBuilder<InstitutionConnectionEntity>
{
    public FakeInstitutionConnectionBuilder(Guid organisationId, Guid? institutionId = null) : base()
    {
        Faker.Rules((faker, entity) =>
        {
            entity.OrganisationId = organisationId;
            entity.ExternalId = faker.Random.AlphaNumeric(10);
            entity.ConnectUrl = new Uri(faker.Internet.Url());
            entity.CreatedAt = faker.Noda().Instant.Recent();

            if (institutionId != null)
            {
                entity.InstitutionId = institutionId.Value;
            }
            else
            {
                var institution = new FakeInstitutionBuilder()
                    .Generate();
                entity.Institution = institution;
                entity.InstitutionId = institution.Id;
            }
        });
    }
}
