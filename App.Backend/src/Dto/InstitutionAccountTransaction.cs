namespace App.Backend.Dto;

public class InstitutionAccountTransaction
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid AccountId { get; set; }

    public string ExternalId { get; set; } = null!;

    public string UnstructuredInformation { get; set; } = null!;

    public string? TransactionCode { get; set; }

    public string? CounterPartyName { get; set; }

    public string? CounterPartyAccount { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public DateTime Date { get; set; }
}
