using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Backend.Data.Entity;

public class InstitutionConnectionAccountEntity
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }

	[DefaultValue("now()")]
	public DateTime CreatedAt { get; set; }

	public string ExternalId { get; set; } = null!;

	[Required]
	public Guid InstitutionConnectionId { get; set; }

	public InstitutionConnectionEntity InstitutionConnection { get; set; } = null!;

	public string OwnerName { get; set; } = null!;

	public string Iban { get; set; } = null!;
}