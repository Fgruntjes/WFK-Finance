using App.Data.Entity;

namespace App.Backend.GraphQL.Type;

public static class InstitutionMapper
{
    public static Institution ToGraphQLType(this InstitutionEntity entity)
    {
        return new Institution
        {
            Id = entity.Id,
            ExternalId = entity.ExternalId,
            Name = entity.Name,
            Logo = entity.Logo,
            Country = entity.CountryIso2,
        };
    }
}
