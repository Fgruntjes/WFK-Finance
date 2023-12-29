using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public static class OrganisationMapper
{
    public static Organisation ToDto(this OrganisationEntity entity)
    {
        return new Organisation()
        {
            Id = entity.Id,
            Slug = entity.Slug,
        };
    }
}