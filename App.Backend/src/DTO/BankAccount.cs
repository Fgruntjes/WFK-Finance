namespace App.Backend;

public class BankAccount
{
    public string Id { get; init; } = null!;
    public string BankName { get; init; } = null!;
    public string? AccountName { get; init; }
    public string AccountNumber { get; init; } = null!;
}
