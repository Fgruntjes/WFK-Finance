namespace App.Backend.Dto;

public class InstitutionTransactionDto
{
    public Guid Id { get; set; }

    public Guid InstitutionId { get; set; }

    public Guid AccountId { get; set; }

    public string AccountIban { get; set; } = null!;

    public Guid? CategoryId { get; set; }

    public DateTime Date { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string? CounterPartyName { get; set; }

    public string? CounterPartyAccount { get; set; }

    public string UnstructuredInformation { get; set; } = null!;

    public string? TransactionCode { get; set; } = null!;
}