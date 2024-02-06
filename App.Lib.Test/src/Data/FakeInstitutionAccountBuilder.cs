using App.Lib.Data.Entity;
using Bogus;

namespace App.Lib.Test.Data;

public class FakeInstitutionAccountBuilder : FakeEntityBuilder<InstitutionAccountEntity>
{
    public FakeInstitutionAccountBuilder(Guid organisationId)
        : this(new FakeInstitutionConnectionBuilder(organisationId).Generate())
    { }

    public FakeInstitutionAccountBuilder(InstitutionConnectionEntity institutionConnection) : base()
    {
        Faker.Rules((faker, entity) =>
        {
            entity.InstitutionConnection = institutionConnection;
            entity.InstitutionConnectionId = institutionConnection.Id;
            entity.ExternalId = faker.Random.AlphaNumeric(10);
            entity.Iban = faker.Finance.Iban();
        });
    }
}
