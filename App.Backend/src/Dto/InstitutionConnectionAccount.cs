namespace App.Backend.Dto;

public class InstitutionConnectionAccount
{
    public Guid Id { get; set; }

    public required string ExternalId { get; set; }

    public required string Iban { get; set; }
}
