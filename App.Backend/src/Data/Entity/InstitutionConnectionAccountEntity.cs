using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;

namespace App.Backend.Data.Entity;

public class InstitutionConnectionAccountEntity
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

	public InstitutionConnectionAccountEntity()
	{
		Id = Guid.NewGuid();
		CreatedAt = SystemClock.Instance.GetCurrentInstant();
	}
}