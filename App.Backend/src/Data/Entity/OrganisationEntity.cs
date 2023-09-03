using System.ComponentModel.DataAnnotations.Schema;

namespace App.Backend.Data.Entity;

public class OrganisationEntity
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public Guid Id { get; set; }
	public string Slug { get; set; } = null!;
	public ICollection<OrganisationUserEntity> Users { get; } = new List<OrganisationUserEntity>();
	public ICollection<InstitutionConnectionEntity> InstitutionConnections { get; } = new List<InstitutionConnectionEntity>();
}