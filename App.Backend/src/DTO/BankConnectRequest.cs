namespace App.Backend.DTO;

public class BankConnectRequest
{
    public Uri ReturnUrl { get; init; } = null!;
    public string BankId { get; init; } = null!;
}