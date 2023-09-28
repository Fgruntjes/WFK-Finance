using App.Backend.Data.Entity;

namespace App.Backend.GraphQL.Type;

public static class OrganisationMapper
{
    public static Organisation ToGraphQLType(this OrganisationEntity entity)
    {
        return new Organisation()
        {
            Id = entity.Id,
            Slug = entity.Slug,
        };
    }
}