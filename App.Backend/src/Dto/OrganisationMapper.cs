using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public static class OrganisationMapper
{
    public static OrganisationDto ToDto(this OrganisationEntity entity)
    {
        return new OrganisationDto()
        {
            Id = entity.Id,
            Slug = entity.Slug,
        };
    }
}