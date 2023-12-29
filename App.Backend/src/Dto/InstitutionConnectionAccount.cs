namespace App.Backend.Dto;

public class InstitutionConnectionAccount
{
    public Guid Id { get; set; }

    public string ExternalId { get; set; } = null!;

    public string Iban { get; set; } = null!;
}
