using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public static class InstitutionConnectionAccountMapper
{
    public static InstitutionConnectionAccount ToDto(this InstitutionAccountEntity entity)
    {
        return new InstitutionConnectionAccount
        {
            Id = entity.Id,
            ExternalId = entity.ExternalId,
            Iban = entity.Iban,
        };
    }
}