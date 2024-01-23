using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace App.Lib.Data.Entity;

public class TransactionCategoryEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Instant CreatedAt { get; set; }

    public required string Name { get; set; }

    public Guid? ParentId { get; set; }

    public TransactionCategoryEntity? Parent { get; set; }

    public required Guid OrganisationId { get; set; }

    public required CategoryGroup Group { get; set; }

    public TransactionCategoryEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = SystemClock.Instance.GetCurrentInstant();
    }
}
