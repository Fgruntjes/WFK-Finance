using App.Lib.Data.Entity;
using Bogus;

namespace App.Lib.Test.Data;

public class FakeTransactionCategoryBuilder : FakeEntityBuilder<TransactionCategoryEntity>
{
    public FakeTransactionCategoryBuilder(Guid organisationId)
    {
        Faker.Rules((faker, entity) =>
        {
            entity.OrganisationId = organisationId;
            entity.Name = faker.Company.CompanyName();
            entity.Group = faker.PickRandom<TransactionCategoryGroup>();
        });
    }
}
