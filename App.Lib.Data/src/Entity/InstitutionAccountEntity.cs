using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace App.Lib.Data.Entity;

[Index(nameof(ExternalId), nameof(InstitutionConnectionId), IsUnique = true)]
public class InstitutionAccountEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Instant CreatedAt { get; set; }

    public string ExternalId { get; set; } = null!;

    [Required]
    public Guid InstitutionConnectionId { get; set; }

    public InstitutionConnectionEntity InstitutionConnection { get; set; } = null!;

    public string Iban { get; set; } = null!;

    public ImportStatus ImportStatus { get; set; } = ImportStatus.Success;

    public Instant? LastImport { get; set; }

    public string? LastImportError { get; set; }

    public InstitutionAccountEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = SystemClock.Instance.GetCurrentInstant();
    }
}