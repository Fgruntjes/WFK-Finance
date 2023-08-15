using System.ComponentModel.DataAnnotations.Schema;

namespace App.Backend.Data.Entity;

public class InstitutionEntity
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }
	public string ExternalId { get; set; } = null!;
	public string Name { get; set; } = null!;
	public string? Logo { get; set; }
	public IList<CountryEntity> Countries { get; set; } = null!;

	public InstitutionEntity()
	{
		Countries = new List<CountryEntity>();
	}
}