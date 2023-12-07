using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace App.Lib.Data.Entity;

[Index(nameof(ExternalId), IsUnique = true)]
public class InstitutionConnectionEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Instant CreatedAt { get; set; }

    public string ExternalId { get; set; } = null!;

    public Uri ConnectUrl { get; set; } = null!;

    public OrganisationEntity Organisation { get; set; } = null!;

    [Required]
    public Guid OrganisationId { get; set; }

    public InstitutionEntity Institution { get; set; } = null!;

    [Required]
    public Guid InstitutionId { get; set; }

    public ICollection<InstitutionConnectionAccountEntity> Accounts { get; } =
        new List<InstitutionConnectionAccountEntity>();

    public InstitutionConnectionEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = SystemClock.Instance.GetCurrentInstant();
    }
}