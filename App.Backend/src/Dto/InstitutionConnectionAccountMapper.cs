using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public static class InstitutionConnectionAccountMapper
{
    public static InstitutionAccountDto ToDto(this InstitutionAccountEntity entity)
    {
        return new InstitutionAccountDto
        {
            Id = entity.Id,
            InstitutionId = entity.InstitutionConnection.InstitutionId,
            ExternalId = entity.ExternalId,
            Iban = entity.Iban,
            ImportStatus = entity.ImportStatus,
            LastImport = entity.LastImport?.ToDateTimeUtc(),
        };
    }
}