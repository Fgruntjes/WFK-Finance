using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace App.Lib.Data.Entity;

[Index(nameof(ExternalId), nameof(AccountId), IsUnique = true)]
public class InstitutionAccountTransaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Instant CreatedAt { get; set; }

    public InstitutionAccountEntity Account { get; set; } = null!;

    [Required]
    public Guid AccountId { get; set; }

    public string ExternalId { get; set; } = null!;

    public string RemittanceInformationUnstructured { get; set; } = null!;

    public string ProprietaryBankTransactionCode { get; set; } = null!;

    public string? CounterPartyName { get; set; }

    public string? CounterPartyAccount { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public Instant Date { get; set; }

    public InstitutionAccountTransaction()
    {
        Id = Guid.NewGuid();
        CreatedAt = SystemClock.Instance.GetCurrentInstant();
    }
}
