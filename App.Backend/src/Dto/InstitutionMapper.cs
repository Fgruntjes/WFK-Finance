using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public static class InstitutionMapper
{
    public static InstitutionDto ToDto(this InstitutionEntity entity)
    {
        return new InstitutionDto
        {
            Id = entity.Id,
            ExternalId = entity.ExternalId,
            Name = entity.Name,
            Logo = entity.Logo,
            CountryIso2 = entity.CountryIso2,
        };
    }
}
