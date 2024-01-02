namespace App.Backend.Dto;

public class InstitutionConnection
{
    public Guid Id { get; set; }

    public required string ExternalId { get; set; }

    public Guid InstitutionId { get; set; }

    public required Uri ConnectUrl { get; set; }

    public required ICollection<InstitutionConnectionAccount> Accounts { get; set; }
}