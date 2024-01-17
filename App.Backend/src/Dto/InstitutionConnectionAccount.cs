using App.Lib.Data.Entity;

namespace App.Backend.Dto;

public class InstitutionAccountDto
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public required string ExternalId { get; set; }

    public required string Iban { get; set; }

    public ImportStatus ImportStatus { get; set; }

    public DateTime? LastImport { get; set; }

    public int TransactionCount { get; set; }
}
