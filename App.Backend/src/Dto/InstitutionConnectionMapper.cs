using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public static class InstitutionConnectionMapper
{
    public static InstitutionConnectionDto ToDto(this InstitutionConnectionEntity entity)
    {
        return new InstitutionConnectionDto
        {
            Id = entity.Id,
            ExternalId = entity.ExternalId,
            InstitutionId = entity.InstitutionId,
            ConnectUrl = entity.ConnectUrl,
            Accounts = entity.Accounts
                .Select(a => a.ToDto())
                .ToList(),
        };
    }
}