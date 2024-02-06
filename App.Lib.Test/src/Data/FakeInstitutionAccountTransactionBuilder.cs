using App.Lib.Data.Entity;
using Bogus;

namespace App.Lib.Test.Data;

public class FakeInstitutionAccountTransactionBuilder : FakeEntityBuilder<InstitutionAccountTransactionEntity>
{
    public FakeInstitutionAccountTransactionBuilder(Guid accountId)
    {
        Faker.Rules((faker, entity) =>
        {
            entity.ExternalId = faker.Random.AlphaNumeric(10);
            entity.AccountId = accountId;
            entity.UnstructuredInformation = faker.Lorem.Sentence();
            entity.Amount = faker.Finance.Amount();
            entity.Currency = "EUR";
            entity.Date = faker.Noda().Instant.Recent();
        });
    }
}
