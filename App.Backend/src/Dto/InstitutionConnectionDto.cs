namespace App.Backend.Dto;

public class InstitutionConnectionDto
{
    public Guid Id { get; set; }

    public required string ExternalId { get; set; }

    public Guid InstitutionId { get; set; }

    public required Uri ConnectUrl { get; set; }

    public required ICollection<InstitutionAccountDto> Accounts { get; set; }
}