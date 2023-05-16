namespace App.Backend.DTO;

public class InstitutionConnection
{
    public string Id { get; init; } = null!;
    public string ExternalId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Uri ConnectUrl { get; set; } = null!;
}
