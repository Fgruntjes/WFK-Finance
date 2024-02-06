using App.Lib.Data.Entity;
using Bogus;

namespace App.Lib.Test.Data;

public class FakeInstitutionBuilder : FakeEntityBuilder<InstitutionEntity>
{
    public FakeInstitutionBuilder()
    {
        Faker.Rules((faker, entity) =>
        {
            entity.Name = faker.Company.CompanyName();
            entity.ExternalId = faker.Random.AlphaNumeric(10);
            entity.CountryIso2 = faker.Address.CountryCode();
        });
    }
}
