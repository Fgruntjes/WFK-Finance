using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public static class InstitutionMapper
{
    public static Institution ToDto(this InstitutionEntity entity)
    {
        return new Institution
        {
            Id = entity.Id,
            ExternalId = entity.ExternalId,
            Name = entity.Name,
            Logo = entity.Logo,
            CountryIso2 = entity.CountryIso2,
        };
    }
}
