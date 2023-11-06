using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace App.Data.Entity;

[Index(nameof(Slug), IsUnique = true)]
public class OrganisationEntity
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public Guid Id { get; set; }
	public string Slug { get; set; } = null!;
	public ICollection<OrganisationUserEntity> Users { get; } = new List<OrganisationUserEntity>();
	public ICollection<InstitutionConnectionEntity> InstitutionConnections { get; } = new List<InstitutionConnectionEntity>();

	public OrganisationEntity()
	{
		Id = Guid.NewGuid();
	}
}