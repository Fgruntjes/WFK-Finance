using System;

namespace App.Backend;

public class BankAccount
{
    public Guid Id { get; init; }
    public string BankName { get; init; } = null!;
    public string? AccountName { get; init; }
    public string AccountNumber { get; init; } = null!;
}
