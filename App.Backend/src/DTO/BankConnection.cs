namespace App.Backend;

public class BankConnection
{
    public string Id { get; init; } = null!;
    public string ExternalId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Uri ConnectUrl { get; set; } = null!;
}
