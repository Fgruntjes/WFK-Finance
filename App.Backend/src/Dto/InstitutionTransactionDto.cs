namespace App.Backend.Dto;

public class InstitutionTransactionDto
{
    public Guid Id { get; set; }

    public string AccountIban { get; set; } = null!;

    public Guid InstitutionId { get; set; }

    public string UnstructuredInformation { get; set; } = null!;

    public string? CounterPartyName { get; set; }

    public string? CounterPartyAccount { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public DateTime Date { get; set; }
}