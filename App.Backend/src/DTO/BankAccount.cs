namespace App.Backend.DTO;

public class BankAccount
{
    public string Id { get; init; } = null!;
    public string? AccountName { get; init; }
    public string AccountNumber { get; init; } = null!;
    public InstitutionConnection InstitutionConnection { get; init; } = null!;
}