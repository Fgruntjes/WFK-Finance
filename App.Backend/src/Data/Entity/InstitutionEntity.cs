using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Backend.Data.Entity;

public class InstitutionEntity
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }

	[DefaultValue("now()")]
	public DateTime CreatedAt { get; set; }

	public string ExternalId { get; set; } = null!;
	public string Name { get; set; } = null!;
	public Uri? Logo { get; set; }
	public ICollection<CountryEntity> Countries { get; set; } = new List<CountryEntity>();
}