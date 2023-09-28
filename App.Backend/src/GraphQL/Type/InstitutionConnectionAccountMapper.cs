using App.Backend.Data.Entity;

namespace App.Backend.GraphQL.Type;

public static class InstitutionConnectionAccountMapper
{
    public static InstitutionConnectionAccount ToGraphQLType(this InstitutionConnectionAccountEntity entity)
    {
        return new InstitutionConnectionAccount
        {
            Id = entity.Id,
            ExternalId = entity.ExternalId,
            Iban = entity.Iban,
        };
    }
}