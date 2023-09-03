using System.ComponentModel.DataAnnotations;

namespace App.Backend.Data.Entity;

public class CountryEntity
{
	[Key]
	public string Iso3 { get; set; } = null!;

	public ICollection<InstitutionEntity> Institutions { get; } = new List<InstitutionEntity>();
}