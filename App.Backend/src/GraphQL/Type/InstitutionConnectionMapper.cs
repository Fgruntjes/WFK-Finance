using App.Lib.Data.Entity;

namespace App.Backend.GraphQL.Type;

public static class InstitutionConnectionMapper
{
    public static InstitutionConnection ToGraphQLType(this InstitutionConnectionEntity entity)
    {
        return new InstitutionConnection
        {
            Id = entity.Id,
            ExternalId = entity.ExternalId,
            InstitutionId = entity.InstitutionId,
            ConnectUrl = entity.ConnectUrl,
        };
    }
}