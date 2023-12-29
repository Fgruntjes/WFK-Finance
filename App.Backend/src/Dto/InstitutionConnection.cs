namespace App.Backend.Dto;

public class InstitutionConnection
{
    public Guid Id { get; set; }

    public string ExternalId { get; set; } = null!;

    public Guid InstitutionId { get; set; }

    public Uri ConnectUrl { get; set; } = null!;

    public ICollection<InstitutionConnectionAccount> Accounts { get; set; } = null!;
}
